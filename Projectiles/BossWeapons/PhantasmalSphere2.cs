using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PhantasmalSphere2 : PhantasmalSphere
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            //dust!
            /*int dustId = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height + 5, 56, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), .5f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height + 5, 56, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), .5f);
            Main.dust[dustId3].noGravity = true;*/

            Projectile.hide = false;

            if (++Projectile.localAI[0] == 20)
            {
                //Projectile.localAI[0] = 0;

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1500));
                if (n == null)
                {
                    Projectile.Kill();
                }
                else
                {
                    Projectile.velocity = Projectile.DirectionTo(n.Center + n.velocity * Main.rand.NextFloat(60)) * 32f;
                }
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Projectile.scale = 1f - Projectile.alpha / 255f;
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }
    }
}