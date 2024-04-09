using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Crimson
{
    public class BloodFeeder : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BloodFeeder);

        public int AttackTimer;
        public int TrueMaxLife;
        public float DamageMultiplier = 1f;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(TrueMaxLife);
            binaryWriter.Write(DamageMultiplier);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            TrueMaxLife = binaryReader.Read7BitEncodedInt();
            DamageMultiplier = binaryReader.ReadSingle();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 2;

            TrueMaxLife = npc.lifeMax;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.damage = (int)(npc.defDamage * DamageMultiplier);

            if (++AttackTimer > 60)
            {
                AttackTimer = 0;

                if (FargoSoulsUtil.HostCheck)
                {
                    bool needsUpdate = false;

                    foreach (NPC target in Main.npc.Where(
                      n => n.active && n.type != npc.type && !n.boss && n.CanBeChasedBy() && npc.Distance(n.Center) < 300 && npc.Hitbox.Intersects(n.Hitbox)))
                    {
                        target.SimpleStrikeNPC(npc.damage, Math.Sign(target.Center.X - npc.Center.X), knockBack: 4f);

                        OnHitSomething(npc, npc.damage);

                        needsUpdate = true;
                    }

                    if (needsUpdate)
                        NetSync(npc);
                }
            }
        }

        private void OnHitSomething(NPC npc, int damage)
        {
            npc.life += damage;
            if (DamageMultiplier < 10)
            {
                npc.lifeMax += damage;
                TrueMaxLife += damage;
            }
            if (npc.life > npc.lifeMax)
                npc.life = npc.lifeMax;
            CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage * 2);

            DamageMultiplier += 0.2f;
            if (DamageMultiplier > 10)
                DamageMultiplier = 10;

            npc.damage = (int)(npc.defDamage * DamageMultiplier);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Bleeding, 300);

            OnHitSomething(npc, hurtInfo.Damage);

            npc.netUpdate = true;
            NetSync(npc, false);
        }
    }
}
