using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeProjSmall : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Shot");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.5f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
            {
                Rectangle trailHitbox = projHitbox;
                Vector2 diff = Projectile.oldPos[i] - Projectile.Center;
                trailHitbox.X += (int)diff.X;
                trailHitbox.Y += (int)diff.Y;
                if (trailHitbox.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void AI()
        {
            //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 0.25f);
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;

            //if (Timer > 120 && Timer < 240)
            //    Projectile.velocity *= 1.015f;

            if (++Projectile.ai[0] > 600f)
            {
                Projectile.Kill();
            }

            //flag to be accelerating rain 
            //commented out because rain is unused
            /*
            if (Projectile.ai[1] == -1)
            {
                if (Timer > 120)
                    Projectile.velocity *= 1.04f;
                if (Timer > 240)
                    Projectile.Kill();
            }
            else //i.e. rain does not do this
            {
            */
                //a bit after spawning, become tangible when it finds an open space
                if (!Projectile.tileCollide && Projectile.ai[0] > 60 * Projectile.MaxUpdates)
                {
                    Tile tile = Framing.GetTileSafely(Projectile.Center);
                    if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
                        Projectile.tileCollide = true;
                }
            //}
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
            }
        }
        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.Pink * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }*/
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity * 0.9f;
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
                Color color27 = color26 * 0.75f;
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
