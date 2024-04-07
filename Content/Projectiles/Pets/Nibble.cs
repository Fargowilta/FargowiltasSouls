using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Pets
{
    public class Nibble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.CursedSapling);
            AIType = ProjectileID.CursedSapling;
            Projectile.width = 24;
            Projectile.height = 40;
            Projectile.scale = 0.8f;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].cSapling = false; // Relic from AIType
            return true;
        }

        private float realFrameCounter;
        private int realFrame;
        private bool shivering;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (!player.active || player.dead || player.ghost)
                modPlayer.Nibble = false;

            if (modPlayer.Nibble)
                Projectile.timeLeft = 2;

            shivering = player.ZoneSnow;

            const int firstFlyFrame = 6;

            Projectile.rotation = 0;

            if (Projectile.tileCollide) //walking
            {
                //wavedash away if too close
                if (player.velocity.X == 0 && Math.Abs(player.Bottom.Y - Projectile.Bottom.Y) < 16 * 2
                    && Math.Abs(player.Center.X - Projectile.Center.X) < 16 * (Projectile.velocity.X == 0 ? 1 : 3))
                {
                    Projectile.velocity.X += 0.115f * (Projectile.Center.X == player.Center.X ? -player.direction : Math.Sign(Projectile.Center.X - player.Center.X));
                }

                //faster
                if (!Collision.SolidCollision(Projectile.position + Projectile.velocity.X * Vector2.UnitX, Projectile.width, Projectile.height))
                    Projectile.position.X += Projectile.velocity.X;

                //high jump
                //if (Projectile.velocity.Y < 0) Projectile.position.Y += Projectile.velocity.Y;

                //animation
                //bool relativelyStillVertically = Projectile.velocity.Y >= 0 && Projectile.velocity.Y <= 0.8f;
                //if (relativelyStillVertically)
                //{
                if (Math.Abs(Projectile.velocity.X) < 0.1f)
                {
                    realFrameCounter = 0;
                    realFrame = 0;

                    Projectile.spriteDirection = player.Center.X < Projectile.Center.X ? 1 : -1;
                }
                else
                {
                    realFrameCounter += Math.Abs(Projectile.velocity.X);

                    if (++realFrameCounter > 5)
                    {
                        realFrameCounter = 0;
                        if (++realFrame > firstFlyFrame - 1)
                            realFrame = 0;
                    }
                }
                //}
                //else
                //{
                //    realFrame = 1;
                //}

                Projectile.ai[2] = 0;
            }
            else //flying
            {
                if (Projectile.velocity != Vector2.Zero)
                {
                    const int rampupTime = 480;
                    float lerp = Math.Min(++Projectile.ai[2] / rampupTime, 1f);
                    float betterHoming = 0.03f * lerp;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(player.Center) * Projectile.velocity.Length(), betterHoming);

                    Projectile.rotation = Projectile.velocity.ToRotation();
                    if (Projectile.spriteDirection > 0)
                        Projectile.rotation += MathHelper.Pi;

                    /*for (int i = 0; i < 2; i++)
                    {
                        Vector2 dustPos = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * (8f + 4f * Main.rand.NextFloat(Projectile.velocity.Length()));
                        int index = Dust.NewDust(dustPos, 0, 0,
                          DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(), 1.5f);
                        Main.dust[index].noGravity = true;
                        //Main.dust[index].velocity = Main.dust[index].velocity * 0.3f;
                        Main.dust[index].velocity = Main.dust[index].velocity - Projectile.velocity * 0.1f;
                    }*/
                }

                if (++realFrameCounter > 3)
                {
                    realFrameCounter = 0;
                    realFrame++;
                }

                if (realFrame < firstFlyFrame || realFrame >= Main.projFrames[Projectile.type])
                    realFrame = firstFlyFrame;
            }

            //Projectile.spriteDirection = Math.Sign(Projectile.velocity.X == 0 ? player.Center.X - Projectile.Center.X : Projectile.velocity.X);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Projectile.owner].Center.Y > Projectile.Bottom.Y;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * realFrame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawoffset = 4 * Vector2.UnitY;
            if (shivering)
                drawoffset.X += Main.rand.NextFloat(-1f, 1f);

            //Color color26 = Color.Cyan * Projectile.Opacity * 0.5f;
            //color26.A = 20;

            //float scale = Projectile.scale * ((Main.mouseTextColor / 200f - 0.35f) * 0.25f + 0.9f);
            //for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.25f)
            //{
            //    Color color27 = color26 * 0.3f;
            //    float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            //    color27 *= fade * fade;
            //    int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
            //    if (max0 < 0)
            //        continue;
            //    float num165 = Projectile.oldRot[max0];
            //    Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
            //    center += Projectile.Size / 2;
            //    Main.EntitySpriteDraw(texture2D13, center + drawoffset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            //}

            //Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawoffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, effects, 0);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawoffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}