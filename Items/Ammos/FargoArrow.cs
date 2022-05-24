using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Projectiles.Ammos;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Ammos
{
    public class FargoArrow : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amalgamated Arrow Quiver");
            Tooltip.SetDefault("Bounces several times\n" +
                "Each impact explodes, summons falling stars, and fires laser arrows\n" +
                "Inflicts several debuffs");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "混合箭袋");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "弹跳多次\n" +
                "每次撞击都会爆炸,召唤流星,发射激光箭\n" +
                "造成多种Debuff");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.knockBack = 8f; //same as hellfire
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<FargoArrowProj>();
            Item.shootSpeed = 6.5f; // same as hellfire arrow
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.EndlessQuiver)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "FlameQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "FrostburnQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "UnholyQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "BoneQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "JesterQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "HellfireQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "CursedQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "IchorQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "HolyQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "VenomQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "ChlorophyteQuiver").Type)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "LuminiteQuiver").Type)
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 15)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
}