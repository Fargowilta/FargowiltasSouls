using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class GolemGib9 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Golem Gibs");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 16;
            projectile.aiStyle = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
        }

        /*public override void AI()
        {
            //projectile.rotation += 0.3f; //

            //projectile.velocity.Y += Math.Abs(projectile.velocity.Y * .05f) + .02f; meme
        }*/

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("GibExplosion"),
                    projectile.damage, projectile.knockBack * 2f, projectile.owner);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item84, projectile.Center);

            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("GibExplosion"),
                    projectile.damage, projectile.knockBack * 2f, projectile.owner);
        }
    }
}