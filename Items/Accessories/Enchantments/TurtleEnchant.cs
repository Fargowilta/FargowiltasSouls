using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TurtleEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Turtle Enchantment");
            Tooltip.SetDefault(
@"100% of contact damage is reflected
When standing still and not attacking, you will enter your shell
While in your shell, you will gain 90% damage resistance 
Additionally you will destroy incoming projectiles and deal 10x more thorns damage
The shell lasts at least 1 second and up to 25 attacks blocked
Enemies may explode into needles on death
'You suddenly have the urge to hide in a shell'");
            DisplayName.AddTranslation(GameCulture.Chinese, "乌龟魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"反弹100%接触伤害
站定不动时且不攻击时你会缩进壳里
当你缩进壳里时增加90%伤害减免
当你缩进壳里时你会摧毁来犯的敌对弹幕且反弹10倍近战伤害
壳可以在消失前手动取消且能抵挡25次攻击
敌人死亡时有几率爆裂出针刺
'你突然有一种想躲进壳里的冲动'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(248, 156, 92);
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
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.CactusEffect();
            modPlayer.TurtleEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TurtleHelmet);
            recipe.AddIngredient(ItemID.TurtleScaleMail);
            recipe.AddIngredient(ItemID.TurtleLeggings);
            recipe.AddIngredient(null, "CactusEnchant");
            recipe.AddIngredient(ItemID.ChlorophytePartisan);
            recipe.AddIngredient(ItemID.Yelets);

            //chloro saber
            //
            //jungle turtle
            //recipe.AddIngredient(ItemID.Seaweed);
            //recipe.AddIngredient(ItemID.LizardEgg);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
