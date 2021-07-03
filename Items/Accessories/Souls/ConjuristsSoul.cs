using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class ConjuristsSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Conjurist's Soul");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "幻术师之魂");
            
            string tooltip =
@"30% increased summon damage
Increases your max number of minions by 4
Increases your max number of sentries by 2
Increased minion knockback
'An army at your disposal'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加30%召唤伤害
增加4个最大召唤栏位
增加2个最大哨兵栏为
增加召唤物击退
“一支听命于您的军队”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 1000000;
            item.rare = ItemRarityID.Purple;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(0, 255, 255));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.minionDamage += 0.3f;
            player.maxMinions += 4;
            player.maxTurrets += 2;
            player.minionKB += 3f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "OccultistsEssence");
            recipe.AddIngredient(ItemID.MonkBelt);
            recipe.AddIngredient(ItemID.SquireShield);
            recipe.AddIngredient(ItemID.HuntressBuckler);
            recipe.AddIngredient(ItemID.ApprenticeScarf);
            recipe.AddIngredient(ItemID.PygmyNecklace);
            recipe.AddIngredient(ItemID.PapyrusScarab);

            //blade staff
            recipe.AddIngredient(ItemID.PirateStaff);
            recipe.AddIngredient(ItemID.OpticStaff);
            recipe.AddIngredient(ItemID.DeadlySphereStaff);
            //desert tiger staff
            recipe.AddIngredient(ItemID.StaffoftheFrostHydra);
            //mourningstar?
            //recipe.AddIngredient(ItemID.DD2BallistraTowerT3Popper);
            //recipe.AddIngredient(ItemID.DD2ExplosiveTrapT3Popper);
            //recipe.AddIngredient(ItemID.DD2FlameburstTowerT3Popper);
            //recipe.AddIngredient(ItemID.DD2LightningAuraT3Popper);
            recipe.AddIngredient(ItemID.TempestStaff);
            recipe.AddIngredient(ItemID.RavenStaff);
            recipe.AddIngredient(ItemID.XenoStaff);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
