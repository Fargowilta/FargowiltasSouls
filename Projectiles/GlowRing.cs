using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;

namespace FargowiltasSouls.Projectiles
{
    public class GlowRing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Glow Ring");
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 0;
            //projectile.timeLeft = 1200;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public Color color = new Color(255, 255, 255, 0);

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0]);
            if (npc != null)
                projectile.Center = npc.Center;

            float scale = 12f;
            int maxTime = 30;
            bool customScaleAlpha = false;

            switch ((int)projectile.ai[1])
            {
                case -23: //eridanus general punch telegraph
                    {
                        customScaleAlpha = true;
                        maxTime = 90;
                        float modifier = projectile.localAI[0] / maxTime;
                        color = new Color(51, 255, 191) * modifier;
                        projectile.alpha = (int)(255f * (1f - modifier));
                        projectile.scale = 3f * 9f * (1f - modifier);
                    }
                    break;

                case -22: //wof vanilla laser telegraph
                    {
                        customScaleAlpha = true;
                        maxTime = 645;

                        if (npc != null && npc.type == NPCID.WallofFleshEye && (npc.GetEModeNPCMod<WallofFleshEye>().HasTelegraphedNormalLasers || Main.netMode == NetmodeID.MultiplayerClient))
                        {
                            projectile.rotation = npc.rotation + (npc.direction > 0 ? 0 : MathHelper.Pi);
                            projectile.velocity = projectile.rotation.ToRotationVector2();
                            projectile.Center = npc.Center + (npc.width - 52) * Vector2.UnitX.RotatedBy(projectile.rotation);

                            if (projectile.localAI[0] < npc.localAI[1])
                                projectile.localAI[0] = (int)npc.localAI[1];

                            float modifier = (float)Math.Cos(Math.PI / 2 / maxTime * projectile.localAI[0]);

                            color = new Color(255, 0, 255, 100) * (1f - modifier);
                            projectile.alpha = (int)(255f * modifier);
                            projectile.scale = 18f * modifier;
                        }
                        else
                        {
                            projectile.Kill();
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
                        float modifier = projectile.localAI[0] / maxTime;
                        color = new Color(51, 255, 191) * modifier;
                        projectile.alpha = (int)(255f * (1f - modifier));
                        projectile.scale = 3f * 6f * (1f - modifier);
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

                case NPCID.PrimeSaw:
                case NPCID.PrimeVice:
                    color = new Color(255, 0, 0, 0);
                    scale = 12f;
                    maxTime = 30;
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

                default:
                    break;
            }

            if (++projectile.localAI[0] > maxTime)
            {
                projectile.Kill();
                return;
            }

            if (!customScaleAlpha)
            {
                projectile.scale = scale * (float)Math.Sin(Math.PI / 2 * projectile.localAI[0] / maxTime);
                projectile.alpha = (int)(255f * projectile.localAI[0] / maxTime);
            }

            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}