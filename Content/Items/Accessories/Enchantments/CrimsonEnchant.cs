using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class CrimsonEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Crimson Enchantment");
			/* Tooltip.SetDefault(
@"After taking a hit, regen is greatly increased until half the hit is healed off
If you take another hit before it's healed, the heal ends early
This does not affect hits dealing less than 10 damage
'The blood of your enemy is your rebirth'"); */
		}

		public override Color nameColor => new(200, 54, 75);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Green;
			Item.value = 50000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<CrimsonEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.CrimsonHelmet)
			.AddIngredient(ItemID.CrimsonScalemail)
			.AddIngredient(ItemID.CrimsonGreaves)
			.AddIngredient(ItemID.TheUndertaker)
			.AddIngredient(ItemID.TheMeatball)
			.AddIngredient(ItemID.CrimsonHeart)

			.AddTile(TileID.DemonAltar)
			.Register();
		}
	}
	public class CrimsonEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
		public override int ToggleItemType => ModContent.ItemType<CrimsonEnchant>();
		public override void OnHurt(Player player, Player.HurtInfo info)
		{
			//if was already healing, stop the heal and do nothing
			if (player.HasBuff<CrimsonRegenBuff>())
			{
				player.ClearBuff(ModContent.BuffType<CrimsonRegenBuff>());
			}
			else
			{
				FargoSoulsPlayer modPlayer = player.FargoSouls();
				if (info.Damage < 10)
					return; //ignore hits under 10 damage
				modPlayer.CrimsonRegenTime = 0; //reset timer
				float returnHeal = 0.5f; //% of damage given back
				modPlayer.CrimsonRegenAmount = (int)(info.Damage * returnHeal); //50% return heal

				player.AddBuff(ModContent.BuffType<CrimsonRegenBuff>(),
					modPlayer.ForceEffect<CrimsonEnchant>() ? 900 : 430); //should never reach that time lol. buff gets removed in buff itself after its done. sets to actual time so that it shows in buff properly
			}
		}
	}
}
