using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ObsidianEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Obsidian Enchantment");
            Tooltip.SetDefault(
@"Grants immunity to fire and lava
You have normal movement and can swim in lava
While standing in lava or lava wet, your attacks spawn explosions
'The earth calls'");
            DisplayName.AddTranslation(GameCulture.Chinese, "黑曜石魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"使你免疫火块与岩浆
你可以在岩浆中正常移动和游泳
在岩浆中或拥有浸入岩浆增益时，你的攻击会引发爆炸
“大地在呼唤”"); 
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(69, 62, 115);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Orange;
            item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().ObsidianEffect(); //add effect
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ObsidianHelm);
            recipe.AddIngredient(ItemID.ObsidianShirt);
            recipe.AddIngredient(ItemID.ObsidianPants);
            recipe.AddIngredient(ItemID.ObsidianRose); //molten skull rose
            //recipe.AddIngredient(ItemID.ObsidianHorseshoe);
            recipe.AddIngredient(ItemID.Cascade);
            recipe.AddIngredient(ItemID.Fireblossom);
            //magma snail
            //obsidifsh
            //mimic pet

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
