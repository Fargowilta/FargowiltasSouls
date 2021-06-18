using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MinerEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miner Enchantment");
            Tooltip.SetDefault(
@"50% increased mining speed
Shows the location of enemies, traps, and treasures
Light is emitted from the player
'The planet trembles with each swing of your pick'");
            DisplayName.AddTranslation(GameCulture.Chinese, "矿工魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"增加50%挖掘速度
高亮显示敌人、陷阱和宝藏
你会发光
“大地随着你的每一次挥镐而颤动”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(95, 117, 151);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Green;
            item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().MinerEffect(hideVisual, .5f);
            //add effects
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MiningHelmet); //ultrabright helmet
            recipe.AddIngredient(ItemID.MiningShirt);
            recipe.AddIngredient(ItemID.MiningPants);
            //ancient chisel
            recipe.AddIngredient(ItemID.CopperPickaxe);
            recipe.AddIngredient(ItemID.CnadyCanePickaxe);
            recipe.AddIngredient(ItemID.GoldPickaxe);
            recipe.AddIngredient(ItemID.MoltenPickaxe);
            //gem critter? or minecart
            //recipe.AddIngredient(ItemID.MagicLantern);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
