using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantBoss : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/MutantBoss/MutantBoss";

        public string trailTexture => "NPCs/Eternals/MutantSoul";
        public int npcType => mod.NPCType("MutantBoss");
        public bool auraTrail;

        const int auraFrames = 19;
        //const int lightningFrames = 20;

        public bool sansEye;
        public float SHADOWMUTANTREAL;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mutant");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 50;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            if (projectile.hide)
                drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], npcType);
            if (npc != null)
            {
                projectile.Center = npc.Center;
                projectile.alpha = npc.alpha;
                projectile.direction = projectile.spriteDirection = npc.direction;
                projectile.timeLeft = 30;
                auraTrail = npc.localAI[3] >= 3;

                /*switch((int)npc.ai[0]) //draw behind whenever holding a weapon
                {
                    case 0:
                    case 4:
                    case 5:
                    case 6:
                    case 13:
                    case 14:
                    case 15:
                    case 21:
                    case 22:
                    case 23:
                    case 25:
                    case 36:
                    case 41:
                        projectile.hide = true;
                        break;

                    default:
                        projectile.hide = false;
                        break;
                }*/

                projectile.hide =
                    Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSpearAim>()] > 0
                    || Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSpearDash>()] > 0
                    || Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSpearSpin>()] > 0
                    || Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSlimeRain>()] > 0;

                sansEye = (npc.ai[0] == 10 && npc.ai[1] > 150) || (npc.ai[0] == -5 && npc.ai[2] > 420 - 90 && npc.ai[2] < 420);
                if (npc.ai[0] == 10 && FargoSoulsWorld.EternityMode)
                {
                    SHADOWMUTANTREAL += 0.03f;
                    if (SHADOWMUTANTREAL > 0.75f)
                        SHADOWMUTANTREAL = 0.75f;
                }
                projectile.localAI[1] = sansEye ? MathHelper.Lerp(projectile.localAI[1], 1f, 0.05f) : 0; //for rotation of sans eye
                projectile.ai[0] = sansEye ? projectile.ai[0] + 1 : 0;

                if (!Main.dedServ)
                    projectile.frame = (int)(npc.frame.Y / (float)(Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]));

                if (npc.frameCounter == 0)
                {
                    if (++projectile.localAI[0] >= auraFrames)
                        projectile.localAI[0] = 0;
                }
            }
            else
            {
                sansEye = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    projectile.Kill();
                return;
            }

            SHADOWMUTANTREAL -= 0.01f;
            if (SHADOWMUTANTREAL < 0)
                SHADOWMUTANTREAL = 0;
        }

        public override void Kill(int timeLeft)
        {
            /*Main.NewText("i die now");
            if (Main.netMode == NetmodeID.Server)
                NetMessage.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("i die now aaaaaa"), Color.LimeGreen);*/
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Texture2D texture2D14 = mod.GetTexture(trailTexture);
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Texture2D aura = mod.GetTexture("NPCs/MutantBoss/MutantAura");
            int auraFrameHeight = aura.Height / auraFrames;
            int auraY = auraFrameHeight * (int)projectile.localAI[0];
            Rectangle auraRectangle = new Rectangle(0, auraY, aura.Width, auraFrameHeight);

            /*Texture2D lightning = mod.GetTexture("NPCs/MutantBoss/MutantLightning");
            int lightningFrameHeight = lightning.Height / lightningFrames;
            int lightningY = lightningFrameHeight * (int)projectile.localAI[0];
            Rectangle lightningRectangle = new Rectangle(0, lightningY, lightning.Width, lightningFrameHeight);*/

            Color color26 = projectile.GetAlpha(projectile.hide && Main.netMode == NetmodeID.MultiplayerClient ? Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16) : lightColor);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 0.9f;
            scale *= projectile.scale;

            if (auraTrail || SHADOWMUTANTREAL > 0)
                Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, scale, effects, 0f);

            if (auraTrail)
            {
                for (float i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.5f)
                {
                    Color color27 = Color.White * projectile.Opacity * 0.75f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                    if (max0 < 0)
                        continue;
                    Vector2 value4 = projectile.oldPos[max0];
                    float num165 = projectile.oldRot[max0];
                    Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                    center += projectile.Size / 2;
                    Main.spriteBatch.Draw(texture2D14, center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
                }

                Main.spriteBatch.Draw(aura, -16 * Vector2.UnitY + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(auraRectangle), Color.White * projectile.Opacity, projectile.rotation, auraRectangle.Size() / 2f, scale, effects, 0f);
            }
            else
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
                }
            }

            color26 = Color.Lerp(color26, Color.Black, SHADOWMUTANTREAL);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);

            if (sansEye)
            {
                Color color = new Color(51, 255, 191);

                const int maxTime = 120;
                float effectiveTime = projectile.ai[0];
                float rotation = MathHelper.TwoPi * projectile.localAI[1];
                float modifier = Math.Min(1f, (float)Math.Sin(Math.PI * effectiveTime / maxTime) * 2f);
                float opacity = Math.Min(1f, modifier * 2f);
                float sansScale = projectile.scale * modifier * Main.cursorScale * 1.25f;

                Texture2D star = mod.GetTexture("Effects/LifeStar");
                Rectangle rect = new Rectangle(0, 0, star.Width, star.Height);
                Vector2 origin = new Vector2((star.Width / 2) + sansScale, (star.Height / 2) + sansScale);

                Vector2 drawPos = projectile.Center;
                drawPos.X += 8 * projectile.spriteDirection;
                drawPos.Y -= 11;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

                spriteBatch.Draw(star, drawPos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rect), color * opacity, rotation, origin, sansScale, SpriteEffects.None, 0);
                spriteBatch.Draw(star, drawPos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rect), Color.White * opacity * 0.75f, rotation, origin, sansScale, SpriteEffects.None, 0);
                /*DrawData starDraw = new DrawData(star, drawPos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rect), Color.White * opacity, rotation, origin, sansScale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.LimeGreen * opacity).UseSecondaryColor(color * opacity);
                GameShaders.Misc["LCWingShader"].Apply(starDraw);
                starDraw.Draw(spriteBatch);*/

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            //if (auraTrail) Main.spriteBatch.Draw(lightning, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(lightningRectangle), Color.White * projectile.Opacity, projectile.rotation, lightningRectangle.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}