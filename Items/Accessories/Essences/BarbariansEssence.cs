using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class BarbariansEssence : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Barbarian's Essence");
            Tooltip.SetDefault(
@"18% increased melee damage
10% increased melee speed
5% increased melee crit chance
'This is only the beginning..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "野蛮人精华");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"增加18%近战伤害
增加10%近战攻速
增加5%近战暴击率
'这只是个开始...'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(255, 111, 6));
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 150000;
            item.rare = ItemRarityID.LightRed;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeDamage += .18f;
            player.meleeSpeed += .1f;
            player.meleeCrit += 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            //consider 10 materials for all?

            recipe.AddIngredient(ItemID.WarriorEmblem);
            recipe.AddIngredient(ItemID.ZombieArm);
            //bloody machete
            recipe.AddIngredient(ItemID.Trident);
            recipe.AddIngredient(ItemID.ChainKnife); //flaming mace
            recipe.AddIngredient(ItemID.StylistKilLaKillScissorsIWish);
            recipe.AddIngredient(ItemID.IceBlade);
            //shroomerang
            recipe.AddIngredient(ItemID.FalconBlade);
            //amazon
            //combat wrench
            //blue moon
            recipe.AddIngredient(ItemID.Flamarang);
            //terragrim -  make recipe phm
            recipe.AddIngredient(ItemID.HallowedBar, 5);

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
