using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Sky
{
    public class AbomSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;

        public override void Update(GameTime gameTime)
        {
            const float increment = 0.01f;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>()))
            {
                intensity += increment;
                if (intensity > 1f)
                {
                    intensity = 1f;
                }
            }
            else
            {
                intensity -= increment;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/AbomSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity * 0.75f);
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override Color OnTileColor(Color inColor)
        {
            return new Color(Vector4.Lerp(new Vector4(1f, 0.9f, 0.6f, 1f), inColor.ToVector4(), 1f - intensity));
        }
    }
}