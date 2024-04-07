using FargowiltasSouls.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class NekomiLeggings : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Nekomi Leggings");
            /* Tooltip.SetDefault(@"7% increased critical strike chance
10% increased movement speed"); */
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 7;
            player.moveSpeed += 0.10f;
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