using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;
using FargowiltasSouls.NPCs.EternityMode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GlowRing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Ring");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 0;
            //Projectile.timeLeft = 1200;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public Color color = new Color(255, 255, 255, 0);

        public override void AI()
        {

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
            if (npc != null)
                Projectile.Center = npc.Center;

            float scale = 12f;
            int maxTime = 30;
            bool customScaleAlpha = false;

            switch ((int)Projectile.ai[1])
            {
                case -24: //baron debuff apply at opening
                    {
                        //customScaleAlpha = true;
                        maxTime = 60;
                        float modifier = Projectile.localAI[0] / maxTime;
                        scale = 1 + (50 * modifier);
                        color = Color.Blue;
                        Projectile.alpha = (int)(255f * (1f - modifier));
                        color.A = 0;
                    }
                    break;
                case -23: //eridanus general punch telegraph
                    {
                        customScaleAlpha = true;
                        maxTime = 90;
                        float modifier = Projectile.localAI[0] / maxTime;
                        color = new Color(51, 255, 191) * modifier;
                        Projectile.alpha = (int)(255f * (1f - modifier));
                        Projectile.scale = 3f * 9f * (1f - modifier);
                    }
                    break;

                case -22: //wof vanilla laser telegraph
                    {
                        customScaleAlpha = true;
                        maxTime = 645;

                        if (npc != null && npc.type == NPCID.WallofFleshEye && (npc.GetGlobalNPC<WallofFleshEye>().HasTelegraphedNormalLasers || Main.netMode == NetmodeID.MultiplayerClient))
                        {
                            Projectile.rotation = npc.rotation + (npc.direction > 0 ? 0 : MathHelper.Pi);
                            Projectile.velocity = Projectile.rotation.ToRotationVector2();
                            Projectile.Center = npc.Center + (npc.width - 52) * Vector2.UnitX.RotatedBy(Projectile.rotation);

                            if (Projectile.localAI[0] < npc.localAI[1])
                                Projectile.localAI[0] = (int)npc.localAI[1];

                            float modifier = (float)Math.Cos(Math.PI / 2 / maxTime * Projectile.localAI[0]);

                            color = new Color(255, 0, 255, 100) * (1f - modifier);
                            Projectile.alpha = (int)(255f * modifier);
                            Projectile.scale = 18f * modifier;
                        }
                        else
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;

                case -21: //default but small, devi uses this for becoming back money
                    scale = 4f;
                    maxTime = 60;
                    break;

                case -20: //eridanus punch windup
                    {
                        customScaleAlpha = true;
                        maxTime = 200;
                        float modifier = Projectile.localAI[0] / maxTime;
                        color = new Color(51, 255, 191) * modifier;
                        Projectile.alpha = (int)(255f * (1f - modifier));
                        Projectile.scale = 3f * 6f * (1f - modifier);
                    }
                    break;

                case -19: //abom dash
                    color = Color.Yellow;
                    color.A = 0;
                    scale = 18f;
                    break;

                case -18: //eridanus timestop
                    scale = 36f;
                    maxTime = 120;
                    break;

                case -17: //devi smallest pink
                    scale = 6f;
                    goto case -16;

                case -16: //devi scaling pink
                    color = new Color(255, 51, 153, 0);
                    break;

                case -15: //devi scaling pink
                    scale = 18f;
                    goto case -16;

                case -14: //deviantt biggest pink
                    scale = 24f;
                    goto case -16;

                case -13: //wof reticle
                    color = new Color(93, 255, 241, 0);
                    scale = 6f;
                    maxTime = 15;
                    break;

                case -12: //nature shroomite blue
                    color = new Color(0, 0, 255, 0);
                    maxTime = 45;
                    break;

                case -11: //nature chlorophyte green
                    color = new Color(0, 255, 0, 0);
                    maxTime = 45;
                    break;

                case -10: //nature frost cyan
                    color = new Color(0, 255, 255, 0);
                    maxTime = 45;
                    break;

                case -9: //nature rain yellow
                    color = new Color(255, 255, 0, 0);
                    maxTime = 45;
                    break;

                case -8: //nature molten orange
                    color = new Color(255, 127, 40, 0);
                    maxTime = 45;
                    break;

                case -7: //nature crimson red
                    color = new Color(255, 0, 0, 0);
                    maxTime = 45;
                    break;

                case -6: //will, spirit champ yellow
                    color = new Color(255, 255, 0, 0);
                    scale = 18f;
                    break;

                case -5: //shadow champ purple
                    color = new Color(200, 0, 255, 0);
                    scale = 18f;
                    break;

                case -4: //life champ yellow
                    color = new Color(255, 255, 0, 0);
                    scale = 18f;
                    maxTime = 60;
                    break;

                case -3: //earth champ orange
                    color = new Color(255, 100, 0, 0);
                    scale = 18f;
                    maxTime = 60;
                    break;

                case -2: //ml teal cyan
                    color = new Color(51, 255, 191, 0);
                    scale = 18f;
                    break;

                case -1: //purple shadowbeam
                    color = new Color(200, 0, 200, 0);
                    maxTime = 60;
                    break;

                case NPCID.EyeofCthulhu:
                    color = new Color(51, 255, 191, 0);
                    maxTime = 45;
                    break;

                case NPCID.QueenBee:
                    color = new Color(255, 255, 100, 0);
                    maxTime = 45;
                    break;

                case NPCID.WallofFleshEye:
                    color = new Color(93, 255, 241, 0);
                    scale = 12f;
                    maxTime = 30;
                    break;

                case NPCID.Retinazer:
                    color = new Color(255, 0, 0, 0);
                    scale = 24f;
                    maxTime = 60;
                    break;

                case NPCID.PrimeCannon:
                case NPCID.PrimeLaser:
                case NPCID.PrimeSaw:
                case NPCID.PrimeVice:
                    color = new Color(51, 255, 191, 0);
                    scale = 12f;
                    maxTime = 30;
                    break;

                case NPCID.QueenSlimeBoss:
                    color = Color.HotPink;
                    color.A = 200;
                    scale = 6f;
                    maxTime = 60;

                    if (Projectile.localAI[0] > maxTime * 0.25f && NPC.AnyNPCs(ModContent.NPCType<GelatinSubject>()))
                        Projectile.localAI[0] = maxTime * 0.25f;

                    if (npc != null)
                        Projectile.Center = npc.Bottom + npc.height / 2 * -Vector2.UnitY.RotatedBy(npc.rotation);
                    break;

                case NPCID.CultistBoss:
                    color = new Color(255, 127, 40, 0);
                    break;

                case NPCID.MoonLordHand:
                case NPCID.MoonLordHead:
                case NPCID.MoonLordCore:
                    color = new Color(51, 255, 191, 0);
                    scale = 12f;
                    maxTime = 60;
                    break;

                case NPCID.Deerclops:
                    color = Color.LightSkyBlue;
                    color.A = 0;
                    scale = 9f;
                    maxTime = 30;

                    if (npc != null)
                        Projectile.Center = npc.direction < 0 ? npc.TopLeft : npc.TopRight;
                    break;

                default:
                    Main.NewText("glow ring: you shouldnt be seeing this text, show terry");
                    break;
            }

            if (++Projectile.localAI[0] > maxTime)
            {
                Projectile.Kill();
                return;
            }

            if (!customScaleAlpha)
            {
                Projectile.scale = scale * (float)Math.Sin(Math.PI / 2 * Projectile.localAI[0] / maxTime);
                Projectile.alpha = (int)(255f * Projectile.localAI[0] / maxTime);
            }

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == NPCID.QueenSlimeBoss)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
                GameShaders.Misc["HallowBoss"].Apply(new Terraria.DataStructures.DrawData?());
            }

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            if (Projectile.ai[1] == NPCID.QueenSlimeBoss)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            return false;
        }
    }
}