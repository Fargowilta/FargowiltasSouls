using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SolarEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Enchantment");
            Tooltip.SetDefault(
@"Solar shield allows you to dash through enemies
Solar shield is not depleted on hit, but has reduced damage reduction
Attacks may inflict the Solar Flare debuff
'Too hot to handle'");
            DisplayName.AddTranslation(GameCulture.Chinese, "耀斑魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"召唤日耀护盾，让你能冲撞敌人
被击中时日耀护盾不会耗尽，但伤害减免效果会降低
攻击有几率造成太阳耀斑减益
“烫手魔石”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(254, 158, 35);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Red;
            item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //solar shields
            modPlayer.SolarEffect();
            //flare debuff
            modPlayer.SolarEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SolarFlareHelmet);
            recipe.AddIngredient(ItemID.SolarFlareBreastplate);
            recipe.AddIngredient(ItemID.SolarFlareLeggings);
            //solar wings
            recipe.AddIngredient(ItemID.HelFire);
            //golem fist
            //xmas tree sword
            //recipe.AddIngredient(ItemID.SolarEruption);
            recipe.AddIngredient(ItemID.DayBreak);
            recipe.AddIngredient(ItemID.StarWrath); //terrarian

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
