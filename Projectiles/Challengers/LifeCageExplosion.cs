using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeCageExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cage Explosion");
        }
        public override void SetDefaults()
        {
            Projectile.width = 184;
            Projectile.height = 184;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 120;
            Projectile.light = 2f;
        }

        public override void AI()
        {

            if (Projectile.ai[0] > 1f)
            {
                Projectile.Kill();
            }
            for (int i = 0; i < 150; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 219, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 0.25f);
            Projectile.ai[0] += 1f;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
    }
}
