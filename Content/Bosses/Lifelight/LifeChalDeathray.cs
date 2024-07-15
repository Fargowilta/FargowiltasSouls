using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;


using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{
    public class LifeChalDeathray : BaseDeathray, IPixelatedPrimitiveRenderer
    {
        public static readonly SoundStyle ScreechSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightScreech1") with { Volume = 1.5f };
        public static readonly SoundStyle DashSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightDash") with { Volume = 1.5f };
        public static readonly SoundStyle DashSound2 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightPixieDash") with { Volume = 1.5f };
        public static readonly SoundStyle RuneSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightRuneSound") with { Volume = 1.5f };
        public static readonly SoundStyle TelegraphSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightShotPrep") with { Volume = 1.5f };

        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/AbomDeathray";

        public LifeChalDeathray() : base(3600) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Holy Deathray");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft *= 10;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<LifeChallenger>());
            if (npc == null || !npc.active || npc.type != ModContent.NPCType<LifeChallenger>())
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = npc.Center;
                LifeChallenger lifelight = npc.As<LifeChallenger>();
                Projectile.rotation = lifelight.GunRotation;
                Projectile.velocity = Projectile.rotation.ToRotationVector2();
                maxTime = Projectile.ai[2];
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                //SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
            }
            float num801 = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 2f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.CopperCoin, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }

            SpawnSparks();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<PurifiedBuff>(), 300);
                target.AddBuff(ModContent.BuffType<SmiteBuff>(), 60 * 3);
            }
        }

        public void SpawnSparks()
        {
            int sparkRate = (int)MathHelper.Lerp(3f, 20f, Utils.GetLerpValue(1f, 0f, Projectile.scale));

            if (Main.rand.NextBool(sparkRate) && Main.netMode != NetmodeID.Server)
            {
                Vector2 position = Projectile.Center + Projectile.velocity * 30f + Main.rand.NextVector2Circular(10f, 10f);

                Vector2 velocity = Main.rand.NextFloat(MathF.Tau).ToRotationVector2() * Main.rand.NextFloat(7f, 15f);

                Color color = Color.Lerp(Color.Gold, Color.OrangeRed, Main.rand.NextFloat(0f, 0.5f));
                if (Main.rand.NextBool(3))
                    color = Color.Lerp(color, Color.Pink, Main.rand.NextFloat(0f, 0.6f));

                new SparkParticle(position, velocity, color, Main.rand.NextFloat(1.5f, 1.9f), Main.rand.Next(25, 45),
                    true, Color.PaleGoldenrod).Spawn();
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float WidthFunction(float completionRatio) => Projectile.scale * Projectile.width;

        public Color ColorFunction(float completionRatio)
        {
            float colorInterpolant = MathF.Sin(Main.GlobalTimeWrappedHourly * -3.2f + completionRatio * 23f) * 0.5f + 0.5f;
            return Color.Lerp(Color.Lerp(Color.Gold, Color.PaleGoldenrod, colorInterpolant * 0.67f), Color.White, 0.1f);
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.LifelightDeathray");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);

            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * Projectile.localAI[1];
            Vector2[] points = new Vector2[8];

            for (int i = 0; i < points.Length; i++)
                points[i] = Vector2.Lerp(start, end, (float)i / points.Length);

            shader.TrySetParameter("mainColor", Color.Pink);
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.SmokyNoise.Value);
            FargoSoulsUtil.SetTexture2(FargosTextureRegistry.WavyNoise.Value);

            PrimitiveRenderer.RenderTrail(points, new(WidthFunction, ColorFunction, Pixelate: true, Shader: shader), 40);
            Vector2 drawPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 30f - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 5f, Projectile.localAI[0], true);
            float scale = MathHelper.Lerp(0.05f, 0.175f, FargoSoulsUtil.SineInOut(Projectile.scale));
            // Draw a nice bloom flare.
            Texture2D bloomFlare = FargosTextureRegistry.BloomFlareTexture.Value;
            Texture2D bloom = FargosTextureRegistry.BloomParticleTexture.Value;
            float rotation = Main.GlobalTimeWrappedHourly * 1.1f;

            Color bloomFlareColor1 = Color.OrangeRed with { A = 0 } * 0.7f;
            Color bloomFlareColor2 = Color.Gold with { A = 0 } * 0.7f;

            float bigGlowOpacity = MathHelper.Lerp(0.3f, 0.4f, (1f + MathF.Sin(MathF.PI * Main.GlobalTimeWrappedHourly)) * 0.5f);
            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, Color.Gold with { A = 0 } * bigGlowOpacity, 0f, bloom.Size() * 0.5f, 10f, 0, 0f);


            float orbScale = MathHelper.Lerp(1.2f, 1.6f, (1f + MathF.Sin(MathF.PI * Main.GlobalTimeWrappedHourly * 9.5f)) * 0.5f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, Color.PaleGoldenrod with { A = 0 }, 0f, bloom.Size() * 0.5f, orbScale, 0, 0f);

            Main.spriteBatch.Draw(bloomFlare, drawPosition, null, bloomFlareColor1, rotation, bloomFlare.Size() * 0.5f, scale, 0, 0f);
            Main.spriteBatch.Draw(bloomFlare, drawPosition, null, bloomFlareColor2, -rotation, bloomFlare.Size() * 0.5f, scale, 0, 0f);
        }
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {


        }
    }
}