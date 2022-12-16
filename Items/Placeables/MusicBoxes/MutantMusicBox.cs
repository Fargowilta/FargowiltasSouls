using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Placeables.MusicBoxes
{
    public class MutantMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Mutant)");
            Tooltip.SetDefault("Sakuzyo 'Steel Red'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, "Assets/Music/SteelRed"),
                    ModContent.ItemType<MutantMusicBox>(),
                    ModContent.TileType<Tiles.MusicBoxes.MutantMusicBoxSheet>());
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)((double)Main.DiscoR * 0.4));
                }
            }
        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.MusicBoxes.MutantMusicBoxSheet>();
            Item.width = 32;
            Item.height = 32;
            Item.rare = 11;
            Item.value = Item.sellPrice(0, 7, 0, 0);
            Item.accessory = true;
        }
    }
}
