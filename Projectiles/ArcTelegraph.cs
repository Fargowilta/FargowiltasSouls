using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using System.Collections.Generic;

namespace FargowiltasSouls.Projectiles
{
    /// <summary>
    /// ai0 determines the rotation, width and length are defined by constants
    /// Note: Heavily unfinished
    /// </summary>
    public class ArcTelegraph : ModProjectile
    {
        private const float coneWidth = MathHelper.Pi * (20 / 48);

        private const float coneLength = 500;

        public Color telegraphColor = Color.White;

        Effect shader;

        public override string Texture => "Terraria/Images/Extra_" + ExtrasID.MartianProbeScanWave;//this is unrelated actually

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arc Telegraph");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Main.NewText(Projectile.timeLeft);
        }
        public float WidthFunction(float progress)
        {
            return coneLength;
        }
        public Color ColorFunction(float progress)
        {
            return telegraphColor;
        }
        public void SetEffectParameters(Effect effect)
        {
            effect.Parameters["WorldViewProjection"].SetValue(GetWorldViewProjectionMatrixIdioticVertexShaderBoilerplate());
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

            shader ??= ModContent.Request<Effect>("FargowiltasSouls/Effects/Vertex_ArcTelegraph", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            SetEffectParameters(shader);
            shader.CurrentTechnique.Passes[0].Apply();
            VertexStrip vertexStrip = new();
            List<Vector2> positions = new();
            List<float> rotations = new();
            for (float i = 0; i < 1; i += 0.005f)
            {
                float rotation = Projectile.ai[0] - coneWidth * 0.5f + coneWidth * i;
                positions.Add(rotation.ToRotationVector2() * coneLength + Projectile.Center);
                rotations.Add(rotation + MathHelper.PiOver2);
            } 
            vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
            vertexStrip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);         
            return false; 
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
}
