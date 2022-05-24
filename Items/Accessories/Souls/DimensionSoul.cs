using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class DimensionSoul : FlightMasteryWings
    {
        protected override bool HasSupersonicSpeed => true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Soul of Dimensions");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "维度之魂");

            String tooltip =
@"Increases HP by 300
20% damage reduction
Increases life regeneration by 8
Grants immunity to knockback and several debuffs
Enemies are more likely to target you
Allows Supersonic running and infinite flight
Increases fishing skill substantially, All fishing rods will have 10 extra lures
Increased block and wall placement speed by 50%
Near infinite block placement and mining reach, Mining speed tripled
Shine, Spelunker, Hunter, and Dangersense effects
Auto paint and actuator effect
Grants the ability to enable Builder Mode
Effects of the Brain of Confusion, Star Veil, Sweetheart Necklace, Bee Cloak, Spore Sac, and Shiny Stone
Effects of Paladin's Shield, Frozen Turtle Shell, Arctic Diving Gear, Frog Legs, and Flying Carpet
Effects of Lava Waders, Angler Tackle Bag, Paint Sprayer, Presserator, Cell Phone, and Gravity Globe
Effects of Shield of Cthulhu and Master Ninja Gear
'The dimensions of Terraria at your fingertips'";
            Tooltip.SetDefault(tooltip);

            String tooltip_ch =
@"增加300点最大生命值
增加20%伤害减免
增加8点生命恢复速度
使你免疫击退和一些减益
增加敌人以你为目标的几率
使你获得超音速奔跑和无限飞行能力
大幅增加渔力且钓竿会额外扔出10根鱼线
增加50%物块和墙壁的放置速度
近乎无限的放置和挖掘距离，挖掘速度x3
拥有光芒、洞穴探险、狩猎和危险感效果
自动刷漆和放置促动器
使你获得开启建造模式的能力
拥有混乱之脑、星星面纱、甜蜜项链、蜜蜂斗篷、孢子囊和闪亮石效果
拥有圣骑士护盾、冰冻海龟壳、北极潜水装备、蛙腿和飞毯效果
拥有熔岩靴、渔夫渔具袋、喷漆器、自动安放器、手机和重力球效果
拥有克苏鲁护盾和忍者大师装备效果
'泰拉瑞亚的维度触手可及'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 18));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override int NumFrames => 18;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.defense = 15;
            Item.value = 5000000;
            Item.rare = -12;
            Item.expert = true;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 1;
            Item.UseSound = SoundID.Item6;
            Item.useAnimation = 1;
        }

        public override bool? UseItem(Player player)
        {
            player.Spawn(PlayerSpawnContext.RecallFromItem);

            for (int num348 = 0; num348 < 70; num348++)
            {
                Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1.5f);
            }

            return base.UseItem(player);
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
            modPlayer.ColossusSoul(Item, 300, 0.2f, 8, hideVisual);
            modPlayer.SupersonicSoul(Item, hideVisual);
            modPlayer.FlightMasterySoul();
            modPlayer.TrawlerSoul(Item, hideVisual);
            modPlayer.WorldShaperSoul(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(null, "ColossusSoul")
            .AddIngredient(null, "SupersonicSoul")
            .AddIngredient(null, "FlightMasterySoul")
            .AddIngredient(null, "TrawlerSoul")
            .AddIngredient(null, "WorldShaperSoul")
            .AddIngredient(null, "AbomEnergy", 10)
            //.AddIngredient(ItemID.BoneKey);

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
