using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class ArrowRain : ModProjectile
    {
        private bool launchArrow = true;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Arrow Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 250;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];

            //follow the cursor and double fire rate with red riding
            if (owner.GetModPlayer<FargoPlayer>().RedEnchant)
            {
                projectile.Center = new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y - 400);
                //launchArrow = true;
            }

            //delay
            if (projectile.timeLeft > 220)
            {
                return;
            }

            if (launchArrow)
            {
                Vector2 position = new Vector2(projectile.Center.X + Main.rand.Next(-100, 100), projectile.Center.Y + Main.rand.Next(-75, 75));

                float direction = projectile.ai[1];
                Vector2 velocity;

                if (direction == 1)
                {
                    velocity = new Vector2(Main.rand.NextFloat(0, 2), Main.rand.NextFloat(20, 25));
                }
                else
                {
                    velocity = new Vector2(Main.rand.NextFloat(-2, 0), Main.rand.NextFloat(20, 25));
                }

                int p = Projectile.NewProjectile(position, velocity, (int)projectile.ai[0], projectile.damage, 0, projectile.owner);
                Main.projectile[p].noDropItem = true;

                launchArrow = false;
            }
            else
            {
                launchArrow = true;
            }
        }
    }
}