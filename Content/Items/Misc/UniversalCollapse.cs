using FargowiltasSouls.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public class UniversalCollapse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Universal Collapse");
            // Tooltip.SetDefault("Destroys the Universe");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "宇宙坍缩");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "毁灭宇宙");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 32;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.expert = true;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.value = Item.buyPrice(0, 0, 3);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<UniversalCollapseProj>();
            Item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GalacticReformer>(), 100)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}