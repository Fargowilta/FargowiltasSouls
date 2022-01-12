using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CobaltEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Cobalt Enchantment");
            Tooltip.SetDefault(
@"25% chance for your projectiles to explode into shards
This can only happen once every second
'I can't believe it's not Palladium'");
            DisplayName.AddTranslation(GameCulture.Chinese, "钴蓝魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"你的弹幕有25%几率爆裂成碎片
此效果在每秒内只会发生一次
'真不敢相信这竟然不是钯金'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(61, 164, 196);
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
            player.GetModPlayer<FargoPlayer>().CobaltEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyCobaltHead");
            recipe.AddIngredient(ItemID.CobaltBreastplate);
            recipe.AddIngredient(ItemID.CobaltLeggings);
            recipe.AddIngredient(null, "AncientCobaltEnchant");
            //recipe.AddIngredient(ItemID.Chik);
            recipe.AddIngredient(ItemID.CrystalStorm);
            recipe.AddIngredient(ItemID.CrystalVileShard);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
