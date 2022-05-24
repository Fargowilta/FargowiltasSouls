using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.JungleMimic
{
    public class AcornProjectile : ModProjectile
    {
        public float bounce = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");

        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            bounce += 1;
            if (bounce == 4)
            {
                Projectile.Kill();
            }

            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 0.1f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 0.1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.8f;
            }

            return false;
        }
        public override void AI()
        {
            Projectile.rotation += 0.2f * (float)Projectile.direction;
            if (Projectile.velocity.Y < 0)
                Projectile.velocity.Y += 0.3f;
            else
                Projectile.velocity.Y += 0.4f;
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AcornProjectileExplosion>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, 0);
            }
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
        }

    }
}