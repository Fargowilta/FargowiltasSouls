using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientCobaltEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Ancient Cobalt Enchantment");
            Tooltip.SetDefault(
@"20% chance for your projectiles to explode into stingers
This can only happen once every second
'The jungle of old empowers you'");
            DisplayName.AddTranslation(GameCulture.Chinese, "远古钴魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"你的弹幕有20%几率爆裂成毒刺
此效果在每秒内只会发生一次
'古老的丛林赋予你力量'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(53, 76, 116);
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
            player.GetModPlayer<FargoPlayer>().AncientCobaltEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.AncientCobaltHelmet);
            recipe.AddIngredient(ItemID.AncientCobaltBreastplate);
            recipe.AddIngredient(ItemID.AncientCobaltLeggings);
            //recipe.AddIngredient(ItemID.AncientIronHelmet);
            recipe.AddIngredient(ItemID.Blowpipe);
            recipe.AddIngredient(ItemID.PoisonDart, 300);
            recipe.AddIngredient(ItemID.PoisonedKnife, 300);
            //moon glow
            //buggy /grubby whoever isnt used
            //variegated lardfish

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
