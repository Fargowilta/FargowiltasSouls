using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShinobiEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shinobi Infiltrator Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "渗透忍者魔石");
            
            string tooltip =
@"Dash into any walls, to teleport through them to the next opening
Allows the ability to dash
Double tap a direction
Throw a smoke bomb to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
Greatly enhances Lightning Aura effectiveness
'Village Hidden in the Wall'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"朝墙壁冲刺时会直接穿过去
使你获得冲刺能力
双击“左”或“右”键进行冲刺
若背包中有烟雾弹，按下“投掷烟雾弹”键后会扔出一颗烟雾弹，再次按下该键时会将你传送至烟雾弹的位置并给予你先发制人增益
使用混沌传送杖也会获得先发制人增益
大幅强化闪电光环的效果
“藏匿于墙中的村庄”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(147, 91, 24);
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
            
            //tele thru wall
            modPlayer.ShinobiEffect(hideVisual);
            //monk dash mayhem
            modPlayer.MonkEffect();
            //ninja, smoke bombs, pet
            modPlayer.NinjaEffect(hideVisual); //destroy
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MonkAltHead);
            recipe.AddIngredient(ItemID.MonkAltShirt);
            recipe.AddIngredient(ItemID.MonkAltPants);
            recipe.AddIngredient(null, "NinjaEnchant");
            recipe.AddIngredient(null, "MonkEnchant");
            recipe.AddIngredient(ItemID.PsychoKnife);
            //chain guiottine
            //code 2
            //flower pow
            //stynger
            //recipe.AddIngredient(ItemID.DD2PetGato);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
