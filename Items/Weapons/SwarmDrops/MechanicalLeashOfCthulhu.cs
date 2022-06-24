using FargowiltasSouls.Items.Weapons.BossDrops;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class MechanicalLeashOfCthulhu : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Mechanical Leash of Cthulhu");
            Tooltip.SetDefault("'The reward for slaughtering many..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "机械克苏鲁连枷");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 266;
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.knockBack = 6f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.MechFlail>();
            Item.shootSpeed = 50f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LeashOfCthulhu>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerEye"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}