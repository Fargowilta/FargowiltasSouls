using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Dyes
{
    public class LifeDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heavenly Dye");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50);
        }
    }

    public class WillDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Willpower Dye");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50);
        }
    }

    public class GaiaDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gaia Dye");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ItemID.BeetleHusk)
            .AddIngredient(ItemID.ShroomiteBar)
            .AddIngredient(ItemID.SpectreBar)
            .AddIngredient(ItemID.SpookyWood)
            .AddTile(TileID.DyeVat)

            .Register();
        }
    }
}