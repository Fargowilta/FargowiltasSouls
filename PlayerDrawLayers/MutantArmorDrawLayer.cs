using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.PlayerDrawLayers
{
    public class MutantArmorDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            drawInfo.drawPlayer.active
            && !drawInfo.drawPlayer.dead
            && !drawInfo.drawPlayer.ghost
            && drawInfo.shadow == 0
            && drawInfo.drawPlayer.GetModPlayer<FargoSoulsPlayer>().MutantSetBonusItem != null;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }

            Player drawPlayer = drawInfo.drawPlayer;
            FargoSoulsPlayer modPlayer = drawPlayer.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.MutantSetBonusItem != null)
            {
                if (modPlayer.frameCounter % 4 == 0)
                {
                    if (++modPlayer.frameMutantAura >= 19)
                        modPlayer.frameMutantAura = 0;
                }

                Texture2D texture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/MutantBoss/MutantAura", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int frameSize = texture.Height / 19;
                int drawX = (int)(drawPlayer.MountedCenter.X - Main.screenPosition.X);
                int drawY = (int)(drawPlayer.MountedCenter.Y - Main.screenPosition.Y - 16 * drawPlayer.gravDir);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), new Rectangle(0, frameSize * modPlayer.frameMutantAura, texture.Width, frameSize), Color.White, drawPlayer.gravDir < 0 ? MathHelper.Pi : 0, new Vector2(texture.Width / 2f, frameSize / 2f), 1f, drawPlayer.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(data);
            }
        }
    }
}
