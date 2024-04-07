using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class SpiderEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// Tooltip.SetDefault(tooltip);

			//             string tooltip_ch =
			// @"你的仆从和哨兵现在可以造成暴击且有15%基础暴击率
			// '对恐蛛症者可惩罚他们死于蜘蛛之口'";
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

		}

		public override Color nameColor => new(109, 78, 69);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Pink;
			Item.value = 150000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<SpiderEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.SpiderMask)
			.AddIngredient(ItemID.SpiderBreastplate)
			.AddIngredient(ItemID.SpiderGreaves)
			.AddIngredient(ItemID.SpiderStaff)
			.AddIngredient(ItemID.QueenSpiderStaff)
			.AddIngredient(ItemID.WebSlinger)
			//web rope coil
			//rainbow string
			//fried egg
			//.AddIngredient(ItemID.SpiderEgg);

			.AddTile(TileID.CrystalBall)
			.Register();
		}
	}

	public class SpiderEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
		public override int ToggleItemType => ModContent.ItemType<SpiderEnchant>();
		public override bool MinionEffect => true;
		public override bool IgnoresMutantPresence => true;
		public override void PostUpdateEquips(Player player)
		{
			//minion crits
			player.FargoSouls().MinionCrits = true;

			player.GetCritChance(DamageClass.Summon) += 4;
		}
	}
}
