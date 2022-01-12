using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SpookyEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Spooky Enchantment");
            Tooltip.SetDefault(
@"All of your minions gain an extra scythe attack
'Melting souls since 1902'");
            DisplayName.AddTranslation(GameCulture.Chinese, "阴森魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"你的召唤物获得了额外的镰刀攻击
'自1902年以来融化的灵魂'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(100, 78, 116);
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
            player.GetModPlayer<FargoPlayer>().SpookyEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpookyHelmet);
            recipe.AddIngredient(ItemID.SpookyBreastplate);
            recipe.AddIngredient(ItemID.SpookyLeggings);
            recipe.AddIngredient(ItemID.ButchersChainsaw);
            recipe.AddIngredient(ItemID.DeathSickle);
            recipe.AddIngredient(ItemID.RavenStaff);

            //psycho knife
            //eoc yoyo
            //dark harvest
            //recipe.AddIngredient(ItemID.CursedSapling);
            //recipe.AddIngredient(ItemID.EyeSpring);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
