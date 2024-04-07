using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class Blender : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("The Blender");
            // Tooltip.SetDefault("'The reward for slaughtering many...'");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "绞肉机");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<BlenderYoyoProj>();
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.shootSpeed = 16f;
            Item.knockBack = 2.5f;
            Item.damage = 512;

            Item.value = Item.sellPrice(0, 25);
            Item.rare = ItemRarityID.Purple;
        }

        public override void HoldItem(Player player)
        {
            //player.counterWeight = 556 + Main.rand.Next(6);
            player.stringColor = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "Dicer")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerPlant"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}