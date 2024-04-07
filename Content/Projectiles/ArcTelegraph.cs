using FargowiltasSouls.Content.Bosses.Lifelight;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
	/// <summary>
	/// ai0 determines the rotation, ai1 the cone width, ai2 the length
	/// </summary>
	public class ArcTelegraph : ModProjectile
	{

		public ref float Timer => ref Projectile.ai[0];

		public ref float ArcAngle => ref Projectile.ai[1];

		public ref float Width => ref Projectile.ai[2];

		// Can be anything.
		public override string Texture => "Terraria/Images/Extra_" + ExtrasID.MartianProbeScanWave;

		public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;

		public override void SetDefaults()
		{
			Projectile.timeLeft = 60 * 5;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.width = 1;
			Projectile.height = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write7BitEncodedInt(npc);
			writer.Write(Projectile.localAI[1]);
			writer.Write(Projectile.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc = reader.Read7BitEncodedInt();
			Projectile.localAI[1] = reader.ReadSingle();
			Projectile.localAI[2] = reader.ReadSingle();
		}
		int npc;
		public override void OnSpawn(IEntitySource source)
		{
			if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && parentNpc.type == ModContent.NPCType<LifeChallenger>())
			{
				npc = parentNpc.whoAmI;
				float angleToMe = Projectile.velocity.ToRotation();
				float angleToPlayer = (Main.player[parentNpc.target].Center - parentNpc.Center).ToRotation();
				Projectile.localAI[1] = MathHelper.WrapAngle(angleToMe - angleToPlayer);
			}
		}

		public override void AI()
		{
			NPC parent = FargoSoulsUtil.NPCExists(npc);
			if (parent != null)
			{
				Projectile.Center = parent.Center;
				Vector2 offset = Main.player[parent.target].Center - parent.Center;
				Projectile.rotation = offset.RotatedBy(Projectile.localAI[1]).ToRotation();
			}

			Timer++;
		}

		public override bool ShouldUpdatePosition() => false;

		public float WidthFunction(float progress) => Width;

		public Color ColorFunction(float progress)
		{
			float opacity = Math.Min(Timer / 30f, Math.Min(Projectile.timeLeft / 15f, 1));
			return Color.Lerp(Color.Transparent, Color.DeepSkyBlue, opacity);
		}


		public static Matrix GetWorldViewProjectionMatrixIdioticVertexShaderBoilerplate()
		{
			Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2, Main.graphics.GraphicsDevice.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
			Matrix projection = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);
			return view * projection;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Vertex_ArcTelegraph");

			FargoSoulsUtil.SetTexture1(ModContent.Request<Texture2D>("Terraria/Images/Extra_193").Value);
			shader.TrySetParameter("mainColor", Color.Lerp(Color.DeepSkyBlue, Color.SlateBlue, 0.7f));
			shader.Apply();

			VertexStrip vertexStrip = new();
			List<Vector2> positions = new();
			List<float> rotations = new();
			float initialRotation = Projectile.rotation - ArcAngle * 0.5f;
			for (float i = 0; i < 1; i += 0.005f)
			{
				float rotation = initialRotation + ArcAngle * i;
				positions.Add(rotation.ToRotationVector2() * Width + Projectile.Center);
				rotations.Add(rotation + MathHelper.PiOver2);
			}
			vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
			vertexStrip.DrawTrail();
			Main.spriteBatch.ExitShaderRegion();
			return false;
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return false;
		}
	}
}
