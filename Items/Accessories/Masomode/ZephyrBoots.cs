using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    //[AutoloadEquip(EquipType.Shoes)] //TODO: enable this when sheeted
    public class ZephyrBoots : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zephyr Boots");
            Tooltip.SetDefault(@"Allows flight and super fast running
8% increased movement speed
Allows the holder to double jump
Increases jump height and negates fall damage
'Run like the wind'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //lightning boots
            player.accRunSpeed = 6.75f;
            player.rocketBoots = player.vanityRocketBoots = ArmorIDs.RocketBoots.SpectreBoots;
            player.moveSpeed += 0.08f;

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
                .AddIngredient(ItemID.LightningBoots)
                .AddIngredient(ItemID.BalloonHorseshoeFart)
                .AddIngredient(ModContent.ItemType<EurusSock>())
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
