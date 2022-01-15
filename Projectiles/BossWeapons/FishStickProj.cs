using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class FishStickProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fish Stick");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.aiStyle = 1;
            aiType = ProjectileID.JavelinFriendly;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(-45f);

            int max = 3;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector2_1 = projectile.position;
                Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                Vector2 vector2_3 = vector2_2;
                int index2 = Dust.NewDust(vector2_1 + vector2_3, projectile.width, projectile.height, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity /= 4f;
                Main.dust[index2].velocity -= projectile.velocity;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width /= 2;
            height /= 2;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        private void ShootSharks(Vector2 target, int rngModifier, Vector2 leadLength = default)
        {
            Main.projectile.Where(x => x.active && x.type == ModContent.ProjectileType<Whirlpool>() && x.owner == projectile.owner).ToList().ForEach(x =>
            {
                if (Main.rand.NextBool(rngModifier))
                {
                    Vector2 velocity = Vector2.Normalize(target + leadLength * Main.rand.NextFloat() - x.Center) * Main.rand.NextFloat(12f, 24f);
                    Projectile.NewProjectile(x.Center, velocity, ModContent.ProjectileType<FishStickShark>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            });
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            
            if (projectile.owner == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WhirlpoolBase>()] < 1)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WhirlpoolBase>(), projectile.damage, projectile.knockBack, projectile.owner, 16, 11);
                else
                    ShootSharks(target.Center, 2, target.velocity * 30f);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
                ShootSharks(projectile.Center, 5);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 59, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 59, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotationModifier = projectile.spriteDirection > 0 ? MathHelper.PiOver2 : MathHelper.Pi;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i] + rotationModifier;
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation + rotationModifier, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}