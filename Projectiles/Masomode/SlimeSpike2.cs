using Terraria;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class SlimeSpike2 : SlimeSpike
    {
        public override string Texture => "Terraria/Projectile_605";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.scale = 1.5f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                Lighting.AddLight(projectile.Center, 0f, 0f, 0.8f);
        }
    }
}