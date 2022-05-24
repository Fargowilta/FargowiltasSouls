using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shoes)]
    public class SupersonicSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

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
@"'我就是速度'
获得超音速奔跑,飞行,以及额外的冰上移动力
在没有装备翅膀时,允许使用者进行五段跳
增加跳跃高度,跳跃速度,允许自动跳跃
获得游泳能力以及极长的水下呼吸时间
获得水/岩浆上行走能力
免疫岩浆和坠落伤害
拥有飞毯效果";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "超音速之魂");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;
        }

        protected override Color? nameColor => new Color(238, 0, 69);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.SupersonicSoul(Item, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<Masomode.AeolusBoots>()) //add terraspark boots
            .AddIngredient(ItemID.FlyingCarpet)
            .AddIngredient(ItemID.SweetheartNecklace)
            .AddIngredient(ItemID.Magiluminescence)
            .AddIngredient(ItemID.BalloonHorseshoeHoney)
            .AddIngredient(ItemID.BundleofBalloons) //(change recipe to use horsehoe varaints ??)
            .AddIngredient(ItemID.EoCShield)
            .AddIngredient(ItemID.MasterNinjaGear)

            .AddIngredient(ItemID.MinecartMech)
            .AddIngredient(ItemID.BlessedApple)
            .AddIngredient(ItemID.AncientHorn)
            .AddIngredient(ItemID.ReindeerBells)
            .AddIngredient(ItemID.BrainScrambler)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}