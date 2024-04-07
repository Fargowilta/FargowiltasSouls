using FargowiltasSouls.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class NekomiHoodie : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Nekomi Hoodie");
            /* Tooltip.SetDefault(@"Slightly increases your life regeneration
Reduces damage taken by 5%"); */
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.defense = 11;
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 1;
            player.endurance += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 5)
            .AddTile(TileID.Loom)

            .Register();
        }
    }
}