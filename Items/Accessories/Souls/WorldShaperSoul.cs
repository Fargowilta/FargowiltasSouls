using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Back)]
    public class WorldShaperSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("World Shaper Soul");
            Tooltip.SetDefault(
@"Increased block and wall placement speed by 50%
Near infinite block placement and mining reach
Mining speed tripled
Shows the location of enemies, traps, and treasures
Auto paint and actuator effect
Provides light and allows gravity control
Grants the ability to enable Builder Mode:
Anything that creates a tile will not be consumed and can be used much faster
No enemies can spawn
Effect can be disabled in Soul Toggles menu
Effects of the Cell Phone and Royal Gel
Summons a pet Magic Lantern
'Limitless possibilities'");
            DisplayName.AddTranslation(GameCulture.Chinese, "铸世者之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"增加50%物块和墙壁的放置速度
近乎无限的放置和挖掘距离
挖掘速度x3
高亮标记敌人、陷阱和宝藏
自动刷漆和放置促动器
允许你控制重力且你会散发光芒
使你获得开启建造模式的能力：
放置物块时不会消耗物块且大幅增加放置速度
敌人不会生成
可以在 魂 选项菜单中禁用此效果
拥有手机和皇家凝胶效果
召唤一个魔法灯笼
'无限的可能性'");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 750000;
            item.rare = ItemRarityID.Purple;

            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTime = 1;
            item.UseSound = SoundID.Item6;
            item.useAnimation = 1;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(255, 239, 2));
                }
            }
        }

        public override bool UseItem(Player player)
        {
            player.Spawn();

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
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.WorldShaperSoul(hideVisual); //add the pet
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            //ancient chisel
            //step stool
            //greedy ring
            //treasure magnet

            recipe.AddIngredient(null, "MinerEnchant");
            recipe.AddIngredient(ItemID.Toolbelt);
            recipe.AddIngredient(ItemID.Toolbox);
            recipe.AddIngredient(ItemID.ArchitectGizmoPack);
            recipe.AddIngredient(ItemID.ActuationAccessory);
            recipe.AddIngredient(ItemID.LaserRuler);
            recipe.AddIngredient(ItemID.RoyalGel);
            recipe.AddIngredient(ItemID.CellPhone);
            //haemoraxe
            recipe.AddRecipeGroup("FargowiltasSouls:AnyDrax");
            recipe.AddIngredient(ItemID.ShroomiteDiggingClaw);
            recipe.AddIngredient(ItemID.DrillContainmentUnit);
            //dynamite kitten pet

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
