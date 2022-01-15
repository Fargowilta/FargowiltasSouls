using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GolemSpikyBall : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_185";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spiky Ball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.SpikyBallTrap);
            aiType = ProjectileID.SpikyBallTrap;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.trap = false;
            projectile.alpha = 255;
            projectile.penetrate = 1;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 2;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                    for (int index1 = 0; index1 < 7; ++index1)
                    {
                        int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= 1.5f;
                        int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                        Main.dust[index3].velocity *= 1.5f;
                    }
                }
            }

            Tile tile = Framing.GetTileSafely(projectile.Center);
            if (tile != null && tile.wall == WallID.LihzahrdBrickUnsafe)
            {
                projectile.velocity.Y -= 0.2f * 2; //reverse gravity in lihzahrd temple
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int index1 = 0; index1 < 7; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 1.5f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X * 0.9f;
            if (projectile.velocity.Y != oldVelocity.Y && System.Math.Abs(oldVelocity.Y) > 1)
                projectile.velocity.Y = -oldVelocity.Y * (System.Math.Abs(oldVelocity.Y) > 8 ? 0.75f : 0.98f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha == 0 ? 0 : 255) * projectile.Opacity;
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

            SpriteEffects effects = SpriteEffects.None;
            
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}