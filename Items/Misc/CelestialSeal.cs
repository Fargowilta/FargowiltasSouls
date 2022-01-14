using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class CelestialSeal : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Celestial Seal");
            Tooltip.SetDefault(@"Permanently increases the number of accessory slots
Only usable after Demon Heart");
            DisplayName.AddTranslation(GameCulture.Chinese, "天篆");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"永久增加饰品栏
只能在使用恶魔之心之后使用");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Purple;
            item.maxStack = 99;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 30;
            item.useTime = 30;
            item.consumable = true;
            item.UseSound = SoundID.Item123;
            item.value = Item.sellPrice(0, 15);
        }

        public override bool CanUseItem(Player player)
        {
            return player.extraAccessorySlots == 1;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoPlayer>().CelestialSeal = true;
            }
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ModContent.\1Type<\2>\(\), 15);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.FragmentNebula, 25);
            recipe.AddIngredient(ItemID.FragmentSolar, 25);
            recipe.AddIngredient(ItemID.FragmentStardust, 25);
            recipe.AddIngredient(ItemID.FragmentVortex, 25);

            recipe.AddTile(mod, "CrucibleCosmosSheet");

            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}