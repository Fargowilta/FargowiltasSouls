using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MeteorEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "陨星魔石");
            
            string tooltip =
@"A meteor shower initiates every few seconds while attacking
'Cosmic power builds your destructive prowess'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"攻击时每过几秒便会释放一次流星雨
“宇宙之力构建你的毁灭力量”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(95, 71, 82);
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
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.MeteorEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.MeteorHelmet);
            recipe.AddIngredient(ItemID.MeteorSuit);
            recipe.AddIngredient(ItemID.MeteorLeggings);
            //meteor hamaxe
            recipe.AddIngredient(ItemID.SpaceGun);
            //orange zapinator, add recipe
            //recipe.AddIngredient(ItemID.StarCannon); //super star shooter, add recipe
            recipe.AddIngredient(ItemID.MeteorStaff);
            recipe.AddIngredient(ItemID.PlaceAbovetheClouds);
            //harpy pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
