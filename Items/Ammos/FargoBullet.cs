using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Projectiles.Ammos;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Ammos
{
    public class FargoBullet : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amalgamated Bullet Pouch");
            Tooltip.SetDefault("Chases after your enemy\n" +
                               "Bounces several times\n" +
                               "Each impact causes an explosion of crystal shards\n" +
                               "Inflicts several debuffs");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "混合子弹袋");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
                               //"追踪敌人\n" +
                               //"弹跳多次\n" +
                               //"每次撞击都会造成魔晶碎片爆炸\n" +
                               //"造成多种Debuff");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.knockBack = 4f; //same as explosive
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<FargoBulletProj>();
            Item.shootSpeed = 15f; // same as high velocity bullets
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.EndlessMusketPouch)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "SilverPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "MeteorPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "CursedPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "IchorPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "CrystalPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "VelocityPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "VenomPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "ExplosivePouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "GoldenPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "PartyPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "ChlorophytePouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "NanoPouch").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "LuminitePouch").Type)
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 15)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
}