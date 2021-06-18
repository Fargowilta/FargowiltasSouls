using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NinjaEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ninja Enchantment");
            Tooltip.SetDefault(
@"Use Ninja hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
First Strike ensures your next attack is a crit dealing 3x damage
'Now you see me, now you don’t'");
            DisplayName.AddTranslation(GameCulture.Chinese, "忍者魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"若背包中有烟雾弹，按下“投掷烟雾弹”键后会扔出一颗烟雾弹，再次按下该键时会将你传送至烟雾弹的位置并给予你先发制人增益
使用混沌传送杖也会获得先发制人增益
先发制人增益会使你下次攻击必定暴击且造成3倍伤害
“你现在能看到我了。诶，你又看不到我了”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(48, 49, 52);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Green;
            item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().NinjaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NinjaHood);
            recipe.AddIngredient(ItemID.NinjaShirt);
            recipe.AddIngredient(ItemID.NinjaPants);
            //chain knife
            recipe.AddIngredient(ItemID.Katana);
            recipe.AddIngredient(ItemID.Shuriken, 300);
            //throwing knives
            recipe.AddIngredient(ItemID.SmokeBomb, 50);
            //recipe.AddIngredient(ItemID.SlimeHook);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
