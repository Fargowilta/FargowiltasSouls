using Fargowiltas.Items.Summons.SwarmSummons.Energizers;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
	public class EaterLauncher : SoulsItem
	{
		public override void SetStaticDefaults()
		{
			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			// DisplayName.SetDefault("Rockeater Launcher");
			// Tooltip.SetDefault("Uses rockets for ammo\n50% chance to not consume ammo\nIncreased damage to enemies in the given range\n'The reward for slaughtering many..'");
			//DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "吞噬者发射器");
			//Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
		}

		public override void SetDefaults()
		{
			Item.damage = 190;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 24;
			Item.height = 24;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 6f;
			Item.UseSound = SoundID.Item62;
			Item.useAmmo = AmmoID.Rocket;
			Item.value = Item.sellPrice(0, 10);
			Item.rare = ItemRarityID.Purple;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<EaterRocket>();
			Item.shootSpeed = 24f;
			Item.scale = .7f;
		}

		//make them hold it different
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-12, -2);
		}
		public const int BaseDistance = 600;
		public const int IncreasedDistance = 1200;
		//public int Cooldown = 0;
		public const int CooldownTime = 24 * 60;
		public override bool AltFunctionUse(Player player) => true;
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				//if (Cooldown > 0)
				//return false;

				Item.useAnimation = 24 * 2;
				Item.useTime = 24 * 2;
				Item.UseSound = SoundID.Item117;
			}
			else
			{
				Item.useAnimation = 24;
				Item.useTime = 24;
				Item.UseSound = SoundID.Item62;
			}
			return base.CanUseItem(player);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				player.FargoSouls().RockeaterDistance = IncreasedDistance;
				//Cooldown = CooldownTime;
				return false;
			}
			for (int i = 0; i < 3; i++)
			{
				Vector2 shotgunVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
				shotgunVelocity *= Main.rand.NextFloat(0.9f, 1.1f);
				Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * Item.width * 0.9f, shotgunVelocity, type, damage, knockback);
			}
			return false;
		}
		public override void HoldItem(Player player)
		{
			if (player.itemTime > 0)
			{
				for (int i = 0; i < 10; i++)
				{
					Vector2 offset = new();
					double angle = Main.rand.NextDouble() * 2d * Math.PI;
					offset.X += (float)(Math.Sin(angle) * 300);
					offset.Y += (float)(Math.Cos(angle) * 300);
					Dust dust = Main.dust[Dust.NewDust(
						player.Center + offset - new Vector2(4, 4), 0, 0,
						DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
						)];
					dust.velocity = player.velocity;
					if (Main.rand.NextBool(3))
						dust.velocity += Vector2.Normalize(offset) * 5f;
					dust.noGravity = true;
					dust.scale = 1f;

					Vector2 offset2 = new();
					double angle2 = Main.rand.NextDouble() * 2d * Math.PI;
					offset2.X += (float)(Math.Sin(angle2) * player.FargoSouls().RockeaterDistance);
					offset2.Y += (float)(Math.Cos(angle2) * player.FargoSouls().RockeaterDistance);
					Dust dust2 = Main.dust[Dust.NewDust(
						player.Center + offset2 - new Vector2(4, 4), 0, 0,
						DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
						)];
					dust2.velocity = player.velocity;
					if (Main.rand.NextBool(3))
						dust2.velocity += Vector2.Normalize(offset2) * -5f;
					dust2.noGravity = true;
					dust2.scale = 1f;
				}
			}

		}


		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = ModContent.ProjectileType<EaterRocket>();
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.NextBool();
		}

		public override void AddRecipes()
		{
			CreateRecipe()

			.AddIngredient(ModContent.ItemType<EaterLauncherJr>())
			.AddIngredient(ModContent.ItemType<EnergizerWorm>())
			.AddIngredient(ItemID.LunarBar, 10)

			.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

			.Register();
		}
	}
}