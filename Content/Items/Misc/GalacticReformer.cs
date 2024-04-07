using FargowiltasSouls.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public class GalacticReformer : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Galactic Reformer");
            /* Tooltip.SetDefault("Destroys an incredibly massive area\n" +
                                "Use at your own risk"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "银河重构器");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "破坏一片难以置信的巨大区域\n" +
            //"风险自负");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 32;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.value = Item.buyPrice(0, 0, 3);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<GalacticReformerProj>();
            Item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Dynamite, 500)
            .AddTile(TileID.Hellforge)

            .Register();
        }
    }
}