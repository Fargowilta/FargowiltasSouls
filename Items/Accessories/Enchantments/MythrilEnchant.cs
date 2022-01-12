using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MythrilEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Mythril Enchantment");
            Tooltip.SetDefault(
@"15% increased weapon use speed
Taking damage temporarily removes this weapon use speed increase
'You feel the knowledge of your weapons seep into your mind'");
            DisplayName.AddTranslation(GameCulture.Chinese, "秘银魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"增加15%武器使用速度
受到伤害时武器使用速度增加效果会暂时失效
'你感觉你对武器的知识渗透进了你的脑海中");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(157, 210, 144);
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
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

            if (player.GetToggleValue("Mythril"))
            {
                fargoPlayer.MythrilEnchant = true;
                if (!fargoPlayer.DisruptedFocus)
                    fargoPlayer.AttackSpeed += fargoPlayer.WizardEnchant ? .2f : .15f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyMythrilHead");
            recipe.AddIngredient(ItemID.MythrilChainmail);
            recipe.AddIngredient(ItemID.MythrilGreaves);
            //flintlock pistol
            //recipe.AddIngredient(ItemID.LaserRifle);
            recipe.AddIngredient(ItemID.ClockworkAssaultRifle);
            recipe.AddIngredient(ItemID.Gatligator);
            recipe.AddIngredient(ItemID.OnyxBlaster);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
