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
        int FrontProj = -1;
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
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Main.projectile[FrontProj].Center, Projectile.width, ref collisionPoint))
            {
                return true;
            }
                /*for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
                {
                    Rectangle trailHitbox = projHitbox;
                    Vector2 diff = Projectile.oldPos[i] - Projectile.Center;
                    trailHitbox.X += (int)diff.X;
                    trailHitbox.Y += (int)diff.Y;
                    if (trailHitbox.Intersects(targetHitbox))
                        return true;
                }*/
                return false;
        }

        public float Timer = 0;
        public override void AI()
        {
            //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 0.25f);
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;

            //if (Timer > 120 && Timer < 240)
            //    Projectile.velocity *= 1.015f;

            if (++Timer > 600f)
            {
                Projectile.Kill();
            }

            //flag to be accelerating rain
            if (Projectile.ai[1] == -1)
            {
                if (Timer > 120)
                    Projectile.velocity *= 1.04f;
                if (Timer > 240)
                    Projectile.Kill();
            }
            else //i.e. rain does not do this
            {
                //a bit after spawning, become tangible when it finds an open space
                if (!Projectile.tileCollide && Timer > 60 * Projectile.MaxUpdates)
                {
                    Tile tile = Framing.GetTileSafely(Projectile.Center);
                    if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
                        Projectile.tileCollide = true;
                }
            }

            //find front proj
            if (FrontProj == -1)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == Projectile.type && p.ai[0] == Projectile.ai[0] - 1)
                        FrontProj = p.whoAmI;
                }
                CombatText.NewText(Projectile.Hitbox, CombatText.HealLife, FrontProj);
                Main.NewText(Projectile.ai[0]);
                FrontProj = (FrontProj == -1) ? Projectile.whoAmI : FrontProj; //if still -1, assign to itself

            }
            else if (!Main.projectile[FrontProj].active)
            {
                FrontProj = Projectile.whoAmI;
            }
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
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Pink * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            int drawLayers = 1;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D GlowlineTexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/GlowLine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = GlowlineTexture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, GlowlineTexture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            int length = (int)Projectile.Distance(Main.projectile[FrontProj].Center);
            Vector2 offset = Projectile.rotation.ToRotationVector2() * length / 2f;
            Vector2 position = Projectile.Center - Main.screenLastPosition + new Vector2(0f, Projectile.gfxOffY) + offset;
            const float resolutionCompensation = 128f / 24f; //i made the image higher res, this compensates to keep original display size
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, length, (int)(rectangle.Height * Projectile.scale / resolutionCompensation));

            Color drawColor = Projectile.GetAlpha(lightColor);
            float DrawRotation = Projectile.DirectionTo(Main.projectile[FrontProj].Center).ToRotation();
            if (FrontProj != Projectile.whoAmI)
                for (int j = 0; j < drawLayers; j++)
                    Main.EntitySpriteDraw(new DrawData(GlowlineTexture, destination, new Rectangle?(rectangle), drawColor, DrawRotation, origin2, SpriteEffects.None, 0));

            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
        /*public override bool PreDraw(ref Color lightColor)
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
        }*/
    }
}
