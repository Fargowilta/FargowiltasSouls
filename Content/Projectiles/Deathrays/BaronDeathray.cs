using FargowiltasSouls.Assets.ExtraTextures;


using FargowiltasSouls.Content.Bosses.BanishedBaron;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
	public class BaronDeathray : BaseDeathray, IPixelatedPrimitiveRenderer
	{
		public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/DeviDeathray";

		public BaronDeathray() : base(300, drawDistance: 3500) { }

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Love Ray");
		}

		public override void AI()
		{
			Vector2? vector78 = null;
			NPC baron = Main.npc[(int)Projectile.ai[0]];
			if (baron.active && baron.type == ModContent.NPCType<BanishedBaron>())
			{
				Projectile.velocity = baron.rotation.ToRotationVector2();
				Projectile.rotation = baron.rotation;
				Projectile.Center = baron.Center + (baron.width / 1.8f) * Projectile.rotation.ToRotationVector2();
				maxTime = Projectile.ai[2];
			}
			else
			{
				Projectile.Kill();
			}


			float num801 = 0.5f;
			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] >= maxTime)
			{
				Projectile.Kill();
				return;
			}
			Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 3f * num801;
			if (Projectile.scale > num801)
				Projectile.scale = num801;
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
			Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
			for (int num809 = 0; num809 < 1; num809 = num3 + 1)
			{
				float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
				float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
				Vector2 vector80 = new((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
				int num812 = Dust.NewDust(vector79, 0, 0, DustID.GemAmethyst, vector80.X, vector80.Y, 0, default, 1f);
				Main.dust[num812].noGravity = true;
				//Main.dust[num812].scale = 1.7f;
				num3 = num809;
			}
			if (Main.rand.NextBool(5))
			{
				Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
				int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.GemAmethyst, 0f, 0f, 100, default, 1f);
				Dust dust = Main.dust[num813];
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
			}
			//DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
			//Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

			Projectile.position -= Projectile.velocity;

			for (int num809 = 0; num809 < 1; num809 = num3 + 1)
			{
				float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
				float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
				Vector2 vector80 = new((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
				int num812 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemAmethyst, vector80.X, vector80.Y, 0, default, 1f);
				Main.dust[num812].noGravity = true;
				Main.dust[num812].scale = 1f;
				num3 = num809;
			}
		}


		public float WidthFunction(float _) => Projectile.width * Projectile.scale * 1.5f;

		public static Color ColorFunction(float _)
		{
			Color color = Color.DeepPink; //new(232, 140, 240);
			color.A = 0;
			return color;
		}

		public override bool PreDraw(ref Color lightColor) => false;

		public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
		{
			if (Projectile.hide)
				return;

			ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.GenericDeathray");

			// Get the laser end position.
			Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance * 1.1f;

			// Create 8 points that span across the draw distance from the projectile center.
			Vector2 initialDrawPoint = Projectile.Center;
			Vector2[] baseDrawPoints = new Vector2[8];
			for (int i = 0; i < baseDrawPoints.Length; i++)
				baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

			// Set shader parameters.
			shader.TrySetParameter("mainColor", new Color(240, 220, 240, 0));
			FargoSoulsUtil.SetTexture1(FargosTextureRegistry.GenericStreak.Value);
			shader.TrySetParameter("stretchAmount", 3);
			shader.TrySetParameter("scrollSpeed", 1f);
			shader.TrySetParameter("uColorFadeScaler", 0.8f);
			shader.TrySetParameter("useFadeIn", false);

			PrimitiveRenderer.RenderTrail(baseDrawPoints, new(WidthFunction, ColorFunction, Shader: shader), 10);
		}
	}
}