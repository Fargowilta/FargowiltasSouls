using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class SlimeSword : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Slinging Slasher");
            Tooltip.SetDefault("'The reward for slaughtering many..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "史莱姆抛射屠戮者");
            Tooltip.AddTranslation(GameCulture.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            item.damage = 295;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("SlimeBallHoming");
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
        {
            int numberProjectiles = 9;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 velocity = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(30) * (Main.rand.NextDouble() - 0.5));
                Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SlimeKingsSlasher");
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerSlime"));
            recipe.AddIngredient(ItemID.LunarBar, 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}