using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Projectiles.Challengers
{
    //ai0: 1 = p2 homing missile, 2 = p2 straight missile, 3 = p1 homing torpedo
    public class BaronRocket : ModProjectile
    {
        public bool home = true;
        public bool BeenOutside = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banished Baron's Rocket");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
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
            Dust.NewDust(Projectile.Center - new Vector2(1, 1), 2, 2, DustID.Torch, -Projectile.velocity.X, -Projectile.velocity.Y, 0, default(Color), 1f);
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.Pi).ToRotation();

            if (++Projectile.localAI[0] > 600f)
            {
                Projectile.Kill();
            }
            //a bit after spawning, become tangible when it finds an open space
            if (!Projectile.tileCollide && Projectile.localAI[0] > 60 * Projectile.MaxUpdates)
            {
                Tile tile = Framing.GetTileSafely(Projectile.Center);
                if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
                    Projectile.tileCollide = true;
            }
            if (Projectile.ai[0] == 2) //accelerating
            {
                Projectile.velocity *= 1.05f;
            }
            if (Projectile.ai[0] == 3 || Projectile.ai[0] == 1) //homing
            {
                Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[1]);
                if (player != null && Projectile.localAI[0] > 10) //homing
                {
                    Vector2 vectorToIdlePosition = player.Center - Projectile.Center;
                    float speed = FargoSoulsWorld.MasochistModeReal ? 18f : 18f;
                    float inertia = 20f;
                    float deadzone = FargoSoulsWorld.MasochistModeReal ? 150f : 180f;
                    float num = vectorToIdlePosition.Length();
                    if (num > deadzone && home)
                    {
                        vectorToIdlePosition.Normalize();
                        vectorToIdlePosition *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                    }
                    if (Projectile.velocity == Vector2.Zero)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    if (num > deadzone)
                    {
                        BeenOutside = true;
                    }
                    if (num < deadzone && BeenOutside)
                    {
                        home = false;
                    }
                }
            }
            //}
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.OnFire, 600);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.Pink * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }*/
        //(public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity * 0.9f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = (Projectile.ai[0] != 3) ? Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value : FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Challengers/BaronRocketTorp", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + drawOffset + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);

          
            return false;
        }
    }
}
