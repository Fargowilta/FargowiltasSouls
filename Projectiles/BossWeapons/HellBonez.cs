using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HellBonez : Bonez
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hell Bonez");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.scale = 2f;
            projectile.timeLeft = 120;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += 0.3f * Math.Sign(projectile.velocity.X);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<Buffs.Souls.HellFire>(), 300);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width/2, projectile.height/2, 190, projectile.velocity.X * 0.75f, projectile.velocity.Y * 0.75f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
            }
        }
    }
}