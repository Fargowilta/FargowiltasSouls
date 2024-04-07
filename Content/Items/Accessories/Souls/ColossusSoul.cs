using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
	//[AutoloadEquip(EquipType.Shield)]
	public class ColossusSoul : BaseSoul
	{

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.defense = 10;
			Item.shieldSlot = 4;
		}
		public static readonly Color ItemColor = new(252, 59, 0);
		protected override Color? nameColor => ItemColor;

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			AddEffects(player, Item, 100, 0.15f, 5);
		}
		public static void AddEffects(Player player, Item item, int maxHP, float damageResist, int lifeRegen)
		{
			Player Player = player;
			player.FargoSouls().ColossusSoul = true;
			Player.statLifeMax2 += maxHP;
			Player.endurance += damageResist;
			Player.lifeRegen += lifeRegen;

			Player.buffImmune[BuffID.Chilled] = true;
			Player.buffImmune[BuffID.Frozen] = true;
			Player.buffImmune[BuffID.Stoned] = true;
			Player.buffImmune[BuffID.Weak] = true;
			Player.buffImmune[BuffID.BrokenArmor] = true;
			Player.buffImmune[BuffID.Bleeding] = true;
			Player.buffImmune[BuffID.Poisoned] = true;
			Player.buffImmune[BuffID.Slow] = true;
			Player.buffImmune[BuffID.Confused] = true;
			Player.buffImmune[BuffID.Silenced] = true;
			Player.buffImmune[BuffID.Cursed] = true;
			Player.buffImmune[BuffID.Darkness] = true;
			Player.noKnockback = true;
			Player.fireWalk = true;
			Player.noFallDmg = true;
			player.AddEffect<DefenseBrainEffect>(item);
			//charm of myths
			Player.pStone = true;
			player.AddEffect<DefenseStarEffect>(item);
			player.AddEffect<DefenseBeeEffect>(item);
			Player.longInvince = true;
			//shiny stone
			Player.shinyStone = true;
			player.AddEffect<FleshKnuckleEffect>(item);
			player.AddEffect<FrozenTurtleEffect>(item);
			player.AddEffect<PaladinShieldEffect>(item);
			player.AddEffect<ShimmerImmunityEffect>(item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.HandWarmer)
			.AddIngredient(ItemID.ObsidianHorseshoe)
			.AddIngredient(ItemID.WormScarf)
			.AddIngredient(ItemID.BrainOfConfusion)
			.AddIngredient(ItemID.CharmofMyths)
			.AddIngredient(ItemID.BeeCloak)
			.AddIngredient(ItemID.StarVeil)
			.AddIngredient(ItemID.ShinyStone)
			.AddIngredient(ItemID.HeroShield)
			.AddIngredient(ItemID.FrozenShield)
			.AddIngredient(ItemID.AnkhShield)
			.AddIngredient(ItemID.ShimmerCloak)
			.AddTile<CrucibleCosmosSheet>()
			.Register();
		}
	}
	public class ShimmerImmunityEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.ShimmerCloak;
		public override bool IgnoresMutantPresence => true;
		public override void PostUpdateEquips(Player player)
		{
			player.buffImmune[BuffID.Shimmer] = true;
		}
	}
	public class PaladinShieldEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.PaladinsShield;
		public override bool IgnoresMutantPresence => true;
		public override void PostUpdateEquips(Player player)
		{
			if (player.statLife > player.statLifeMax2 * .25)
			{
				player.hasPaladinShield = true;
				for (int k = 0; k < Main.maxPlayers; k++)
				{
					Player target = Main.player[k];

					if (target.active && player != target && Vector2.Distance(target.Center, player.Center) < 400) target.AddBuff(BuffID.PaladinsShield, 30);
				}
			}
		}
	}
	public class FrozenTurtleEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.FrozenTurtleShell;
		public override bool IgnoresMutantPresence => true;
		public override void PostUpdateEquips(Player player)
		{
			if (player.statLife <= player.statLifeMax2 * 0.5)
				player.AddBuff(BuffID.IceBarrier, 5, true);
		}
	}
	public class FleshKnuckleEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.FleshKnuckles;
		public override bool IgnoresMutantPresence => true;
		public override void PostUpdateEquips(Player player)
		{
			player.aggro += 400;
		}
	}
	public class DefenseBeeEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.BeeCloak;
		public override void PostUpdateEquips(Player player)
		{
			player.honeyCombItem = EffectItem(player);
		}
	}
	public class DefenseStarEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.StarVeil;
		public override void PostUpdateEquips(Player player)
		{
			player.starCloakItem = EffectItem(player);
		}
	}
	public class DefenseBrainEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
		public override int ToggleItemType => ItemID.BrainOfConfusion;
		public override bool IgnoresMutantPresence => true;
		public override void PostUpdateEquips(Player player)
		{
			player.brainOfConfusionItem = EffectItem(player);
		}
	}
}