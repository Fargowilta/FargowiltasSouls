using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class SlingersEssence : SoulsItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Slinger's Essence");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "投手精华");
            
            string tooltip =
@"18% increased throwing damage
5% increased throwing critical chance
5% increased throwing velocity
'This is only the beginning..'";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"增加18%投掷伤害
增加5%投掷暴击率
增加5%投掷物力度
'这只是个开始...'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(85, 5, 230));
                }
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 150000;
            item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.thrownDamage += 0.18f;
            //player.thrownCrit += 5;
            //player.thrownVelocity += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            /*
            else
            {
                //no others
                recipe.AddIngredient(fargos.ItemType("WoodYoyoThrown"));
                recipe.AddIngredient(fargos.ItemType("BloodyMacheteThrown"));
                recipe.AddIngredient(fargos.ItemType("IceBoomerangThrown"));
                recipe.AddIngredient(ItemID.AleThrowingGlove);
                recipe.AddIngredient(ItemID.PartyGirlGrenade, 300);
                recipe.AddIngredient(fargos.ItemType("TheMeatballThrown"));
                recipe.AddIngredient(fargos.ItemType("JungleYoyoThrown"));
                recipe.AddIngredient(ItemID.Beenade, 300);
                recipe.AddIngredient(ItemID.BoneGlove);
                recipe.AddIngredient(fargos.ItemType("BlueMoonThrown"));
                recipe.AddIngredient(fargos.ItemType("FlamarangThrown"));
            }*/

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
