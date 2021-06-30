using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantTrueEyeL : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_650";

        private float localAI0;
        private float localAI1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Eye of Mutant");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 42;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            cooldownSlot = 1;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToGuttedHeart = true;

            projectile.hide = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCheck = projectile => { return false; };
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player target = Main.player[(int)projectile.ai[0]];
            projectile.localAI[0]++;
            switch ((int)projectile.ai[1])
            {
                case 0: //true eye movement code
                    Vector2 newVel = target.Center - projectile.Center + new Vector2(0f, -300f);
                    if (newVel != Vector2.Zero)
                    {
                        newVel.Normalize();
                        newVel *= 24f;
                        projectile.velocity.X = (projectile.velocity.X * 29 + newVel.X) / 30;
                        projectile.velocity.Y = (projectile.velocity.Y * 29 + newVel.Y) / 30;
                    }
                    if (projectile.Distance(target.Center) < 150f)
                    {
                        if (projectile.Center.X < target.Center.X)
                            projectile.velocity.X -= 0.25f;
                        else
                            projectile.velocity.X += 0.25f;

                        if (projectile.Center.Y < target.Center.Y)
                            projectile.velocity.Y -= 0.25f;
                        else
                            projectile.velocity.Y += 0.25f;
                    }

                    if (projectile.localAI[0] > 120f)
                    {
                        projectile.localAI[0] = 0f;
                        projectile.ai[1]++;
                        projectile.netUpdate = true;
                    }
                    break;

                case 1: //slow down
                    projectile.velocity *= 0.95f;
                    if (projectile.velocity.Length() < 1f) //stop
                    {
                        projectile.velocity = Vector2.Zero;
                        projectile.localAI[0] = 0f;
                        projectile.ai[1]++;
                        projectile.netUpdate = true;
                    }
                    break;

                case 2: //firing laser
                    if (projectile.localAI[0] == 1f)
                    {
                        const float PI = (float)Math.PI;
                        float rotationDirection = PI * 2f / 3f / 90f; //positive is CW, negative is CCW
                        if (projectile.Center.X < target.Center.X)
                            rotationDirection *= -1;
                        localAI0 -= rotationDirection * 60f;
                        Vector2 speed = -Vector2.UnitX.RotatedBy(localAI0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(projectile.Center - Vector2.UnitY * 6f, speed, mod.ProjectileType("MutantTrueEyeDeathray"),
                                projectile.damage, 0f, projectile.owner, rotationDirection, projectile.whoAmI);
                        projectile.localAI[1] = rotationDirection;
                        projectile.netUpdate = true;
                    }
                    else if (projectile.localAI[0] > 90f)
                    {
                        projectile.localAI[0] = 0f;
                        projectile.ai[1]++;
                    }
                    else
                    {
                        localAI0 += projectile.localAI[1];
                    }
                    break;

                default:
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 8f;
                    }
                    Main.PlaySound(SoundID.Zombie, (int)projectile.Center.X, (int)projectile.Center.Y, 102, 1f, 0.0f);
                    projectile.Kill();
                    break;
            }

            if (projectile.rotation > 3.14159274101257)
                projectile.rotation = projectile.rotation - 6.283185f;
            projectile.rotation = projectile.rotation <= -0.005 || projectile.rotation >= 0.005 ? projectile.rotation * 0.96f : 0.0f;
            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
            if (projectile.ai[1] != 2f) //custom pupil when attacking
                UpdatePupil();
        }

        private void UpdatePupil()
        {
            float f1 = (float)(localAI0 % 6.28318548202515 - 3.14159274101257);
            float num13 = (float)Math.IEEERemainder(localAI1, 1.0);
            if (num13 < 0.0)
                ++num13;
            float num14 = (float)Math.Floor(localAI1);
            float max = 0.999f;
            int num15 = 0;
            float amount = 0.1f;
            float f2;
            float num18;
            float num19;
            f2 = projectile.AngleTo(Main.player[(int)projectile.ai[0]].Center);
            num15 = 2;
            num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
            num19 = num14 + Math.Sign(-12f - num14);
            Vector2 rotationVector2 = f2.ToRotationVector2();
            localAI0 = (float)(Vector2.Lerp(f1.ToRotationVector2(), rotationVector2, amount).ToRotation() + num15 * 6.28318548202515 + 3.14159274101257);
            localAI1 = num19 + num18;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 360);
            if (FargoSoulsWorld.MasochistMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = projectile.GetAlpha(projectile.hide && Main.netMode == NetmodeID.MultiplayerClient ? Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16) : lightColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 1f;
            scale *= projectile.scale;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            Texture2D pupil = mod.GetTexture("Projectiles/Minions/TrueEyePupil");
            Vector2 pupilOffset = new Vector2(localAI1 / 2f, 0f).RotatedBy(localAI0);
            pupilOffset += new Vector2(0f, -6f).RotatedBy(projectile.rotation);
            Vector2 pupilOrigin = pupil.Size() / 2f;
            Main.spriteBatch.Draw(pupil, pupilOffset + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(pupil.Bounds), color26, 0f, pupilOrigin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}