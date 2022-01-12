using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NebulaEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Nebula Enchantment");
            Tooltip.SetDefault(
@"Hurting enemies has a chance to spawn buff boosters
Buff booster stacking capped at 2
'The pillars of creation have shined upon you'");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"伤害敌人时有几率生成强化增益
强化增益最大堆叠上限为2
'创生之柱照耀着你'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(254, 126, 229);
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
            player.GetModPlayer<FargoPlayer>().NebulaEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NebulaHelmet);
            recipe.AddIngredient(ItemID.NebulaBreastplate);
            recipe.AddIngredient(ItemID.NebulaLeggings);
            //recipe.AddIngredient(ItemID.WingsNebula);
            recipe.AddIngredient(ItemID.NebulaArcanum);
            recipe.AddIngredient(ItemID.NebulaBlaze);
            //LeafBlower
            //bubble gun
            //chaarged blaster cannon
            recipe.AddIngredient(ItemID.LunarFlareBook);

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
