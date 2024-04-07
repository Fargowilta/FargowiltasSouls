using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class AncientHallowEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Ancient Hallowed Enchantment");
			/* Tooltip.SetDefault(
@"You gain a shield that can reflect projectiles
Summons a Terraprisma familiar that scales with minion damage
'Have you power enough to wield me?'"); */
		}

		public override Color nameColor => new(150, 133, 100);

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.LightPurple;
			Item.value = 180000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			AddEffects(player, Item);
		}

		public static void AddEffects(Player player, Item item)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();

			bool minion = player.AddEffect<AncientHallowMinion>(item);
			int damage = modPlayer.ForceEffect<AncientHallowEnchant>() ? 600 : 350;
			modPlayer.AddMinion(item, minion, ModContent.ProjectileType<HallowSword>(), damage, 2);
		}

		public static Color GetFairyQueenWeaponsColor(float alphaChannelMultiplier, float lerpToWhite, float rawHueOverride)
		{
			float num = rawHueOverride;

			float num2 = (num + 0.5f) % 1f;
			float saturation = 1f;
			float luminosity = 0.5f;

			Color color3 = Main.hslToRgb(num2, saturation, luminosity, byte.MaxValue);
			//color3 *= this.Opacity;
			if (lerpToWhite != 0f)
			{
				color3 = Color.Lerp(color3, Color.White, lerpToWhite);
			}
			color3.A = (byte)(color3.A * alphaChannelMultiplier);
			return color3;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup("FargowiltasSouls:AnyAncientHallowHead")
				.AddIngredient(ItemID.AncientHallowedPlateMail)
				.AddIngredient(ItemID.AncientHallowedGreaves)
				.AddIngredient(ItemID.RainbowRod)
				.AddIngredient(ItemID.SwordWhip) //durendal
				.AddIngredient(ItemID.HolyWater, 50)
				.AddTile(TileID.CrystalBall)
				.Register();
		}
	}
	public class AncientHallowMinion : AccessoryEffect
	{
		public override int ToggleItemType => ModContent.ItemType<AncientHallowEnchant>();
		public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
		public override bool MinionEffect => true;
	}
}
