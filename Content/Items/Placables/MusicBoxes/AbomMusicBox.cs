using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class AbomMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Music Box (Abominationn)");
            // Tooltip.SetDefault("Sakuzyo 'Stigma'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, musicMod.Version >= Version.Parse("0.1.5") ? "Assets/Music/Laevateinn_P2" : "Assets/Music/Stigma"),
                    ModContent.ItemType<AbomMusicBox>(),
                    ModContent.TileType<Tiles.MusicBoxes.AbomMusicBoxSheet>());
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;
                }
            }
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.MusicBoxes.AbomMusicBoxSheet>();
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 7, 0, 0);
            Item.accessory = true;
        }
    }
}
