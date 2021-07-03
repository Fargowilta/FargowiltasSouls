using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CactusEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cactus Enchantment");
            Tooltip.SetDefault(
@"25% of contact damage is reflected
Enemies may explode into needles on death
'It's the quenchiest!'");
            DisplayName.AddTranslation(GameCulture.Chinese, "仙人掌魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"反弹25%接触伤害
敌人死亡时有几率爆裂出针刺
“太解渴了！”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(121, 158, 29);
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
            player.GetModPlayer<FargoPlayer>().CactusEffect();
            player.thorns = .25f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CactusHelmet);
            recipe.AddIngredient(ItemID.CactusBreastplate);
            recipe.AddIngredient(ItemID.CactusLeggings);
            recipe.AddIngredient(ItemID.CactusSword);
            //recipe.AddIngredient(ItemID.Sandgun);
            recipe.AddIngredient(ItemID.Waterleaf);
            //recipe.AddIngredient(ItemID.PinkPricklyPear);
            //any dragonfly
            // flounder
            recipe.AddIngredient(ItemID.SecretoftheSands);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
