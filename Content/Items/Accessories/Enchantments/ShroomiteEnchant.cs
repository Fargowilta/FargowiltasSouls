using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class ShroomiteEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Shroomite Enchantment");

			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "蘑菇魔石");

			// Tooltip.SetDefault(tooltip);

			//             string tooltip_ch =
			// @"所有攻击都会留下蘑菇尾迹
			// 站定不动时使你进入隐身状态
			// 处于隐身状态时攻击会留下更多蘑菇尾迹
			// '真的是用蘑菇做的！'";
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

		}

		public override Color nameColor => new(0, 140, 244);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Lime;
			Item.value = 250000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<ShroomiteStealthEffect>(Item);
			player.AddEffect<ShroomiteShroomEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddRecipeGroup("FargowiltasSouls:AnyShroomHead")
			.AddIngredient(ItemID.ShroomiteBreastplate)
			.AddIngredient(ItemID.ShroomiteLeggings)
			//shroomite digging
			//hammush
			.AddIngredient(ItemID.MushroomSpear)
			.AddIngredient(ItemID.Uzi)
			//venus magnum
			.AddIngredient(ItemID.TacticalShotgun)
			//.AddIngredient(ItemID.StrangeGlowingMushroom);

			.AddTile(TileID.CrystalBall)
			.Register();
		}
	}
	public class ShroomiteStealthEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
		public override int ToggleItemType => ModContent.ItemType<ShroomiteEnchant>();
		public override void PostUpdateEquips(Player player)
		{
			if (!player.FargoSouls().TerrariaSoul)
				player.shroomiteStealth = true;
		}
	}
	public class ShroomiteShroomEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
		public override int ToggleItemType => ModContent.ItemType<ShroomiteEnchant>();
		public override bool ExtraAttackEffect => true;
		public override void MeleeEffects(Player player, Item item, Rectangle hitbox)
		{
			Player Player = player; //too lazy
			if (!item.noMelee && (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.1) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.3) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.5) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.9)))
			{
				//hellish code from hammush
				float num340 = 0f;
				float num341 = 0f;
				float num342 = 0f;
				float num343 = 0f;
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.9))
				{
					num340 = -7f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7))
				{
					num340 = -6f;
					num341 = 2f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.5))
				{
					num340 = -4f;
					num341 = 4f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.3))
				{
					num340 = -2f;
					num341 = 6f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.1))
				{
					num341 = 7f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7))
				{
					num343 = 26f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.3))
				{
					num343 -= 4f;
					num342 -= 20f;
				}
				if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.1))
				{
					num342 += 6f;
				}
				if (Player.direction == -1)
				{
					if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.9))
					{
						num343 -= 8f;
					}
					if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7))
					{
						num343 -= 6f;
					}
				}
				num340 *= 1.5f;
				num341 *= 1.5f;
				num343 *= (float)Player.direction;
				num342 *= Player.gravDir;
				Projectile.NewProjectile(Player.GetSource_ItemUse(item), (float)(hitbox.X + hitbox.Width / 2) + num343, (float)(hitbox.Y + hitbox.Height / 2) + num342, (float)Player.direction * num341, num340 * Player.gravDir, ModContent.ProjectileType<ShroomiteShroom>(), item.damage / 4, 0f, Player.whoAmI, 0f, 0);
			}
		}
	}
}
