using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class EaterLauncher : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rockeater Launcher");
            Tooltip.SetDefault("Uses rockets for ammo\n50% chance to not consume ammo\nIncreased damage to enemies in the given range\n'The reward for slaughtering many..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "吞噬者发射器");
            Tooltip.AddTranslation(GameCulture.Chinese, "使用火箭作为弹药\n有50%的几率不消耗弹药\n提高对在光环内的敌人造成的伤害\n'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            item.damage = 315; //
            item.ranged = true;
            item.width = 24;
            item.height = 24;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.UseSound = new LegacySoundStyle(2, 62);
            item.useAmmo = AmmoID.Rocket;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("EaterRocket");
            item.shootSpeed = 16f;
            item.scale = .7f;
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, -2);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = mod.ProjectileType("EaterRocket");
            return true;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.Next(2) == 0;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("EaterStaff"));
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerWorm"));
            recipe.AddIngredient(ItemID.LunarBar, 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
