using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FakeHeart2 : FakeHeart
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(Projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);

            Projectile.ai[0]--;
            if (Projectile.ai[0] > 0)
            {
                Projectile.rotation = -Projectile.velocity.ToRotation();
            }
            else if (Projectile.ai[0] == 0)
                Projectile.velocity = Vector2.Zero;
            else
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                {
                    Player target = FargoSoulsUtil.PlayerExists(Player.FindClosest(Projectile.Center, 0, 0));
                    if (target != null)
                    {
                        Projectile.velocity = Projectile.DirectionTo(target.Center) * 20;
                        Projectile.netUpdate = true;
                    }
                }
                if (Projectile.ai[1] <= 0)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }

            Projectile.rotation -= (float)Math.PI / 2;
        }
    }
}