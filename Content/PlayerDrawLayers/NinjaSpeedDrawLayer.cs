using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class NinjaSpeedDrawLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => false;
        public int DrawTime = 0;
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player == null || !player.active || player.dead || player.ghost || player.whoAmI != Main.myPlayer || drawInfo.shadow != 0)
                return false;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer == null)
                return false;
            if (!player.HasEffect<NinjaEffect>())
                return false;

            float maxSpeed = modPlayer.ForceEffect<NinjaEnchant>() ? 7 : 4;
            if (player.velocity.Length() < maxSpeed && DrawTime < 15)
            {
                DrawTime++;
            }
            else if (DrawTime > 0)
            {
                DrawTime--;
            }
            return DrawTime > 0;
        }

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayerLoader.Layers[0]);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            Vector2 drawPos = drawPlayer.Center - Main.screenPosition;

            const float scale = 1f;

            Texture2D texture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height);
            Vector2 origin2 = rectangle.Size() / 2f;

            //float opacity = 0.4f + (-0.4f * drawInfo.drawPlayer.velocity.Length() / (drawPlayer.FargoSouls().ShadowForce ? 8f : 5f));
            float opacity = 0.4f;
            opacity *= Math.Min((int)DrawTime / 15f, 1);
            DrawData data = new(texture, drawPos, rectangle, Color.Black * opacity, 0f, origin2, scale, SpriteEffects.None, 0);
            drawInfo.DrawDataCache.Add(data);
        }
    }
}
