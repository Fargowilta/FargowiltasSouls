using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
	public abstract class BaseForce : SoulsItem
	{
		public static int[] EnchantsIn<T>() where T : BaseForce => Enchants[ModContent.ItemType<T>()];
		public void SetActive(Player player) => player.FargoSouls().ForceEffects.Add(Type);
		/// <summary>
		/// IDs for the Enchants contained of each Force type. <para/>
		/// Set in SetStaticDefaults.
		/// </summary>
		public static Dictionary<int, int[]> Enchants = new();
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			ItemID.Sets.ItemNoGravity[Type] = true;
			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Purple;
			Item.value = 600000;
		}
	}
}
