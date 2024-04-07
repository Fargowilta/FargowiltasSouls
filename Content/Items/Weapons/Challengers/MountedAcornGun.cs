using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class MountedAcornGun : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mounted Acorn Gun");
            // Tooltip.SetDefault("Uses acorns as ammo\n50% chance to not consume ammo\nShoots acorns that sprout on enemies");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "安装好的橡果枪");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使用橡实作为弹药\n50%的几率不消耗弹药\n向敌怪射出会发芽的橡果");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 26;
            Item.useTime = 64;
            Item.useAnimation = 64;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SproutingAcorn>();
            Item.shootSpeed = 16f;

            Item.useAmmo = ItemID.Acorn;
            Item.noMelee = true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<TrojanSquirrelBag>(2).AddTile(TileID.Solidifier).Register();
        }
    }
}