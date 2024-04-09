using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

    public class BaronMine : ModProjectile
    {
        public bool home = true;
        public bool BeenOutside = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron's Mine");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.frame = 2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 60 * 6);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * 0f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * 0f;
            }
            return false;
        }
        private Vector2 drawOffset = Vector2.Zero;
        public bool Floating => Projectile.ai[0] == 1;
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            float modifier = Floating ? 0.4f : 1f;
            Projectile.rotation += modifier * MathHelper.ToRadians(Projectile.velocity.Length());
            /*
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frame %= 3;
                Projectile.frameCounter = 0;
            }
            Projectile.frameCounter++;
            */

            int endTime = Floating ? 140 : 120;
            if (++Projectile.localAI[1] > endTime * 0.33f && Projectile.frame > 0)
            {
                Projectile.frame--;
                Projectile.localAI[1] = 0;
            }
            int scaleupTime = 15;
            Projectile.scale = (float)Utils.Lerp(0, 1, Math.Clamp(Projectile.localAI[0] / scaleupTime, 0, 1));
            if (++Projectile.localAI[0] > endTime)
            {
                Projectile.Kill();
            }

            float length = 2.5f * Projectile.localAI[0] / endTime;
            drawOffset = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * length;

            if (!Projectile.tileCollide)
            {
                if (!Collision.SolidCollision(Projectile.position, Projectile.height, Projectile.width)) //this check is inside to stop checking once tileCollide is on
                {
                    Projectile.tileCollide = !Floating;
                }
            }
            if (Floating) //floating
            {
                if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height) || Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    if (Projectile.velocity.Y > 0)
                        Projectile.velocity.Y *= 0.94f;
                    Projectile.velocity.Y -= 0.12f;
                }
                else
                {
                    if (Projectile.velocity.Y < 0)
                        Projectile.velocity.Y /= 3;
                    Projectile.velocity.Y += 0.3f;
                }
                Projectile.velocity.X *= 0.97f;
            }
            if (Projectile.ai[0] == 2) //slowing down, flying
            {
                Projectile.velocity *= 0.97f;
            }
            //}
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int j = 0; j < 4; j++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, (Vector2.UnitX * 5).RotatedByRandom(MathHelper.TwoPi), Main.rand.Next(61, 64), 1f);
            }
            float speedmod = Projectile.ai[0] == 1 ? 1 : 1.5f;
            float offset = 24;
            for (int i = 0; i < 8; i++)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 pos = new Vector2(0, 1).RotatedBy(Projectile.rotation + i * MathHelper.TwoPi / 8);
                    Vector2 vel = pos * Main.rand.NextFloat(5, 6) * speedmod;
                    pos *= offset;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + pos, vel, ModContent.ProjectileType<BaronShrapnel>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 0);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].hostile = Projectile.hostile;
                        Main.projectile[p].friendly = Projectile.friendly;
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = lightColor * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);


            return false;
        }
    }
}

