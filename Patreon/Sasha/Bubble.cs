using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Sasha
{
    public class Bubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Bubble);
            aiType = ProjectileID.Bubble;

            projectile.penetrate = -1;
            projectile.scale = 2f;
            projectile.extraUpdates = 1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Vector2 speed = 5f * Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center, speed.RotatedBy(2 * Math.PI / 4 * i), ModContent.ProjectileType<WaterStream>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
        }
    }
}