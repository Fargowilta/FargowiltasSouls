using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class NatureBullet : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/SniperBullet";

        public int stopped;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Bullet");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ExplosiveBullet);
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 7;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.localAI[1] = Projectile.velocity.Length();
                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
            }

            Projectile.hide = false;

            if (--Projectile.ai[0] < 0 && Projectile.ai[0] > -40 * Projectile.MaxUpdates)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.hide = true;

                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, 229, Scale: 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                }
            }
            else if (Projectile.ai[0] == -40 * Projectile.MaxUpdates)
            {
                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                    Projectile.velocity = Projectile.DirectionTo(Main.player[p].Center) * Projectile.localAI[1];
                else
                    Projectile.Kill();
            }
            else if (Projectile.ai[0] < -40 * Projectile.MaxUpdates)
            {
                Projectile.tileCollide = true;
                Projectile.ignoreWater = false;
            }

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 68, 0f, 0f, 0, default(Color), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int index = 0; index < 6; ++index)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.Next(30, 60) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -Projectile.velocity.Y * Main.rand.Next(30, 60) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, SpeedX, SpeedY,
                        ModContent.ProjectileType<SniperBulletShard>(), 0, 0f, Projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}