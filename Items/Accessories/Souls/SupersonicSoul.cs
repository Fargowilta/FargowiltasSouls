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
            DisplayName.SetDefault("Supersonic Soul");

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
@"使你获得超音速奔跑和飞行能力，在冰面上获得额外机动性
未装备翅膀类饰品时使你获得五段跳能力
增加跳跃高度和跳跃速度，使你获得自动跳跃能力
你走过的草地上会生长花朵
使你获得游泳能力并大幅延长水下呼吸时间
使你获得在水和熔岩表面行走能力
使你免疫熔岩和摔落伤害
拥有飞毯、克苏鲁护盾和忍者大师装备效果
拥有甜心项链和琥珀马掌气球效果
'我就是速度'";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "超音速之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
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
