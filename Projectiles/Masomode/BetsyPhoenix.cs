using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyPhoenix : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_706";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Phoenix");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.DD2PhoenixBowShot];
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (--Projectile.ai[1] < 0 && Projectile.ai[1] > -300)
            {
                Player p = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (p != null)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 18f;

                    //if (Projectile.localAI[0] == 0) Projectile.localAI[0] = Projectile.Center.X < p.Center.X ? 1 : -1;

                    Vector2 target = p.Center;
                    //target.X += Projectile.localAI[0] * 200;

                    if (Projectile.Distance(target) > 200)
                    {
                        Vector2 distance = target - Projectile.Center;

                        double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                        if (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        if (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.2);
                    }
                    else
                    {
                        Projectile.ai[1] = -300;
                    }
                }
                else
                {
                    Projectile.ai[0] = Player.FindClosest(Projectile.Center, 0, 0);
                }
            }

            if (Projectile.alpha <= 0) //vanilla display code
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center - Projectile.Size / 4f, Projectile.width / 2, Projectile.height / 2,
                            Utils.SelectRandom(Main.rand, new int[3] { 6, 31, 31 }), 0.0f, 0.0f, 0, default, 1f);
                        dust.noGravity = true;
                        dust.velocity *= 2.3f;
                        dust.fadeIn = 1.5f;
                        dust.noLight = true;
                    }
                }
                Vector2 vector2_1 = 16f * new Vector2(0f, (float)Math.Cos(Projectile.frameCounter * 6.28318548202515 / 40.0 - 1.57079637050629)).RotatedBy(Projectile.rotation);
                Vector2 vector2_2 = Vector2.Normalize(Projectile.velocity);

                Dust dust1 = Dust.NewDustDirect(Projectile.Center - Projectile.Size / 4f, Projectile.width / 2, Projectile.height / 2, 6, 0.0f, 0.0f, 0, default, 1f);
                dust1.noGravity = true;
                dust1.position = Projectile.Center + vector2_1;
                dust1.velocity = Vector2.Zero;
                dust1.fadeIn = 1.4f;
                dust1.scale = 1.15f;
                dust1.noLight = true;
                dust1.position += Projectile.velocity * 1.2f;
                dust1.velocity += vector2_2 * 2f;

                Dust dust2 = Dust.NewDustDirect(Projectile.Center - Projectile.Size / 4f, Projectile.width / 2, Projectile.height / 2, 6, 0.0f, 0.0f, 0, default, 1f);
                dust2.noGravity = true;
                dust2.position = Projectile.Center + vector2_1;
                dust2.velocity = Vector2.Zero;
                dust2.fadeIn = 1.4f;
                dust2.scale = 1.15f;
                dust2.noLight = true;
                dust2.position += Projectile.velocity * 0.5f;
                dust2.position += Projectile.velocity * 1.2f;
                dust2.velocity += vector2_2 * 2f;
            }
            int num9 = Projectile.frameCounter + 1;
            Projectile.frameCounter = num9;
            if (num9 >= 40)
                Projectile.frameCounter = 0;
            Projectile.frame = Projectile.frameCounter / 5;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha = Projectile.alpha - 55;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                    float num1 = 16f;
                    for (int index1 = 0; index1 < num1; ++index1)
                    {
                        Vector2 vector2 = -Vector2.UnitY.RotatedBy(index1 * (6.28318548202515 / num1)) * new Vector2(1f, 4f);
                        vector2 = vector2.RotatedBy(Projectile.velocity.ToRotation());
                        int index2 = Dust.NewDust(Projectile.Center, 0, 0, 6, 0.0f, 0.0f, 0, default, 1f);
                        Main.dust[index2].scale = 1.5f;
                        Main.dust[index2].noLight = true;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = Projectile.Center + vector2;
                        Main.dust[index2].velocity = Main.dust[index2].velocity * 4f + Projectile.velocity * 0.3f;
                    }
                }
            }
            DelegateMethods.v3_1 = new Vector3(1f, 0.6f, 0.2f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 4f, 40f, DelegateMethods.CastLightOpen);

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.direction < 0)
                Projectile.rotation += (float)Math.PI;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, 300);
            target.AddBuff(BuffID.WitheredWeapon, 300);
            target.AddBuff(BuffID.Burning, 300);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}