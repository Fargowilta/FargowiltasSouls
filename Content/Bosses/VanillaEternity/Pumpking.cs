using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class Pumpking : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Pumpking);

        public int AttackTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer > 300)
            {
                AttackTimer = 0;
                if (npc.whoAmI == NPC.FindFirstNPC(npc.type) && FargoSoulsUtil.HostCheck)
                {
                    for (int j = -1; j <= 1; j++) //fire these to either side of target
                    {
                        if (j == 0)
                            continue;

                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 vel = npc.SafeDirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.Pi / 6 * (Main.rand.NextDouble() - 0.5) + MathHelper.Pi / 2 * j);
                            float ai0 = Main.rand.NextFloat(1.04f, 1.05f);
                            float ai1 = Main.rand.NextFloat(0.03f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<PumpkingFlamingScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 2), 0f, Main.myPlayer, ai0, ai1);
                        }
                    }
                }
            }
            /*if (++Counter[2] >= 12)
            {
                Counter[2] = 0;
                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1)
                {
                    Player player = Main.player[t];
                    Vector2 distance = player.Center - npc.Center;
                    if (Math.Abs(distance.X) < npc.width && FargoSoulsUtil.HostCheck) //flame rain if player roughly below me
                        Projectile.NewProjectile(npc.Center.X, npc.position.Y, Main.rand.Next(-3, 4), Main.rand.Next(-4, 0), Main.rand.Next(326, 329), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 0.8f), 0f, Main.myPlayer);
                }
            }*/
        }
    }

    public class PumpkingPart : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.Pumpking, NPCID.PumpkingBlade);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<RottingBuff>(), 900);
            target.AddBuff(ModContent.BuffType<LivingWastelandBuff>(), 900);
        }
    }
}
