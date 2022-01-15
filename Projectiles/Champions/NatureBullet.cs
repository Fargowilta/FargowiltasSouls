using Microsoft.Xna.Framework;
using System;
using Terraria;
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
            //DisplayName.SetDefault("Nature Bullet");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.ExplosiveBullet);
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.ranged = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 7;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.localAI[1] = projectile.velocity.Length();
                Main.PlaySound(SoundID.Item11, projectile.Center);
            }

            projectile.hide = false;
            
            if (--projectile.ai[0] < 0 && projectile.ai[0] > -40 * projectile.MaxUpdates)
            {
                projectile.velocity = Vector2.Zero;
                projectile.hide = true;

                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(projectile.Center, 0, 0, 229, Scale: 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                }
            }
            else if (projectile.ai[0] == -40 * projectile.MaxUpdates)
            {
                Main.PlaySound(SoundID.Item11, projectile.Center);
                int p = Player.FindClosest(projectile.Center, 0, 0);
                if (p != -1)
                    projectile.velocity = projectile.DirectionTo(Main.player[p].Center) * projectile.localAI[1];
                else
                    projectile.Kill();
            }
            else if (projectile.ai[0] < -40 * projectile.MaxUpdates)
            {
                projectile.tileCollide = true;
                projectile.ignoreWater = false;
            }

            if (projectile.velocity != Vector2.Zero)
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, default(Color), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int index = 0; index < 6; ++index)
                {
                    float SpeedX = projectile.velocity.Length() * Main.rand.Next(-60, 61) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = projectile.velocity.Length() * Main.rand.Next(-60, 61) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, SpeedX, SpeedY,
                        ModContent.ProjectileType<CrystalBombShard>(), projectile.damage, 0f, projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}