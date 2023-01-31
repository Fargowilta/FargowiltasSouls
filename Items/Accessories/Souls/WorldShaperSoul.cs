using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Back)]
    public class WorldShaperSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("World Shaper Soul");
            Tooltip.SetDefault(
@"Increased block and wall placement speed by 50%
Near infinite block placement and mining reach
Mining speed tripled
Shows the location of enemies, traps, and treasures
Auto paint and actuator effect
Provides light and allows gravity control
Grants toggleable Builder Mode:
    Tiles and walls will not be consumed
	Increased placement speed and range
Effect can be disabled in Soul Toggles menu
Effects of the Cell Phone and Royal Gel
Summons a pet Magic Lantern
'Limitless possibilities'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铸世者之魂");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
//@"增加50%物块和墙壁的放置速度
//近乎无限的放置和挖掘距离
//挖掘速度x3
//高亮标记敌人、陷阱和宝藏
//自动刷漆和放置促动器
//允许你控制重力且你会散发光芒
//使你获得开启建造模式的能力：
//放置物块时不会消耗物块且大幅增加放置速度
//敌人不会生成
//可以在 魂 选项菜单中禁用此效果
//拥有手机和皇家凝胶效果
//召唤一个魔法灯笼
//'无限的可能性'");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 1;
            Item.UseSound = SoundID.Item6;
            Item.useAnimation = 1;
        }

        protected override Color? nameColor => new Color(255, 239, 2);

        public override bool? UseItem(Player player)
        {
            player.Spawn(PlayerSpawnContext.RecallFromItem);

            for (int num348 = 0; num348 < 70; num348++)
            {
                Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1.5f);
            }

            return true;
        }

        public override void UpdateInventory(Player player)
        {
            //cell phone
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accDreamCatcher = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accCritterGuide = true;
            player.accJarOfSouls = true;
            player.accThirdEye = true;
            player.accCalendar = true;
            player.accWeatherRadio = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.WorldShaperSoul(hideVisual); //add the pet
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            //ancient chisel
            //step stool
            //greedy ring
            //treasure magnet

            .AddIngredient(null, "MinerEnchant")
            .AddIngredient(ItemID.Toolbelt)
            .AddIngredient(ItemID.Toolbox)
            .AddIngredient(ItemID.ArchitectGizmoPack)
            .AddIngredient(ItemID.ActuationAccessory)
            .AddIngredient(ItemID.LaserRuler)
            .AddIngredient(ItemID.RoyalGel)
            .AddIngredient(ItemID.CellPhone)
            //.AddIngredient(ItemID.BloodHamaxe) //haemoraxe
            .AddRecipeGroup("FargowiltasSouls:AnyDrax")
            .AddIngredient(ItemID.ShroomiteDiggingClaw)
            .AddIngredient(ItemID.DrillContainmentUnit)
            //.AddIngredient(ItemID.BallOfFuseWire) //dynamite kitten pet

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
