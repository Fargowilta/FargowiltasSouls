using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class GolemGib : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/GolemGib1";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Golem Gibs");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 42;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            if (projectile.ai[1] == 2)
            {
                projectile.width = 34;
                projectile.height = 36;
            }
            else if (projectile.ai[1] == 3)
            {
                projectile.width = 24;
                projectile.height = 36;
            }
            else if (projectile.ai[1] == 4)
            {
                projectile.width = 32;
                projectile.height = 28;
            }
            else if (projectile.ai[1] == 5)
            {
                projectile.width = 36;
                projectile.height = 38;
            }
            else if (projectile.ai[1] == 6)
            {
                projectile.width = 52;
                projectile.height = 54;
            }
            else if (projectile.ai[1] == 7)
            {
                projectile.width = 40;
                projectile.height = 26;
            }
            else if (projectile.ai[1] == 8)
            {
                projectile.width = 62;
                projectile.height = 42;
            }
            else if (projectile.ai[1] == 9)
            {
                projectile.width = 14;
                projectile.height = 16;
            }
            else if (projectile.ai[1] == 10)
            {
                projectile.width = 34;
                projectile.height = 32;
            }
            else if (projectile.ai[1] == 11)
            {
                projectile.width = 18;
                projectile.height = 12;
            }
            else
            {
                projectile.width = 30;
                projectile.height = 42;
            }
            return true;
        }


        public override void AI()
        {
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.03f * (float)projectile.direction;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 20f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.4f;
                projectile.velocity.X = projectile.velocity.X * 0.97f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GibExplosion>(),
                    projectile.damage, projectile.knockBack * 2f, projectile.owner);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item84, projectile.Center);

            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GibExplosion>(),
                    projectile.damage, projectile.knockBack * 2f, projectile.owner);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = mod.GetTexture("Projectiles/BossWeapons/" + GetType().Name + projectile.ai[1]);
            BaseDrawing.DrawTexture(spriteBatch, tex, 0, projectile, lightColor, true);

            return false;
        }
    }
}