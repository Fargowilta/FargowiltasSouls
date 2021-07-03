using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class OrichalcumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orichalcum Enchantment");
            Tooltip.SetDefault(
@"Flower petals will cause extra damage to your target and inflict Orichalcum Poison
Damaging debuffs deal 3x damage
'Nature blesses you'");
            DisplayName.AddTranslation(GameCulture.Chinese, "山铜魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"花瓣将落到你攻击的敌人的身上，造成额外伤害和山铜中毒减益
伤害性减益造成的伤害x3
“自然祝福着你”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(235, 50, 145);
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
            player.GetModPlayer<FargoPlayer>().OrichalcumEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyOriHead");
            recipe.AddIngredient(ItemID.OrichalcumBreastplate);
            recipe.AddIngredient(ItemID.OrichalcumLeggings);
            //recipe.AddIngredient(ItemID.OrichalcumWaraxe);
            //ori sword
            //flare gun
            recipe.AddIngredient(ItemID.FlowerofFire);
            recipe.AddIngredient(ItemID.FlowerofFrost);
            recipe.AddIngredient(ItemID.CursedFlames);
            //flamethrower

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
