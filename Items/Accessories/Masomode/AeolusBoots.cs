using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AeolusBoots : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aeolus Boots");
            Tooltip.SetDefault(@"Allows flight, super fast running, and extra mobility on ice
The wearer can run even faster on sand
8% increased movement speed
Increases jump speed and allows auto-jump
Flowers grow on the grass you walk on
Allows the holder to double jump
Increases jump height and negates fall damage
'Back and better than ever'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "埃俄罗斯之靴");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"使你获得飞行和快速奔跑能力，提供冰面上的额外机动性
            // 增加8%移动速度
            // 使你获得二段跳能力
            // 增加跳跃高度并免疫摔落伤害
            // '像风一样奔跑'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //terraspark
            player.accRunSpeed = 6.75f;
            player.rocketBoots = player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
            player.moveSpeed += 0.08f;
            player.iceSkate = true;

            //lava wader
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 420;
            player.lavaRose = true;

            //amph boot
            player.autoJump = true;
            player.frogLegJumpBoost = true;

            //fairy boot
            if (!player.flowerBoots && player.GetToggleValue("MasoAeolusFlower"))
            {
                player.flowerBoots = true;
                if (player.whoAmI == Main.myPlayer)
                    player.DoBootsEffect(new Utils.TileActionAttempt(player.DoBootsEffect_PlaceFlowersOnTile));
            }

            //dunerider boot
            player.desertBoots = true;

            //fart balloon
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoAeolus"))
            {
                player.hasJumpOption_Fart = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
            }
            player.jumpBoost = true;
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ZephyrBoots>())
                .AddIngredient(ItemID.TerrasparkBoots)
                .AddIngredient(ItemID.AmphibianBoots)
                .AddIngredient(ItemID.FairyBoots)
                .AddIngredient(ItemID.SandBoots)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
