﻿using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Nebula
{
    public class LunarTowerNebula : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerNebula;
            set => NPC.ShieldStrengthTowerNebula = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerNebula);

        public LunarTowerNebula() : base(ModContent.BuffType<ReverseManaFlowBuff>(), 58) { }

        public override void ShieldsDownAI(NPC npc)
        {
            if (--AttackTimer < 0)
            {
                AttackTimer = 300;
                npc.TargetClosest(false);
                NetSync(npc);
                for (int i = 0; i < 40; ++i)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PinkTorch, 0.0f, 0.0f, 0, new Color(), 1f);
                    Dust dust = Main.dust[d];
                    dust.velocity *= 4f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale += 1.5f;
                }
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) < 3000)
                {
                    int x = (int)Main.player[npc.target].Center.X / 16;
                    int y = (int)Main.player[npc.target].Center.Y / 16;
                    for (int i = 0; i < 100; i++)
                    {
                        int newX = x + Main.rand.Next(10, 31) * (Main.rand.NextBool() ? 1 : -1);
                        int newY = y + Main.rand.Next(-15, 16);
                        Vector2 newPos = new(newX * 16, newY * 16);
                        if (!Collision.SolidCollision(newPos, npc.width, npc.height))
                        {
                            //npc.Center = newPos;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), newPos, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 10, npc.whoAmI);
                            break;
                        }
                    }
                }
                for (int i = 0; i < 40; ++i)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PinkTorch, 0.0f, 0.0f, 0, new Color(), 1f);
                    Dust dust = Main.dust[d];
                    dust.velocity *= 4f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale += 1.5f;
                }
                SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                npc.netUpdate = true;
            }

            //if (++Counter[2] > 60)
            //{
            //    Counter[2] = 0;
            //    npc.TargetClosest(false);
            //    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) < 5000)
            //    {
            //        for (int i = 0; i < 3; i++)
            //        {
            //            Vector2 position = Main.player[npc.target].Center;
            //            position.X += Main.rand.Next(-150, 151);
            //            position.Y -= Main.rand.Next(600, 801);
            //            Vector2 speed = Main.player[npc.target].Center - position;
            //            speed.Normalize();
            //            speed *= 10f;
            //            Projectile.NewProjectile(position, speed, ProjectileID.NebulaLaser, 40, 0f, Main.myPlayer);
            //        }
            //    }
            //}
        }
    }
}
