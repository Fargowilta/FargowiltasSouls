using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.EternityMode.NPCMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Crimson
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

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bool needsUpdate = false;

                    foreach (NPC target in Main.npc.Where(
                      n => n.active && n.type != npc.type && !n.boss && n.CanBeChasedBy() && npc.Distance(n.Center) < 300 && npc.Hitbox.Intersects(n.Hitbox)))
                    {
                        target.StrikeNPC(npc.damage, 4f, Math.Sign(target.Center.X - npc.Center.X));

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

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Bleeding, 300);

            OnHitSomething(npc, damage);

            npc.netUpdate = true;
            NetSync(npc, false);
        }
    }
}
