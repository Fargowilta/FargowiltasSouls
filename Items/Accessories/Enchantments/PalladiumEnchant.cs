using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalladiumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Palladium Enchantment");
            Tooltip.SetDefault(
@"Briefly increases life regeneration after striking an enemy
You spawn an orb of damaging life energy every 80 life regenerated
'You feel your wounds slowly healing' ");
            DisplayName.AddTranslation(GameCulture.Chinese, "钯金魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"击中敌人后暂时增加你的生命恢复速度
你每恢复80点生命值便会生成一个伤害性的生命能量球
“你感到你的伤口在慢慢愈合”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(245, 172, 40);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Pink;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().PalladiumEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyPallaHead");
            recipe.AddIngredient(ItemID.PalladiumBreastplate);
            recipe.AddIngredient(ItemID.PalladiumLeggings);
            recipe.AddIngredient(ItemID.PalladiumSword);
            recipe.AddIngredient(ItemID.SoulDrain);
            //sanguine staff
            //recipe.AddIngredient(ItemID.VampireKnives);
            recipe.AddIngredient(ItemID.UndergroundReward);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
