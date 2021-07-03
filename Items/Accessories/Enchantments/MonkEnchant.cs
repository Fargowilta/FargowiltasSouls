using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MonkEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monk Enchantment");
            Tooltip.SetDefault(
@"Allows the ability to dash
Double tap a direction
You are immune to damage and debuffs for half a second after dashing
Dash cooldown is twice as long as normal dashes
Lightning Aura can now crit and strikes faster
'Return to Monk'");
            DisplayName.AddTranslation(GameCulture.Chinese, "武僧魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"使你获得冲刺能力
双击“左”或“右”键进行冲刺
在冲刺后的0.5秒内你免疫伤害和减益
冲刺冷却是普通冲刺的二倍
闪电光环现在可以暴击且攻击速度更快
“返本还僧”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(146, 5, 32);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Yellow;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().MonkEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.MonkBrows);
            recipe.AddIngredient(ItemID.MonkShirt);
            recipe.AddIngredient(ItemID.MonkPants);
            //recipe.AddIngredient(ItemID.MonkBelt);
            recipe.AddIngredient(ItemID.DD2LightningAuraT2Popper);
            //meatball
            //blue moon
            //valor
            recipe.AddIngredient(ItemID.DaoofPow);
            recipe.AddIngredient(ItemID.MonkStaffT2);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
