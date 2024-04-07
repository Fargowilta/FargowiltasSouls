using System;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

    public class BaronNuke : ModProjectile
    {

        private readonly int ExplosionDiameter = WorldSavingSystem.MasochistModeReal ? 500 : 500;

        public static readonly SoundStyle Beep = new("FargowiltasSouls/Assets/Sounds/NukeBeep");
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron's Spicy Beeping Nuclear Torpedo of Death and Destruction");
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.timeLeft = LumUtils.SecondsToFrames(60);
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
        private int RingFlash;
        private const int RingFlashDuration = 20;
        ref float Duration => ref Projectile.ai[0];
        ref float Timer => ref Projectile.localAI[0];
        bool Rocket => Projectile.ai[2] != 0;
        public override bool CanHitPlayer(Player target) => Timer > 60;
        public override void AI()
        {
            if (Duration < 190) //make sure it doesn't bug out and explode early
            {
                Duration = 190;
            }
            if (++Projectile.frameCounter > 8)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            if (Timer == NextBeep)
            {
                SoundEngine.PlaySound(Beep, Projectile.Center);
                NextBeep = (int)((int)Timer + Math.Floor(Duration / (3 + 2 * beep)));
                beep++;
                RingFlash = RingFlashDuration;
            }
            if (RingFlash > 0)
                RingFlash--;

            Vector2 backPos = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 130f * Projectile.scale / 2 + Main.rand.NextVector2Circular(10, 10);
            if (Main.rand.NextBool(3) && Projectile.velocity.LengthSquared() > 9)
            {
                if (Rocket)
                {
                    Dust.NewDust(backPos, 2, 2, DustID.Torch, -Projectile.velocity.X, -Projectile.velocity.Y, 0, default, 1f);
                }
                else
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Particle p = new Bubble(backPos, -Projectile.velocity.RotatedByRandom(MathF.PI * 0.12f) * Main.rand.NextFloat(0.6f, 1f) / 2f, 1, 30, rotation: Main.rand.NextFloat(MathF.Tau));
                        p.Spawn();
                    }
                    Dust.NewDust(backPos, 2, 2, DustID.Water, -Projectile.velocity.X, -Projectile.velocity.Y, 0, default, 1f);
                }
            }


            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.Pi).ToRotation();

            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            if (++Timer >= Duration - 2)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 0;
                Projectile.position = Projectile.Center;
                Projectile.width = ExplosionDiameter;
                Projectile.height = ExplosionDiameter;
                Projectile.Center = Projectile.position;
            }

            if (Timer > Duration)
            {
                Projectile.Kill();
            }
            Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[1]);
            if (Timer < 60)
            {
                Projectile.velocity *= 0.965f;

            }
            else if (player != null && player.active && !player.ghost) //homing
            {
                Vector2 vectorToIdlePosition = player.Center - Projectile.Center;
                float speed = WorldSavingSystem.MasochistModeReal ? 24f : 20f;
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
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 60 * 10);
            if (!WorldSavingSystem.EternityMode)
            {
                return;
            }
            target.FargoSouls().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 60 * 30);
            target.AddBuff(BuffID.OnFire3, 60 * 10);
            target.AddBuff(BuffID.BrokenArmor, 60 * 40);

        }
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.FargoSouls().Screenshake = 30;

            for (int i = 0; i < 200; i++)
            {
                Vector2 pos = Projectile.Center + new Vector2(0, Main.rand.NextFloat(ExplosionDiameter * 0.8f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)); //circle with highest density in middle
                Vector2 vel = (pos - Projectile.Center) / 500;
                Particle p = new ExpandingBloomParticle(pos, vel, Color.Lerp(Color.Yellow, Color.Red, pos.Distance(Projectile.Center) / (ExplosionDiameter / 2f)), startScale: Vector2.One * 3, endScale: Vector2.One * 6, lifetime: 60);
                p.Velocity *= 2f;
                p.Spawn();
                //int d = Dust.NewDust(pos, 0, 0, DustID.Fireworks, 0f, 0f, 0, default, 1.5f);
                //Main.dust[d].noGravity = true;
            }

            float scaleFactor9 = 2;
            for (int j = 0; j < 20; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, (Vector2.UnitX * 5).RotatedByRandom(MathHelper.TwoPi), Main.rand.Next(61, 64), scaleFactor9);
            }
            SoundEngine.PlaySound(SoundID.Item62 with { Pitch = -0.2f }, Projectile.Center);

            if (WorldSavingSystem.MasochistModeReal)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 pos = new Vector2(0, Main.rand.NextFloat(5, 7)).RotatedBy(i * MathHelper.TwoPi / 24);
                        Vector2 vel = pos.RotatedBy(Main.rand.NextFloat(-MathHelper.TwoPi / 64, MathHelper.TwoPi / 64));
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + pos, vel, ModContent.ProjectileType<BaronShrapnel>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 0);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].hostile = Projectile.hostile;
                            Main.projectile[p].friendly = Projectile.friendly;
                        }
                    }
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
            if (Timer >= Duration - 2) //if exploding
            {
                return false;
            }
            //draw glow ring
            float modifier = (float)RingFlash / RingFlashDuration;
            Color RingColor = Color.Lerp(Color.Orange, Color.Red, modifier);
            Texture2D ringTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            float RingScale = Projectile.scale * 2 * ExplosionDiameter / ringTexture.Height;
            Rectangle ringrect = new(0, 0, ringTexture.Width, ringTexture.Height);
            Vector2 ringorigin = ringrect.Size() / 2f;
            RingColor *= modifier;
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(ringrect), RingColor, Projectile.rotation, ringorigin, RingScale, SpriteEffects.None, 0);

            //draw projectile
            Texture2D texture2D13 = Rocket ? ModContent.Request<Texture2D>(Texture + "Rocket", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value : Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + drawOffset + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);


            return false;
        }
    }
}
