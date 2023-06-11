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

    public class BaronNuke : ModProjectile
    {

        private int ExplosionDiameter = FargoSoulsWorld.MasochistModeReal ? 400 : 350;

        private SoundStyle Beep = new SoundStyle("FargowiltasSouls/Sounds/NukeBeep");
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banished Baron's Spicy Beeping Nuclear Torpedo of Death and Destruction");
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 2f;
            Projectile.light = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //circular hitbox
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
        }
        private int NextBeep = 1;
        private int beep = 1;
        public override void AI()
        {
            if (Projectile.localAI[0] == NextBeep)
            {
                SoundEngine.PlaySound(Beep, Projectile.Center);
                NextBeep = (int)((int)Projectile.localAI[0] + Math.Floor(Projectile.ai[0] / (3 + (2*beep))));
                beep++;
            }
            Dust.NewDust(Projectile.Center - new Vector2(1, 1), 2, 2, DustID.Water, -Projectile.velocity.X, -Projectile.velocity.Y, 0, default(Color), 1f);
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.Pi).ToRotation();

            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            if (++Projectile.localAI[0] >= Projectile.ai[0] - 2)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 0;
                Projectile.position = Projectile.Center;
                Projectile.width = ExplosionDiameter;
                Projectile.height = ExplosionDiameter;
                Projectile.Center = Projectile.position;
            }
            
            if (Projectile.localAI[0] > Projectile.ai[0])
            {
                Projectile.Kill();
            }
            Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[1]);
            if (player != null) //homing
            {
                Vector2 vectorToIdlePosition = player.Center - Projectile.Center;
                float speed = FargoSoulsWorld.MasochistModeReal ? 24f : 20f;
                float inertia = 48f;
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            //}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
            Projectile.soundDelay = 10;
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.9f;
            }
            return false;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 100; i++)
            {
                Vector2 pos = Projectile.Center + new Vector2(0, Main.rand.Next(Projectile.width)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)); //circle with highest density in middle
                int d = Dust.NewDust(pos, 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 24; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 pos = new Vector2(0, 6).RotatedBy(i * MathHelper.TwoPi / 24);
                    Vector2 vel = pos.RotatedBy(Main.rand.NextFloat(-MathHelper.TwoPi / 64, MathHelper.TwoPi / 64));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + pos, vel, ModContent.ProjectileType<BaronShrapnel>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 0);
                }
            }
        }
        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.Pink * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }*/
        //(public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity * 0.9f;
        public override bool PreDraw(ref Color lightColor)
        {
            //draw glow ring
            float modifier = Projectile.localAI[0] / Projectile.ai[0];
            Color RingColor = Color.Lerp(Color.Orange, Color.Red, modifier);
            Texture2D ringTexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int ringy = ringTexture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            float RingScale = Projectile.scale * ExplosionDiameter / ringTexture.Height;
            int ringy3 = ringy * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle ringrect = new Rectangle(0, ringy3, ringTexture.Width, ringy);
            Vector2 ringorigin = ringrect.Size() / 2f;
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(ringrect), Projectile.GetAlpha(RingColor), Projectile.rotation, ringorigin, RingScale, SpriteEffects.None, 0);

            //draw projectile
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
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
