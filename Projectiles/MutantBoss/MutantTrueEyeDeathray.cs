﻿using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantTrueEyeDeathray : Deathrays.BaseDeathray, IPixelPrimitiveDrawer
    {
        public PrimDrawer LaserDrawer { get; private set; } = null;

        public override string Texture => "FargowiltasSouls/Projectiles/Deathrays/PhantasmalDeathrayML";
        public MutantTrueEyeDeathray() : base(90) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Phantasmal Deathray");
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override void AI()
        {
            Projectile.hide = false;
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*int ai1 = (int)Projectile.ai[1];
            if (Main.projectile[ai1].active && Main.projectile[ai1].type == ModContent.ProjectileType<MutantTrueEyeL>())
            {
                Projectile.Center = Main.projectile[ai1].Center - Vector2.UnitY * 6f;
            }
            else
            {
                Projectile.Kill();
                return;
            }*/
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Zombie_104") with { Volume = 0.5f }, Projectile.Center);
            }
            float num801 = 0.4f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 10f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
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
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, DelegateMethods.CastLight);

            Projectile.position -= Projectile.velocity;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float WidthFunction(float trailInterpolant) => Projectile.width * Projectile.scale * 1.3f;

        public Color ColorFunction(float trailInterpolant)
        {
            Color color = Color.Cyan;//Color.Lerp(new(31, 187, 192), new(51, 255, 191), trailInterpolant) * Projectile.Opacity;
            color.A = 100;
            return color;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            if (Projectile.hide)
                return;

            LaserDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["FargowiltasSouls:GenericDeathray"]);

            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance * 1.1f;

            // Create 8 points that span across the draw distance from the projectile center.

            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 70f;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Set shader parameters. This one takes a fademap and a color.

            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].SetShaderTexture(FargosTextureRegistry.MutantStreak);
            // The laser should fade to this in the middle.
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].UseColor(new Color(183, 252, 253, 100));
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["stretchAmount"].SetValue(3);
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["scrollSpeed"].SetValue(2f);
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["uColorFadeScaler"].SetValue(1f);
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["useFadeIn"].SetValue(true);

            LaserDrawer.DrawPixelPrims(baseDrawPoints, -Main.screenPosition, 20);
        }
    }
}