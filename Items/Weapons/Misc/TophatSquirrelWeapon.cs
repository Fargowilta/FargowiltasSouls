using FargowiltasSouls.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.Misc
{
    public class TophatSquirrelWeapon : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Top Hat Squirrel");
            Tooltip.SetDefault("'Who knew this squirrel had phenomenal cosmic power?'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "高顶礼帽松鼠");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'谁能知道,这只松鼠竟然有着非凡的宇宙力量呢?'");
        }

        public override void SetDefaults()
        {
            Item.damage = 22;

            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 45;
            Item.useTime = 45;

            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.6f;

            Item.mana = 66;

            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<TopHatSquirrelProj>();
            Item.shootSpeed = 8f;

            Item.value = Item.sellPrice(0, 20);
        }

        //public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{
        //    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

        //    return false;
        //}

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "Squirrel"), 10)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddIngredient(ItemID.SoulofFright, 3)
            .AddIngredient(ItemID.SoulofSight, 3)
            .AddIngredient(ItemID.SoulofMight, 3)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddTile(TileID.MythrilAnvil)


            .Register();
        }
    }
}
