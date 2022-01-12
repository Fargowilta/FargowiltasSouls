using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PearlwoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Pearlwood Enchantment");
            Tooltip.SetDefault(
@"Projectiles may spawn a star when they hit something
'Too little, too late…'");
            DisplayName.AddTranslation(GameCulture.Chinese, "珍珠木魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"弹幕在击中敌人或物块时有几率生成一颗星星
'既渺小无力，又慢人一步...'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(173, 154, 95);
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
            item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().PearlEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PearlwoodHelmet);
            recipe.AddIngredient(ItemID.PearlwoodBreastplate);
            recipe.AddIngredient(ItemID.PearlwoodGreaves);
            recipe.AddIngredient(ItemID.PearlwoodSword);
            recipe.AddIngredient(ItemID.UnicornonaStick);
            //dragonfruit or starfruit
            //recipe.AddIngredient(ItemID.LightningBug);
            //recipe.AddIngredient(ItemID.Prismite);
            recipe.AddIngredient(ItemID.TheLandofDeceivingLooks);
            //butterfly pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
