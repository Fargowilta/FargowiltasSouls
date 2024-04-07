using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class ArrowRain : ModProjectile
    {
        private bool launchArrow = true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arrow Rain");
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 330;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[1]];

            if (target.active)
            {
                //follow the target
                Projectile.Center = new Vector2(target.Center.X, target.Center.Y - 1000);
            }

            //delay
            if (Projectile.timeLeft > 300)
            {
                return;
            }

            if (launchArrow)
            {
                Vector2 position = new(Projectile.Center.X + Main.rand.Next(-100, 100), Projectile.Center.Y + Main.rand.Next(-75, 75));
                Vector2 velocity;
                if (target.active)
                {
                    velocity = Vector2.Normalize(target.Center - Projectile.position) * 25;
                }
                else
                {
                    velocity = new Vector2(0, 5);
                }


                //if (direction == 1)
                //{
                //    velocity = new Vector2(Main.rand.NextFloat(0, 2), Main.rand.NextFloat(20, 25));
                //}
                //else
                //{
                //    velocity = new Vector2(Main.rand.NextFloat(-2, 0), Main.rand.NextFloat(20, 25));
                //}

                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, (int)Projectile.ai[0], Projectile.damage, 0, Projectile.owner);
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