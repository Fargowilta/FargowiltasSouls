using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class EurusSock : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eurus Socks");
            /* Tooltip.SetDefault(
@"The wearer can run pretty fast"); */
            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "欧洛斯之袜");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"穿戴者可跑的非常快");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 2)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}