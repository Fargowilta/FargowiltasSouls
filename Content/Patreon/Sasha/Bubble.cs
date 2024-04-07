using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Sasha
{
    public class Bubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bubble);
            AIType = ProjectileID.Bubble;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
            Projectile.extraUpdates = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 7;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 speed = 5f * Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed.RotatedBy(2 * Math.PI / 4 * i), ProjectileID.WaterStream, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}