using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
	public class UniverseCore : SoulsItem
	{
		public override int NumFrames => 5;

		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(10, NumFrames));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(0, 50);

			//Item.scale *= 0.5f;

			Item.expert = true;
		}

		//public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.FargoSouls().UniverseCore = true;
		}
	}
}