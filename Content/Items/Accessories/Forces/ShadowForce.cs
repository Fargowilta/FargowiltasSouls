using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
	public class ShadowForce : BaseForce
	{
		public override void SetStaticDefaults()
		{
			Enchants[Type] = new int[]
			{
				ModContent.ItemType<NinjaEnchant>(),
				ModContent.ItemType<AncientShadowEnchant>(),
				ModContent.ItemType<CrystalAssassinEnchant>(),
				ModContent.ItemType<SpookyEnchant>(),
				ModContent.ItemType<ShinobiEnchant>(),
				ModContent.ItemType<DarkArtistEnchant>(),
				ModContent.ItemType<NecroEnchant>()
			};
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SetActive(player);
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			player.AddEffect<NinjaEffect>(Item);
			modPlayer.ApprenticeEnchantActive = true;
			modPlayer.DarkArtistEnchantActive = true;
			player.AddEffect<ApprenticeSupport>(Item);
			player.AddEffect<DarkArtistMinion>(Item);
			player.AddEffect<NecroEffect>(Item);
			//shadow orbs
			modPlayer.AncientShadowEnchantActive = true;
			player.AddEffect<ShadowBalls>(Item);
			//darkness debuff
			player.AddEffect<AncientShadowDarkness>(Item);
			//shinobi and monk effects
			ShinobiEnchant.AddEffects(player, Item);
			//smoke bomb nonsense
			CrystalAssassinEnchant.AddEffects(player, Item);
			//scythe doom
			player.AddEffect<SpookyEffect>(Item);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			foreach (int ench in Enchants[Type])
				recipe.AddIngredient(ench);
			recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
			recipe.Register();
		}
	}
}
