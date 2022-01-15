using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TimberTreeAcorn : Acorn
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/Acorn";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Acorn");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 90;
            projectile.tileCollide = false;

            projectile.extraUpdates = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(projectile.Center - projectile.velocity - Vector2.UnitY * 160, Vector2.Zero, 
                    ModContent.ProjectileType<TimberTree>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0]);
        }
    }
}