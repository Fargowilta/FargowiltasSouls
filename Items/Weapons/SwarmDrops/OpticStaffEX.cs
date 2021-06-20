using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class OpticStaffEX : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omniscience Staff");
            Tooltip.SetDefault("Summons the real twins to fight for you\nNeeds 4 minion slots\n'The reward for slaughtering many...'");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 4;
                        DisplayName.AddTranslation(GameCulture.Chinese, "全知权杖");
            Tooltip.AddTranslation(GameCulture.Chinese, "'召唤真正的双子魔眼为你战斗\n需要4召唤栏\n'屠戮众多的奖励……'");
        }

        public override void SetDefaults()
        {
            item.damage = 265;
            item.mana = 10;
            item.summon = true;
            item.width = 24;
            item.height = 24;
            item.useAnimation = 37;
            item.useTime = 37;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item82;
            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
            item.buffType = mod.BuffType("TwinsEX");
            item.shoot = mod.ProjectileType("OpticRetinazer");
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            Vector2 spawnPos = Main.MouseWorld;
            Vector2 speed = new Vector2(speedX, speedY).RotatedBy(Math.PI / 2);
            Projectile.NewProjectile(spawnPos, speed, mod.ProjectileType("OpticRetinazer"), damage, knockBack, player.whoAmI, -1);
            Projectile.NewProjectile(spawnPos, -speed, mod.ProjectileType("OpticSpazmatism"), damage, knockBack, player.whoAmI, -1);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.OpticStaff);
            recipe.AddIngredient(null, "TwinRangs");
            recipe.AddIngredient(null, "MutantScale", 10);
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerTwins"));
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
