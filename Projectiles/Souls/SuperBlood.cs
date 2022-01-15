using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class SuperBlood : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Super Blood");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 4; 
            projectile.aiStyle = 2;
            projectile.timeLeft = 300;
            aiType = 48;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 5, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 5, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100);
            Main.dust[dustId3].noGravity = true;
        }
    }
}