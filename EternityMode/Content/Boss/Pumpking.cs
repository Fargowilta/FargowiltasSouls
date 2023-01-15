using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss
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
                if (npc.whoAmI == NPC.FindFirstNPC(npc.type) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int j = -1; j <= 1; j++) //fire these to either side of target
                    {
                        if (j == 0)
                            continue;

                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.Pi / 6 * (Main.rand.NextDouble() - 0.5) + MathHelper.Pi / 2 * j);
                            float ai0 = Main.rand.NextFloat(1.04f, 1.05f);
                            float ai1 = Main.rand.NextFloat(0.03f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<PumpkingFlamingScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 2), 0f, Main.myPlayer, ai0, ai1);
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
                    if (Math.Abs(distance.X) < npc.width && Main.netMode != NetmodeID.MultiplayerClient) //flame rain if player roughly below me
                        Projectile.NewProjectile(npc.Center.X, npc.position.Y, Main.rand.Next(-3, 4), Main.rand.Next(-4, 0), Main.rand.Next(326, 329), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                }
            }*/
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<PumpkingsCape>(), 5));
        }
    }

    public class PumpkingPart : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.Pumpking, NPCID.PumpkingBlade);
        
        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Rotting>(), 900);
            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
        }
    }
}
