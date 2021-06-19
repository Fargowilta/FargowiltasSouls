using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ValhallaKnightEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valhalla Knight Enchantment");
            Tooltip.SetDefault(
@"Increases the effectiveness of healing sources by 33%
Greatly enhances Ballista effectiveness
'Valhalla calls'");
            DisplayName.AddTranslation(GameCulture.Chinese, "英灵殿骑士魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"增加33%受治疗量
大幅强化弩车的效果
“瓦尔哈拉的呼唤”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(147, 101, 30);
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
            item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().ValhallaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SquireAltHead);
            recipe.AddIngredient(ItemID.SquireAltShirt);
            recipe.AddIngredient(ItemID.SquireAltPants);
            recipe.AddIngredient(ItemID.VikingHelmet);
            recipe.AddIngredient(null, "SquireEnchant");
            //recipe.AddIngredient(ItemID.ShinyStone);
            //starlight
            //shadow lance
            recipe.AddIngredient(ItemID.DD2SquireBetsySword);
            //recipe.AddIngredient(ItemID.DD2PetDragon);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
