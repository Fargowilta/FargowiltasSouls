using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Phupperbat
{
    public class ChibiiRemii : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chibii Remii");
            Main.projFrames[Projectile.type] = 11;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BlackCat);
            AIType = ProjectileID.BlackCat;
            Projectile.width = 20;
            Projectile.height = 40;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].blackCat = false; // Relic from AIType
            return true;
        }

        private int sitTimer;
        private float realFrameCounter;
        private int realFrame;

        private bool squeak;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();

            if (!player.active || player.dead || player.ghost)
                modPlayer.ChibiiRemii = false;

            if (modPlayer.ChibiiRemii)
                Projectile.timeLeft = 2;

            const int sitTime = 600;
            const int sitFrame = 6;
            const int firstFlyFrame = 7;

            if (Projectile.tileCollide) //walking
            {
                //wavedash away if too close but not if sitting
                if (player.velocity.X == 0 && Math.Abs(player.Bottom.Y - Projectile.Bottom.Y) < 16 * 2
                    && Math.Abs(player.Center.X - Projectile.Center.X) < 16 * (Projectile.velocity.X == 0 ? 1 : 3)
                    && sitTimer < sitTime)
                {
                    Projectile.velocity.X += 0.1f * (Projectile.Center.X == player.Center.X ? -player.direction : Math.Sign(Projectile.Center.X - player.Center.X));
                }

                //faster
                if (!Collision.SolidCollision(Projectile.position + Projectile.velocity.X * Vector2.UnitX, Projectile.width, Projectile.height))
                    Projectile.position.X += Projectile.velocity.X;

                //high jump
                //if (Projectile.velocity.Y < 0) Projectile.position.Y += Projectile.velocity.Y;

                //animation
                bool relativelyStillVertically = Projectile.velocity.Y >= 0 && Projectile.velocity.Y <= 0.8f;
                if (relativelyStillVertically)
                {
                    if (Math.Abs(Projectile.velocity.X) < 1f)
                    {
                        realFrameCounter = 0;
                        realFrame = 1;
                    }
                    else
                    {
                        realFrameCounter += Math.Abs(Projectile.velocity.X);

                        if (++realFrameCounter > 8)
                        {
                            realFrameCounter = 0;
                            realFrame += 1;
                        }
                    }

                    if (realFrame >= sitFrame)
                        realFrame = 0;
                }
                else
                {
                    realFrame = 1;
                }

                if (Projectile.velocity.X == 0)
                {
                    realFrame = relativelyStillVertically ? 0 : 1;
                    realFrameCounter = 0;

                    if (!relativelyStillVertically)
                        sitTimer = 0;

                    if (sitTimer >= 600)
                    {
                        realFrame = sitFrame;

                        if (sitTimer == 600 && squeak)
                        {
                            squeak = false;

                            if (!Main.dedServ)
                                SoundEngine.PlaySound(new SoundStyle($"FargowiltasSouls/Assets/Sounds/SqueakyToy/squeak{Main.rand.Next(1, 7)}"), Projectile.Center);
                        }
                    }
                    else
                    {
                        sitTimer += 1;

                        //face player when standing idle
                        Projectile.direction = Math.Sign(player.Center.X - Projectile.Center.X);

                        //dont squeak if player moves at all
                        if (player.velocity.X != 0 || Math.Abs(player.velocity.Y) > 0.5f)
                            squeak = false;
                    }
                }
                else
                {
                    sitTimer = 0;
                    squeak = true;
                }

                if (realFrame > sitFrame)
                    realFrame = 0;
            }
            else //flying
            {
                Projectile.Center += Projectile.velocity;

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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.vampireHeal(1, Projectile.Center, target);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * realFrame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = Color.Red * Projectile.Opacity;
            color26.A = 20;

            Vector2 drawoffset = -2 * Vector2.UnitY;

            float scale = Projectile.scale * (Main.mouseTextColor / 200f - 0.35f) * 0.45f + 0.8f;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.25f)
            {
                Color color27 = color26 * 0.3f;
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center + drawoffset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawoffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, effects, 0);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawoffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}