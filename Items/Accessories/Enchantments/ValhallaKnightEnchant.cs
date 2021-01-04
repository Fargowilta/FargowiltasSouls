using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ValhallaKnightEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valhalla Knight Enchantment");
            Tooltip.SetDefault(
@"Continually attacking an enemy will grant you the Power of Valhalla buff
You will drastically reduce enemy immunity frames during this time
Greatly enhances Ballista effectiveness
Effects of Shiny Stone
Summons a pet Dragon
'Valhalla calls'");
            DisplayName.AddTranslation(GameCulture.Chinese, "瓦尔哈拉骑士魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'瓦尔哈拉的呼唤'
持续攻击敌人会给予你瓦尔哈拉之力buff
持续时间内大幅削减敌人无敌帧
大大提高弩车能力
拥有闪耀石的效果
召唤一只宠物小龙");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(147, 101, 30);
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
            player.GetModPlayer<FargoPlayer>().ValhallaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SquireAltHead);
            recipe.AddIngredient(ItemID.SquireAltShirt);
            recipe.AddIngredient(ItemID.SquireAltPants);
            //viking helmet
            recipe.AddIngredient(null, "SquireEnchant");
            recipe.AddIngredient(ItemID.ShinyStone);
            //starlight
            //shadow lance
            recipe.AddIngredient(ItemID.DD2SquireBetsySword);
            recipe.AddIngredient(ItemID.DD2PetDragon);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
