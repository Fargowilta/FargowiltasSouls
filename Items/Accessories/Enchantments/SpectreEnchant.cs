using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SpectreEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Spectre Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "幽魂魔石");
            
            string tooltip =
@"Damage has a chance to spawn damaging orbs
If you crit, you might also get a healing orb
'Their lifeforce will be their undoing'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"伤害敌人时有几率生成幽魂珠
攻击造成暴击时有几率生成治疗珠
'他们的生命力将毁灭他们自己'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(172, 205, 252);
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
            player.GetModPlayer<FargoPlayer>().SpectreEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddRecipeGroup("FargowiltasSouls:AnySpectreHead");
            recipe.AddIngredient(ItemID.SpectreRobe);
            recipe.AddIngredient(ItemID.SpectrePants);
            //spectre wings
            recipe.AddIngredient(ItemID.UnholyTrident);
            //nettle burst
            //recipe.AddIngredient(ItemID.Keybrand);
            recipe.AddIngredient(ItemID.SpectreStaff);
            recipe.AddIngredient(ItemID.BatScepter);
            //bat scepter
            //recipe.AddIngredient(ItemID.WispinaBottle);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
