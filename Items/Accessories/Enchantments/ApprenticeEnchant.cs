using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ApprenticeEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apprentice Enchantment");
            Tooltip.SetDefault(
@"After attacking for 2 seconds you will be enveloped in flames
Switching weapons will increase the next attack's damage by 50% and spawn an inferno
Flameburst field of view and range are dramatically increased
'A long way to perfection'");
            DisplayName.AddTranslation(GameCulture.Chinese, "学徒魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"持续攻击两秒后你将被火焰包裹
切换武器后，下次攻击的伤害增加50%，并造成地狱爆炸
大幅增加爆炸烈焰哨兵的索敌范围和攻击距离
“追求完美的漫漫长路”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(93, 134, 166);
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
            player.GetModPlayer<FargoPlayer>().ApprenticeEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ApprenticeHat);
            recipe.AddIngredient(ItemID.ApprenticeRobe);
            recipe.AddIngredient(ItemID.ApprenticeTrousers);
            //recipe.AddIngredient(ItemID.ApprenticeScarf);
            recipe.AddIngredient(ItemID.DD2FlameburstTowerT2Popper);
            //magic missile
            //ice rod
            //golden shower
            recipe.AddIngredient(ItemID.BookStaff);
            recipe.AddIngredient(ItemID.ClingerStaff);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
