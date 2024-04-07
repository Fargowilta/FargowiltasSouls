
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
	[AutoloadEquip(EquipType.Wings)]
	public class EternitySoul : FlightMasteryWings
	{

		public override bool HasSupersonicSpeed => true;

		public override bool Eternity => true;
		public override int NumFrames => 10;

		public static int WingSlotID
		{
			get;
			private set;
		}

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			WingSlotID = Item.wingSlot;

			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 10));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
		}

		public override void SafeModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Item.social)
			{
				return;
			}

			const int linesToShow = 7;

			string description = Language.GetTextValue("Mods.FargowiltasSouls.Items.EternitySoul.Extra.Additional");
			description += "                                                                                                                                               "; // blankspaces for consistent box size lmao

			if (Main.GameUpdateCount % 5 == 0 || EternitySoulSystem.TooltipLines == null)
			{
				EternitySoulSystem.TooltipLines = new();
				for (int i = 0; i < linesToShow; i++)
				{
					string line = Main.rand.NextFromCollection(EternitySoulSystem.Tooltips);
					if (EternitySoulSystem.TooltipLines.Contains(line)) // duplicate
					{
						i--;
						continue;
					}
					EternitySoulSystem.TooltipLines.Add(line);
				}
			}
			for (int i = 0; i < EternitySoulSystem.TooltipLines.Count; i++)
			{
				description += "\n" + EternitySoulSystem.TooltipLines[i];
			}
			tooltips.Add(new TooltipLine(Mod, "tooltip", description));
			tooltips.Add(new TooltipLine(Mod, "FlavorText", Language.GetTextValue("Mods.FargowiltasSouls.Items.EternitySoul.Extra.Flavor")));
		}

		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
		{
			if ((line.Mod == "Terraria" && line.Name == "ItemName") || line.Name == "FlavorText")
			{
				Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Text");
				shader.TrySetParameter("mainColor", new Color(42, 42, 99));
				shader.TrySetParameter("secondaryColor", FargowiltasSouls.EModeColor());
				shader.Apply("PulseUpwards");
				Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
				Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
				return false;
			}
			return true;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Red;
			Item.value = 200000000;
			Item.shieldSlot = 5;
			Item.defense = 100;

			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 180;
			Item.useAnimation = 180;
			Item.UseSound = SoundID.Item6;
		}

		public override void UseItemFrame(Player player) => SandsofTime.Use(player);
		public override bool? UseItem(Player player) => true;

		void PassiveEffect(Player player)
		{
			BionomicCluster.PassiveEffect(player, Item);
			AshWoodEnchant.PassiveEffect(player);

			player.FargoSouls().CanAmmoCycle = true;

			player.FargoSouls().WoodEnchantDiscount = true;

			//cell phone
			player.accWatch = 3;
			player.accDepthMeter = 1;
			player.accCompass = 1;
			player.accFishFinder = true;
			player.accDreamCatcher = true;
			player.accOreFinder = true;
			player.accStopwatch = true;
			player.accCritterGuide = true;
			player.accJarOfSouls = true;
			player.accThirdEye = true;
			player.accCalendar = true;
			player.accWeatherRadio = true;
		}

		public override void UpdateInventory(Player player) => PassiveEffect(player);
		public override void UpdateVanity(Player player) => PassiveEffect(player);
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			PassiveEffect(player);

			FargoSoulsPlayer modPlayer = player.FargoSouls();
			//auto use, debuffs, mana up
			modPlayer.Eternity = true;
			player.AddEffect<EternityTin>(Item);

			//UNIVERSE
			modPlayer.UniverseSoul = true;
			modPlayer.UniverseCore = true;
			player.GetDamage(DamageClass.Generic) += 2.5f;
			player.AddEffect<UniverseSpeedEffect>(Item);
			player.maxMinions += 20;
			player.maxTurrets += 10;
			//accessorys

			player.counterWeight = 556 + Main.rand.Next(6);
			player.yoyoGlove = true;
			player.yoyoString = true;

			player.AddEffect<SniperScopeEffect>(Item);
			player.manaFlower = true;
			player.manaMagnet = true;
			player.magicCuffs = true;
			player.manaCost -= 0.5f;

			//DIMENSIONS
			player.statLifeMax2 *= 5;
			player.buffImmune[BuffID.ChaosState] = true;
			ColossusSoul.AddEffects(player, Item, 0, 0.4f, 15);
			SupersonicSoul.AddEffects(player, Item, hideVisual);
			FlightMasterySoul.AddEffects(player, Item);
			TrawlerSoul.AddEffects(player, Item, hideVisual);
			WorldShaperSoul.AddEffects(player, Item, hideVisual);

			//TERRARIA
			ModContent.GetInstance<TerrariaSoul>().UpdateAccessory(player, hideVisual);
			//MASOCHIST
			ModContent.GetInstance<MasochistSoul>().UpdateAccessory(player, hideVisual);

		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(null, "UniverseSoul")
			.AddIngredient(null, "DimensionSoul")
			.AddIngredient(null, "TerrariaSoul")
			.AddIngredient(null, "MasochistSoul")

			.AddIngredient(null, "EternalEnergy", 30)

			.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

			.Register();
		}
	}
	public class EternitySoulSystem : ModSystem
	{
		public static List<string> Tooltips = new();
		public static List<string> TooltipLines = new();
		public override void OnLocalizationsLoaded() // rebuild on language changed
		{
			Tooltips.Clear();
			PostAddRecipes();
		}

		public override void PostAddRecipes()
		{
			string[] startsWithFilter = Language.GetTextValue("Mods.FargowiltasSouls.Items.EternitySoul.Extra.StartsWithFilter").Split("|", StringSplitOptions.RemoveEmptyEntries);
			string[] containsFilter = Language.GetTextValue("Mods.FargowiltasSouls.Items.EternitySoul.Extra.ContainsFilter").Split("|", StringSplitOptions.RemoveEmptyEntries);

			foreach (Recipe recipe in Main.recipe.Where(r => r.createItem != null && r.createItem.ModItem is BaseSoul))
			{
				foreach (Item item in recipe.requiredItem)
				{
					if (item.ModItem is ModItem modItem)
					{
						var tooltips = modItem.Tooltip.Value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) // trim lines start with whitespace
							.Where(line => !startsWithFilter.Any(line.StartsWith) && !containsFilter.Any(line.Contains)); // filter flavor text or undesired lines

						Tooltips.AddRange(tooltips);
					}
				}
			}
		}
	}
	public class EternityTin : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<EternityHeader>();
		public override int ToggleItemType => ModContent.ItemType<EternitySoul>();
		public override bool IgnoresMutantPresence => true;
	}

}