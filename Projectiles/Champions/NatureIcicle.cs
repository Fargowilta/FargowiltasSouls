using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class NatureIcicle : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Souls/FrostIcicle";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Nature Icicle");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 600;

            projectile.scale = 1.5f;
            projectile.hide = true;
            cooldownSlot = 1;
            projectile.tileCollide = false;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                projectile.rotation = Main.rand.NextFloat(0, (float)Math.PI * 2);
                projectile.hide = false;
            }
            
            if (--projectile.ai[0] > 0)
            {
                projectile.tileCollide = false;
                projectile.rotation += projectile.velocity.Length() * .1f * projectile.localAI[0];
            }
            else if (projectile.ai[0] == 0)
            {
                int p = Player.FindClosest(projectile.Center, 0, 0);
                if (p != -1)
                {
                    projectile.velocity = projectile.DirectionTo(Main.player[p].Center) * 30;
                    projectile.netUpdate = true;

                    if (projectile.ai[1] > 0)
                    {
                        float rotation = MathHelper.ToRadians(20) + Main.rand.NextFloat(MathHelper.ToRadians(30));
                        if (Main.rand.NextBool())
                            rotation *= -1;
                        projectile.velocity = projectile.velocity.RotatedBy(rotation);
                    }

                    Main.PlaySound(SoundID.Item1, projectile.Center);
                }
            }
            else
            {
                if (!projectile.tileCollide && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    projectile.tileCollide = true;

                if (projectile.velocity != Vector2.Zero)
                    projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.Center);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 92, 0.0f, 0.0f, 0, new Color(), 1f);
                if (Main.rand.Next(3) != 0)
                {
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity = dust1.velocity * 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust2 = Main.dust[index2];
                    dust2.scale = dust2.scale * 1.75f;
                }
                else
                {
                    Dust dust = Main.dust[index2];
                    dust.scale = dust.scale * 0.5f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 300);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}