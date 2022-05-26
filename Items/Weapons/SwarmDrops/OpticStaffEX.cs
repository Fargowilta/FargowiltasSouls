using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class OpticStaffEX : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Omniscience Staff");
            Tooltip.SetDefault("Summons the real twins to fight for you\nNeeds 3 minion slots\n'The reward for slaughtering many...'");
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 4;
        }

        public override void SetDefaults()
        {
            Item.damage = 295;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.width = 24;
            Item.height = 24;
            Item.useAnimation = 37;
            Item.useTime = 37;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item82;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ItemRarityID.Purple;
            Item.buffType = ModContent.BuffType<TwinsEX>();
            Item.shoot = ModContent.ProjectileType<OpticRetinazer>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Vector2 spawnPos = Main.MouseWorld;
            velocity = velocity.RotatedBy(Math.PI / 2);

            player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<OpticRetinazer>(), Item.damage, knockback, default, velocity);
            player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<OpticSpazmatism>(), Item.damage, knockback, default, -velocity);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.OpticStaff)
            .AddIngredient(null, "TwinRangs")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerTwins"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}