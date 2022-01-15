using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class VoidArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Void Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 24;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 600;
            aiType = ProjectileID.Bullet;
        }

        // The first 0f is the x-axis direction the projectile will go
        // The second 0f is the y-axis direction the projectile will go
        // I have it set up so that the portal will go in no direction after spawning
        // 75 is how much damage it will do
        // 5 is the amount of knockback it will do
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 109);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("VoidPortal"), 250, 5, projectile.owner);
        }

        // These are particle effects; 62 is bright purple, 27 is dark purple, 69 is blue and 71 is a lighter purple
        // the 2.0f at the ends are the sizes of the dust particles. The higher it is (like 3.0f) the bigger the dust
        public override void AI()
        {
            projectile.velocity.Y += projectile.ai[0];
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 62, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), .75f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 27, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), .75f);
            Main.dust[dustId3].noGravity = true;
            int dustId4 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 71, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), .75f);
            Main.dust[dustId4].noGravity = true;
            int dustId5 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 69, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), .75f);
            Main.dust[dustId5].noGravity = true;
        }
    }
}