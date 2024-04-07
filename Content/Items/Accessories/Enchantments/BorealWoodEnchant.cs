using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class BorealWoodEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override Color nameColor => new(139, 116, 100);

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Blue;
			Item.value = 10000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<BorealEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.BorealWoodHelmet)
			.AddIngredient(ItemID.BorealWoodBreastplate)
			.AddIngredient(ItemID.BorealWoodGreaves)
			.AddIngredient(ItemID.Shiverthorn)
			.AddIngredient(ItemID.Plum)
			.AddIngredient(ItemID.Snowball, 300)

			.AddTile(TileID.DemonAltar)
			.Register();
		}
	}
	public class BorealEffect : AccessoryEffect
	{

		public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
		public override int ToggleItemType => ModContent.ItemType<BorealWoodEnchant>();
		public override bool ExtraAttackEffect => true;

		public override void PostUpdateEquips(Player player)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			if (modPlayer.BorealCD > 0)
				modPlayer.BorealCD--;
		}
		public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			if (modPlayer.BorealCD <= 0 && player.whoAmI == Main.myPlayer)
			{
				Item item = EffectItem(player);
				bool forceEffect = modPlayer.ForceEffect(item.type);
				modPlayer.BorealCD = forceEffect ? 30 : 60;

				Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
				int snowballDamage = damage / 2;
				if (!modPlayer.TerrariaSoul)
					snowballDamage = Math.Min(snowballDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, forceEffect ? 300 : 30));
				int p = Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, vel, ProjectileID.SnowBallFriendly, snowballDamage, 1, Main.myPlayer);

				int numSnowballs = forceEffect ? 7 : 3;
				if (p != Main.maxProjectiles)
					FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
			}
		}
	}
}
