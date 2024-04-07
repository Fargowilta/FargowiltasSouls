using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class SnowballStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Snowball Effect");
            // Tooltip.SetDefault("Creates a snowball that grows as you roll it\nMust use continuously to sustain snowball\nRight click to recall the snowball to yourself");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "雪球法杖");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "制造一个会越滚越大的雪球，你可以通过主动推雪球来滚它\n长按左键才能维持雪球\n右键点击可让雪球回到你身上");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RollingSnowball>();
            Item.shootSpeed = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.mana = 5;
        }

        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<TrojanSquirrelBag>(2).AddTile(TileID.Solidifier).Register();
        }
    }
}
