using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GlowRingHollow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Ring");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;

            Projectile.hide = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public Color color = Color.White;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.timeLeft = 2;

            float radius = 500f;
            int maxTime = 60;
            int alphaModifier = 3;

            switch ((int)Projectile.ai[0])
            {
                case 1: //mutant reti glaive
                    Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
                    color = Color.Red;
                    radius = 525;
                    maxTime = 90;
                    alphaModifier = 2;
                    break;

                case 2: //mutant spaz glaive
                    Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
                    color = Color.Green;
                    radius = 350;
                    maxTime = 90;
                    alphaModifier = 2;
                    break;

                case 3: //abom emode p2 dash telegraph
                    {
                        color = Color.Yellow;
                        maxTime = 180;
                        alphaModifier = 10;
                        if (Projectile.localAI[0] > maxTime / 2)
                        {
                            alphaModifier = -1;
                            Projectile.alpha = 0;
                        }

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                        radius = 1400f * (maxTime - Projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 4: //betsy electrosphere boundary
                    color = Color.Cyan;
                    radius = 1200;
                    maxTime = 360;
                    break;

                case 5: //mutant subphase transition
                    Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
                    color = new Color(51, 255, 191);
                    maxTime = 120;
                    radius = 1200 * (float)Math.Cos(Math.PI / 2 * Projectile.localAI[0] / maxTime);
                    alphaModifier = -1;
                    Projectile.alpha = 0;
                    break;

                case 6: //destroyer coil tell
                    {
                        color = Color.Purple;
                        maxTime = 120;
                        alphaModifier = Projectile.localAI[0] > maxTime / 2 ? 10 : 1;
                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.TheDestroyer);
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                        radius = 1200f * (maxTime - Projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 7: //life champ dash tell
                    {
                        color = Color.Yellow;
                        alphaModifier = 10;
                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.Champions.LifeChampion>());
                        if (npc != null && npc.ai[3] == 0)
                        {
                            Projectile.Center = npc.Center;

                            maxTime = npc.localAI[2] == 1 ? 30 : 60;

                            if (npc.ai[1] == 0)
                                Projectile.localAI[0] = 0;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                        radius = 1800f * (maxTime - Projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 8: //boc confused tell
                    color = Color.Red;
                    maxTime = 60;
                    alphaModifier = 3;
                    radius = Projectile.ai[1] * (float)Math.Sqrt(Math.Sin(Math.PI / 2 * Projectile.localAI[0] / maxTime));
                    break;

                case 9: //destroyer light show tell
                    {
                        color = Color.Yellow;
                        maxTime = 120;
                        alphaModifier = Projectile.localAI[0] > maxTime / 2 ? 10 : 1;
                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.TheDestroyer);
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                        radius = 1200f * (maxTime - Projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 10: //nebula tower tp
                    {
                        color = Color.Violet;
                        maxTime = 90;
                        alphaModifier = 10;
                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.LunarTowerNebula);
                        if (npc != null)
                        {
                            if (Projectile.localAI[0] == maxTime)
                            {
                                npc.Center = Projectile.Center;

                                for (int i = 0; i < 100; i++)
                                {
                                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 86, Scale: 4f);
                                    Main.dust[d].velocity *= 4f;
                                    Main.dust[d].noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = -2; i <= 2; i++)
                                    {
                                        Vector2 spawnPoint = npc.Center + Vector2.UnitX * i * 1000;
                                        for (int j = -1; j <= 1; j += 2)
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPoint, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 15f, MathHelper.PiOver2 * j);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                        radius = 1200f * (maxTime - Projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 11: //retinazer aura
                    {
                        color = Color.Red;

                        if (Projectile.localAI[0] > maxTime / 2) //NEVER fade normally
                            Projectile.localAI[0] = maxTime / 2;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.Retinazer);
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                            radius = 2000 - 1200 * npc.GetGlobalNPC<Retinazer>().AuraRadiusCounter / 180f;
                            if (FargoSoulsWorld.MasochistModeReal)
                                radius *= 0.75f;
                            if (radius == 2000)
                                Projectile.localAI[0] = -1;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 12: //terra champ tell
                    {
                        color = Color.OrangeRed;
                        maxTime = 300 - 90;
                        alphaModifier = Projectile.localAI[0] > maxTime / 2 ? 10 : 1;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.Champions.TerraChampion>());
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center + Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2) * 300;
                            radius = 2000f * (1f - Projectile.localAI[0] / maxTime);
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case 13: //wof arena
                    {
                        color = Color.Orange;
                        radius = 2000f;

                        if (Projectile.localAI[0] > maxTime / 2) //NEVER fade normally
                            Projectile.localAI[0] = maxTime / 2;

                        NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.WallofFlesh);
                        if (npc != null)
                        {
                            Projectile.Center = npc.Center;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                default:
                    Main.NewText("glow ring hollow: you shouldnt be seeing this text, show terry");
                    break;
            }

            if (++Projectile.localAI[0] > maxTime)
            {
                Projectile.Kill();
                return;
            }

            if (alphaModifier >= 0)
            {
                Projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * Projectile.localAI[0])) * alphaModifier;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            color.A = 0;

            Projectile.scale = radius * 2f / 1000f;

            //Projectile.position = Projectile.Center;
            //Projectile.width = Projectile.height = (int)(1000 * Projectile.scale);
            //Projectile.Center = Projectile.position;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}