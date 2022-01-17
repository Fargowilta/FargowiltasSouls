using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shoes)]
    public class SupersonicSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Supersonic Soul");

            string tooltip =
@"Allows Supersonic running, flight, and extra mobility on ice
Allows the holder to quintuple jump if no wings are equipped
Increases jump height, jump speed, and allows auto-jump
Flowers grow on the grass you walk on
Grants the ability to swim and greatly extends underwater breathing
Provides the ability to walk on water and lava
Grants immunity to lava and fall damage
Effects of Flying Carpet, Shield of Cthulhu and Master Ninja Gear
Effects of Sweetheart Necklace and Amber Horseshoe Balloon
'I am speed'";
            string tooltip_ch =
@"'我就是速度'
获得超音速奔跑,飞行,以及额外的冰上移动力
在没有装备翅膀时,允许使用者进行五段跳
增加跳跃高度,跳跃速度,允许自动跳跃
获得游泳能力以及极长的水下呼吸时间
获得水/岩浆上行走能力
免疫岩浆和坠落伤害
拥有飞毯效果";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "超音速之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 750000;
            item.rare = ItemRarityID.Purple;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(238, 0, 69));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.SupersonicSoul(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ModContent.ItemType<Masomode.AeolusBoots>()); //add terraspark boots
            //amphibian boots
            recipe.AddIngredient(ItemID.FlowerBoots); //fairy boots
            //hellfire treads?
            recipe.AddIngredient(ItemID.FlyingCarpet);
            recipe.AddIngredient(ItemID.SweetheartNecklace);
            recipe.AddIngredient(ItemID.FrogLeg); //frog gear
            recipe.AddIngredient(ItemID.BalloonHorseshoeHoney);
            recipe.AddIngredient(ItemID.BundleofBalloons); //(change recipe to use horsehoe varaints ??)
            recipe.AddIngredient(ItemID.EoCShield);
            recipe.AddIngredient(ItemID.MasterNinjaGear);

            recipe.AddIngredient(ItemID.MinecartMech);
            recipe.AddIngredient(ItemID.BlessedApple);
            recipe.AddIngredient(ItemID.AncientHorn);
            recipe.AddIngredient(ItemID.ReindeerBells);
            recipe.AddIngredient(ItemID.BrainScrambler);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}