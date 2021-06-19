using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SquireEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squire Enchantment");
            Tooltip.SetDefault(
@"Increases the effectiveness of healing sources by 25%
Ballista pierces more targets and panics when you take damage
'Squire, will you hurry?'");
            DisplayName.AddTranslation(GameCulture.Chinese, "侍卫魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"回复生命值时，治疗量增加25%
弩车可以刺穿更多目标，你受到伤害时会得到弩车恐慌增益
“侍卫，你能快点吗？”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(148, 143, 140);
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
            player.GetModPlayer<FargoPlayer>().SquireEnchant = true;
            player.setSquireT2 = true;
            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SquireGreatHelm);
            recipe.AddIngredient(ItemID.SquirePlating);
            recipe.AddIngredient(ItemID.SquireGreaves);
            //recipe.AddIngredient(ItemID.SquireShield);
            recipe.AddIngredient(ItemID.DD2BallistraTowerT2Popper);
            //rally
            //lance
            recipe.AddIngredient(ItemID.RedPhasesaber);
            recipe.AddIngredient(ItemID.DD2SquireDemonSword);
            //light discs

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
