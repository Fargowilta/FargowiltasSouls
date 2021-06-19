using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShroomiteEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomite Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "蘑菇魔石");
            
            string tooltip =
@"All attacks gain trails of mushrooms
Not moving puts you in stealth
While in stealth, more mushrooms will spawn
'Made with real shrooms!'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"所有攻击都会留下蘑菇尾迹
站定不动时使你进入隐身状态
处于隐身状态时，攻击会产生更多蘑菇
“是用真的蘑菇做的！”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(0, 140, 244);
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
            player.GetModPlayer<FargoPlayer>().ShroomiteEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyShroomHead");
            recipe.AddIngredient(ItemID.ShroomiteBreastplate);
            recipe.AddIngredient(ItemID.ShroomiteLeggings);
            //shroomite digging
            //hammush
            recipe.AddIngredient(ItemID.MushroomSpear);
            recipe.AddIngredient(ItemID.Uzi);
            //venus magnum
            recipe.AddIngredient(ItemID.TacticalShotgun);
            //recipe.AddIngredient(ItemID.StrangeGlowingMushroom);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
