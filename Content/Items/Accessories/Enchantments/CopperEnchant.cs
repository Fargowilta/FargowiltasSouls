using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class CopperEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Copper Enchantment");
			/* Tooltip.SetDefault(
@"Crits have a chance to shock enemies with chain lightning
'Behold'"); */
		}

		public override Color nameColor => new(213, 102, 23);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Blue;
			Item.value = 100000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<CopperEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.CopperHelmet)
				.AddIngredient(ItemID.CopperChainmail)
				.AddIngredient(ItemID.CopperGreaves)
				.AddIngredient(ItemID.CopperShortsword)
				.AddIngredient(ItemID.WandofSparking)
				.AddIngredient(ItemID.ThunderStaff)

			.AddTile(TileID.DemonAltar)
			.Register();
		}
	}
	public class CopperEffect : AccessoryEffect
	{

		public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
		public override int ToggleItemType => ModContent.ItemType<CopperEnchant>();
		public override bool ExtraAttackEffect => true;
		public override void PostUpdateEquips(Player player)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			if (modPlayer.CopperProcCD > 0)
				modPlayer.CopperProcCD--;
		}
		public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
		{
			bool wetCheck = target.HasBuff(BuffID.Wet) && Main.rand.NextBool(4);
			if ((hitInfo.Crit || wetCheck))
			{
				CopperProc(player, target);
			}
		}

		public static void CopperProc(Player player, NPC target)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			if (modPlayer.CopperProcCD == 0)
			{
				bool forceEffect = modPlayer.ForceEffect<CopperEnchant>();
				target.AddBuff(BuffID.Electrified, 180);

				int dmg = 50;
				int maxTargets = 1;
				int cdLength = 300;

				if (forceEffect)
				{
					dmg = 200;
					maxTargets = 5;
					cdLength = 150;
				}

				List<int> npcIndexes = new();
				float closestDist = 500f;
				NPC closestNPC;

				for (int i = 0; i < maxTargets; i++)
				{
					closestNPC = null;

					//find closest npc to target that has not been chained to yet
					for (int j = 0; j < Main.maxNPCs; j++)
					{
						NPC npc = Main.npc[j];

						if (npc.active && npc.whoAmI != target.whoAmI && npc.CanBeChasedBy() && npc.Distance(target.Center) < closestDist && !npcIndexes.Contains(npc.whoAmI)
							&& Collision.CanHitLine(npc.Center, 0, 0, target.Center, 0, 0))
						{
							closestNPC = npc;
							closestDist = npc.Distance(target.Center);
							//break;
						}
					}

					if (closestNPC != null)
					{
						npcIndexes.Add(closestNPC.whoAmI);

						Vector2 ai = closestNPC.Center - target.Center;
						Vector2 velocity = Vector2.Normalize(ai) * 20;

						int damage = FargoSoulsUtil.HighestDamageTypeScaling(modPlayer.Player, dmg);
						FargoSoulsUtil.NewProjectileDirectSafe(modPlayer.Player.GetSource_ItemUse(modPlayer.Player.HeldItem), target.Center, velocity, ModContent.ProjectileType<CopperLightning>(), damage, 0f, modPlayer.Player.whoAmI, ai.ToRotation(), damage);
					}
					else
					{
						break;
					}

					target = closestNPC;
				}

				modPlayer.CopperProcCD = cdLength;
			}
		}
	}
}
