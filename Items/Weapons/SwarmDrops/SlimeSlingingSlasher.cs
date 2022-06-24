using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class SlimeSlingingSlasher : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Slime Slinging Slasher");
            Tooltip.SetDefault("'The reward for slaughtering many..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "史莱姆抛射屠戮者");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 340;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlimeBallHoming>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 9;
            for (int i = 0; i < numberProjectiles; i++)
            {
                velocity = velocity.RotatedBy(MathHelper.ToRadians(45) * (Main.rand.NextDouble() - 0.5));
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "SlimeKingsSlasher")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerSlime"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}