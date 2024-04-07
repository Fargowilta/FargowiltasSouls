using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class SnowEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Snow Enchantment");
			/* Tooltip.SetDefault(
@"Your attacks briefly inflict Frostburn
Press the Freeze Key to chill everything for 15 seconds
There is a 60 second cooldown for this effect
'It's Burning Cold Outside'"); */
		}

		public override Color nameColor => new(37, 195, 242);

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Blue;
			Item.value = 50000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<SnowEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()

			.AddIngredient(ItemID.EskimoHood)
			.AddIngredient(ItemID.EskimoCoat)
			.AddIngredient(ItemID.EskimoPants)
			//hand warmer
			//fruitcake chakram
			.AddIngredient(ItemID.IceBoomerang)
			.AddIngredient(ItemID.FrostMinnow)
			.AddIngredient(ItemID.AtlanticCod)

			.AddTile(TileID.DemonAltar)
			.Register();

			CreateRecipe()

			.AddIngredient(ItemID.EskimoHood)
			.AddIngredient(ItemID.EskimoCoat)
			.AddIngredient(ItemID.EskimoPants)
			//hand warmer
			//fruitcake chakram
			.AddIngredient(ItemID.IceBlade)
			.AddIngredient(ItemID.FrostMinnow)
			.AddIngredient(ItemID.AtlanticCod)

			.AddTile(TileID.DemonAltar)
			.Register();
		}
	}
	public class SnowEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
		public override int ToggleItemType => ModContent.ItemType<SnowEnchant>();
		public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
		{
			target.AddBuff(BuffID.Frostburn, 120);
		}
		public override void PostUpdateEquips(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				FargoSoulsPlayer modPlayer = player.FargoSouls();
				int maxIcicles = player.HasEffect<FrostEffect>() ? 10 : 5;
				int type = ModContent.ProjectileType<FrostIcicle>();
				if (modPlayer.icicleCD <= 0 && modPlayer.IcicleCount < maxIcicles && player.ownedProjectileCounts[type] < maxIcicles)
				{
					modPlayer.IcicleCount++;

					//kill all current ones
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile proj = Main.projectile[i];

						if (proj.active && proj.type == type && proj.owner == player.whoAmI)
						{
							proj.active = false;
							proj.netUpdate = true;
						}
					}

					//respawn in formation
					for (int i = 0; i < modPlayer.IcicleCount; i++)
					{
						float radians = 360f / modPlayer.IcicleCount * i * (float)(Math.PI / 180);
						FargoSoulsUtil.NewProjectileDirectSafe(GetSource_EffectItem(player), player.Center, Vector2.Zero, type, 0, 0f, player.whoAmI, 5, radians);
					}

					float dustScale = 1.5f;

					if (modPlayer.IcicleCount % maxIcicles == 0)
					{
						dustScale = 3f;
					}

					//dust
					for (int j = 0; j < 20; j++)
					{
						Vector2 vector6 = Vector2.UnitY * 5f;
						vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
						Vector2 vector7 = vector6 - player.Center;
						int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.MagicMirror);
						Main.dust[d].noGravity = true;
						Main.dust[d].scale = dustScale;
						Main.dust[d].velocity = vector7;

						if (modPlayer.IcicleCount % maxIcicles == 0)
						{
							Main.dust[d].velocity *= 2;
						}
					}

					modPlayer.icicleCD = 30;
				}

				if (modPlayer.icicleCD > 0)
					modPlayer.icicleCD--;

				if (modPlayer.IcicleCount >= 1 && player.controlUseItem && player.HeldItem.IsWeapon() && player.HeldItem.createTile == -1 && player.HeldItem.createWall == -1 && player.HeldItem.ammo == AmmoID.None)
				{

					int dmg = modPlayer.ForceEffect<FrostEnchant>() ? 100 : (player.HasEffect<FrostEffect>() ? 50 : 20);

					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile proj = Main.projectile[i];

						if (proj.active && proj.type == type && proj.owner == player.whoAmI)
						{
							bool stayFrosty = player.HasEffect<FrostEffect>();
							Vector2 vel = (Main.MouseWorld - proj.Center).SafeNormalize(-Vector2.UnitY);
							vel *= stayFrosty ? 20f : 10f;
							int attackType = stayFrosty ? ProjectileID.Blizzard : ProjectileID.SnowBallFriendly;
							int p = Projectile.NewProjectile(GetSource_EffectItem(player), proj.Center, vel, attackType, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 1f, player.whoAmI);
							if (p != Main.maxProjectiles)
							{
								Main.projectile[p].FargoSouls().CanSplit = false;
								Main.projectile[p].FargoSouls().FrostFreeze = true;
							}

							proj.Kill();
						}
					}

					modPlayer.IcicleCount = 0;
					modPlayer.icicleCD = 120;
				}
			}
		}
	}
}
