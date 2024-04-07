using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
	//[AutoloadEquip(EquipType.Waist)]
	public class BerserkerSoul : BaseSoul
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Berserker's Soul");

			//DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "狂战士之魂");

			// Tooltip.SetDefault(tooltip);
			//string tooltip_ch =
			//@"增加30%近战伤害
			//增加20%近战攻速
			//增加15%近战暴击率
			//增加近战击退
			//拥有烈火手套、悠悠球袋和天界壳效果
			//'吾之传说生者弗能传颂'";
			//Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);


		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.defense = 4;
		}
		public static readonly Color ItemColor = new(255, 111, 6);
		protected override Color? nameColor => ItemColor;

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Melee) += 0.3f;
			player.GetCritChance(DamageClass.Melee) += 15;

			player.AddEffect<MeleeSpeedEffect>(Item);

			player.FargoSouls().MeleeSoul = true;

			//gauntlet
			player.AddEffect<MagmaStoneEffect>(Item);
			player.kbGlove = true;
			player.autoReuseGlove = true;
			player.meleeScaleGlove = true;

			player.counterWeight = 556 + Main.rand.Next(6);
			player.yoyoGlove = true;
			player.yoyoString = true;

			//celestial shell
			player.wolfAcc = true;
			player.accMerman = true;

			if (hideVisual)
			{
				player.hideMerman = true;
				player.hideWolf = true;
			}

			player.lifeRegen += 2;
		}

		public override void AddRecipes()
		{
			CreateRecipe()

			.AddIngredient(null, "BarbariansEssence")
			.AddIngredient(ItemID.StingerNecklace)
			.AddIngredient(ItemID.YoyoBag)
			.AddIngredient(ItemID.FireGauntlet)
			.AddIngredient(ItemID.BerserkerGlove)
			.AddIngredient(ItemID.CelestialShell)

			.AddIngredient(ItemID.KOCannon)
			.AddIngredient(ItemID.IceSickle)
			.AddIngredient(ItemID.DripplerFlail)
			.AddIngredient(ItemID.ScourgeoftheCorruptor)
			.AddIngredient(ItemID.Kraken)
			.AddIngredient(ItemID.Flairon)
			.AddIngredient(ItemID.MonkStaffT3)
			.AddIngredient(ItemID.NorthPole)
			.AddIngredient(ItemID.Zenith)

			.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
			.Register();
		}
	}
	public class MeleeSpeedEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
		public override int ToggleItemType => ModContent.ItemType<BerserkerSoul>();
		public override void PostUpdateEquips(Player player)
		{
			player.GetAttackSpeed(DamageClass.Melee) += .2f;
		}
	}
	public class MagmaStoneEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
		public override int ToggleItemType => ItemID.MagmaStone;
		public override void PostUpdateEquips(Player player)
		{
			player.magmaStone = true;
		}
	}
}
