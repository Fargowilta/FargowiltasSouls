﻿using FargowiltasSouls.Assets.ExtraTextures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
    public class DeviDeathraySmall : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/DeviDeathray";


        public DeviDeathraySmall() : base(60) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Love Ray");
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<MutantBoss>())
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
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
            /*if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
            }*/
            float num801 = 0.3f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 0.6f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            //Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
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
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;
        }
        public float WidthFunction(float _) => Projectile.width * Projectile.scale * 1.2f;

        public static Color ColorFunction(float _) => new(232, 140, 240, 100);

        public override bool PreDraw(ref Color lightColor)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.GenericDeathray");

            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.
            Vector2 initialDrawPoint = Projectile.Center;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

			// Set shader parameters.
			shader.TrySetParameter("mainColor", new Color(240, 220, 240, 100));
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.MutantStreak.Value);
            shader.TrySetParameter("stretchAmount", 3);
            shader.TrySetParameter("scrollSpeed", 1f);
            shader.TrySetParameter("uColorFadeScaler", 0.8f);

            PrimitiveRenderer.RenderTrail(baseDrawPoints, new(WidthFunction, ColorFunction, Shader: shader), 10);
            return false;
        }
    }
}