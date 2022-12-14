using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;

            Projectile.hide = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item41, Projectile.Center);

                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.HallowedTorch, 0, 0, 0, default, 4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 9;
                }

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.HallowedTorch, 0, 0, 0, default, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity += Main.rand.NextFloat(32f) * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 4 * i);
                    }
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
    }
}
