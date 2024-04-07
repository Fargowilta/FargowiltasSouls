using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class DestroyerGun2 : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Destruction Cannon");
            // Tooltip.SetDefault("Becomes longer and faster with up to 5 empty minion slots\n'The reward for slaughtering many...'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "毁灭者之枪 EX");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            Item.damage = 275;
            Item.mana = 30;
            Item.DamageType = DamageClass.Summon;
            Item.width = 126;
            Item.height = 38;
            Item.useAnimation = 70;
            Item.useTime = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.NPCDeath13;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DestroyerHead2>();
            Item.shootSpeed = 18f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            FargoSoulsUtil.NewSummonProjectile(source, position, velocity, type, Item.damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "DestroyerGun")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerDestroy"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}