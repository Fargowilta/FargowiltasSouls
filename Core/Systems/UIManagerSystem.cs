using FargowiltasSouls.Content.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Core.Systems
{
    public class UIManagerSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            FargoUIManager.UpdateUI(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            FargoUIManager.ModifyInterfaceLayers(layers);
        }
    }
}