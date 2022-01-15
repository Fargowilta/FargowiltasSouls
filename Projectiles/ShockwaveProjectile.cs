using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class ShockwaveProjectile : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Empty";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Shockwave");
        }
        
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.hide = true;
        }

        public override void AI()
        {
            int rippleCount = 5;
            int rippleSize = 6;
            int rippleSpeed = 15;
            float distortStrength = 200f;

            int maxTime = 120;
            float modifier = 3f;

            if (++projectile.ai[0] > maxTime)
            {
                projectile.Kill();
                return;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                int progress = (int)(projectile.ai[0] / maxTime * modifier);

                if (projectile.ai[0] == 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == projectile.type)
                            Main.projectile[i].Kill();
                    }

                    if (Filters.Scene["Shockwave"].IsActive())
                        Filters.Scene["Shockwave"].GetShader().UseTargetPosition(projectile.Center);
                    else
                        Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(projectile.Center);
                }

                if (Filters.Scene["Shockwave"].IsActive())
                    Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / modifier));
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (Filters.Scene["Shockwave"].IsActive())
                    Filters.Scene["Shockwave"].Deactivate();
            }
        }
    }
}