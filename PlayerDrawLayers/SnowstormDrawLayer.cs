using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.PlayerDrawLayers
{
    public class SnowstormDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            drawInfo.drawPlayer.whoAmI == Main.myPlayer
            && drawInfo.drawPlayer.active
            && !drawInfo.drawPlayer.dead
            && !drawInfo.drawPlayer.ghost
            && drawInfo.shadow == 0
            && drawInfo.drawPlayer.GetModPlayer<FargoSoulsPlayer>().SnowVisual;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }

            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("FargowiltasSouls");
            FargoSoulsPlayer modPlayer = drawPlayer.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.SnowVisual)
            {
                if (modPlayer.frameCounter % 5 == 0)
                {
                    if (++modPlayer.frameSnow > 20)
                        modPlayer.frameSnow = 1;
                }

                Texture2D texture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Souls/SnowBlizzard", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int frameSize = texture.Height / 20;
                int drawX = (int)(drawPlayer.MountedCenter.X - Main.screenPosition.X);
                int drawY = (int)(drawPlayer.MountedCenter.Y - Main.screenPosition.Y);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), new Rectangle(0, frameSize * modPlayer.frameSnow, texture.Width, frameSize), Lighting.GetColor((int)((drawInfo.Position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.Position.Y + drawPlayer.height / 2f) / 16f)), drawPlayer.gravDir < 0 ? MathHelper.Pi : 0f, new Vector2(texture.Width / 2f, frameSize / 2f), 1f, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(data);
            }

            //GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type, drawPlayer, data);
        }
    }
}
