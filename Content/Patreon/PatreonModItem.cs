using FargowiltasSouls.Content.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon
{
    public abstract class PatreonModItem : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            TooltipLine line = new(Mod, "tooltip", Language.GetTextValue($"Mods.{Mod.Name}.Items.Extra.PatreonItem"))
            {
                OverrideColor = Color.Orange
            };
            tooltips.Add(line);
        }
    }
}