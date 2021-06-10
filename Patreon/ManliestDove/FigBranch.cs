using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.ManliestDove
{
    public class FigBranch : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fig Branch");
            DisplayName.AddTranslation(GameCulture.Chinese, "无花果枝");
            Tooltip.SetDefault("Summons a Dove companion");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤一只和平鸽");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<DoveProj>();
            item.buffType = ModContent.BuffType<DoveBuff>();
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
        if (Language.ActiveCulture == GameCulture.Chinese)
            {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> 捐赠者物品 <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
            }
            else
            {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
            }
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonDove)
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddRecipeGroup("FargowiltasSouls:AnyBird");
                recipe.AddIngredient(ItemID.Wood, 50);
                recipe.AddIngredient(ItemID.BorealWood, 50);
                recipe.AddIngredient(ItemID.RichMahogany, 50);
                recipe.AddIngredient(ItemID.PalmWood, 50);
                recipe.AddIngredient(ItemID.Ebonwood, 50);
                recipe.AddIngredient(ItemID.Shadewood, 50);

                recipe.AddTile(TileID.LivingLoom);

                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }
    }
}
