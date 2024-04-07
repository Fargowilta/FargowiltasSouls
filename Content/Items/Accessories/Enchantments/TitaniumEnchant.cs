using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class TitaniumEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Titanium Enchantment");
			/* Tooltip.SetDefault(
@"Attacking generates a defensive barrier of up to 20 titanium shards
When you reach the maximum amount, you gain resistance to most debuffs on hit and 50% damage resistance against contact damage and projectiles in close range
This has a cooldown of 10 seconds during which you cannot gain shards
'The power of titanium in the palm of your hand'"); */
		}

		public override Color nameColor => new(130, 140, 136);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Pink;
			Item.value = 100000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<TitaniumEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
				.AddIngredient(ItemID.TitaniumBreastplate)
				.AddIngredient(ItemID.TitaniumLeggings)
				.AddIngredient(ItemID.Chik)
				.AddIngredient(ItemID.CrystalStorm)
				.AddIngredient(ItemID.CrystalVileShard)

				.AddTile(TileID.CrystalBall)
				.Register();
		}
	}

	public class TitaniumEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
		public override int ToggleItemType => ModContent.ItemType<TitaniumEnchant>();

		public override float ContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers)
		{
			return base.ContactDamageDR(player, npc, ref modifiers);
		}
		public override float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
		{
			return TitaniumDR(player, projectile);
		}
		public static float TitaniumDR(Player player, Entity attacker)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();

			if (!modPlayer.TitaniumDRBuff)
				return 0;

			bool canUseDR = attacker is NPC ||
				attacker is Projectile projectile && projectile.GetSourceNPC() is NPC sourceNPC
				&& player.Distance(sourceNPC.Center) < Math.Max(sourceNPC.width, sourceNPC.height) + 16 * 8;

			if (canUseDR)
			{
				float diff = 1f - player.endurance;
				diff *= modPlayer.ForceEffect<TitaniumEnchant>() ? 0.35f : 0.25f;
				return diff;
			}
			return 0;
		}

		public static void TitaniumShards(FargoSoulsPlayer modPlayer, Player player)
		{

			if (modPlayer.TitaniumCD)
				return;

			player.AddBuff(306, 600, true, false);
			if (player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] < 20)
			{
				int damage = 50;
				if (modPlayer.ForceEffect(player.EffectItem<TitaniumEffect>().ModItem))
				{
					damage = FargoSoulsUtil.HighestDamageTypeScaling(player, damage);
				}

				Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<TitaniumEffect>()), player.Center, Vector2.Zero, ProjectileID.TitaniumStormShard /*ModContent.ProjectileType<TitaniumShard>()*/, damage, 15f, player.whoAmI, 0f, 0f);
			}
			else
			{
				if (!player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()))
				{
					//dust ring
					for (int j = 0; j < 20; j++)
					{
						Vector2 vector6 = Vector2.UnitY * 5f;
						vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
						Vector2 vector7 = vector6 - player.Center;
						int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Titanium);
						Main.dust[d].noGravity = true;
						Main.dust[d].scale = 1.5f;
						Main.dust[d].velocity = vector7;
					}
				}

				int buffDuration = 240;
				player.AddBuff(ModContent.BuffType<TitaniumDRBuff>(), buffDuration);
			}
		}

		public override void PostUpdateMiscEffects(Player player)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();

			if (modPlayer.TitaniumDRBuff && modPlayer.prevDyes == null)
			{
				modPlayer.prevDyes = new List<int>();
				int reflectiveSilver = GameShaders.Armor.GetShaderIdFromItemId(ItemID.ReflectiveSilverDye);

				for (int i = 0; i < player.dye.Length; i++)
				{
					modPlayer.prevDyes.Add(player.dye[i].dye);
					player.dye[i].dye = reflectiveSilver;
				}

				for (int j = 0; j < player.miscDyes.Length; j++)
				{
					modPlayer.prevDyes.Add(player.miscDyes[j].dye);
					player.miscDyes[j].dye = reflectiveSilver;
				}

				player.UpdateDyes();
			}
			else if (!player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()) && modPlayer.prevDyes != null)
			{
				for (int i = 0; i < player.dye.Length; i++)
				{
					player.dye[i].dye = modPlayer.prevDyes[i];
				}

				for (int j = 0; j < player.miscDyes.Length; j++)
				{
					player.miscDyes[j].dye = modPlayer.prevDyes[j + player.dye.Length];
				}

				player.UpdateDyes();

				modPlayer.prevDyes = null;
			}
		}
	}
}
