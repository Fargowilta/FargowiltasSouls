using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AeolusBoots : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Aeolus Boots");
            Tooltip.SetDefault(@"Allows flight, super fast running, and extra mobility on ice
8% increased movement speed
Allows the holder to double jump
Increases jump height and negates fall damage
'Run like the wind'");
            DisplayName.AddTranslation(GameCulture.Chinese, "埃俄罗斯之靴");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你获得飞行和快速奔跑能力，提供冰面上的额外机动性
增加8%移动速度
使你获得二段跳能力
增加跳跃高度并免疫摔落伤害
'像风一样奔跑'");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 6.75f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.08f;
            player.iceSkate = true;
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoAeolus"))
            {
                player.doubleJumpFart = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
            }
            player.jumpBoost = true;
            player.noFallDmg = true;

            //add effects
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.FrostsparkBoots); //terraspark
            //amphibian
            //fairy boots
            //dunerider
            recipe.AddIngredient(ItemID.BalloonHorseshoeFart);
            recipe.AddIngredient(mod.ItemType("EurusSock"));
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
