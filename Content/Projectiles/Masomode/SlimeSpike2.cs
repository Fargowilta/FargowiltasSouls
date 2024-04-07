using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SlimeSpike2 : SlimeSpike
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/MutantBoss/MutantSlimeBall_3";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 14;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.rotation += MathHelper.Pi;
            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Lighting.AddLight(Projectile.Center, 0f, 0f, 0.8f);
        }
    }
}