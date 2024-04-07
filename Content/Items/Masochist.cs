using Fargowiltas.NPCs;
using Fargowiltas.Projectiles;

using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items
{
	public class Masochist : SoulsItem
	{
		public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mutant's Gift");
			/* Tooltip.SetDefault(@"World must be in Expert Mode
[c/ff0000:Not] intended for use with Master Mode
Toggles Eternity Mode and enables Master Mode drops
Deviantt provides a starter pack and progress-based advice
Improves base stats
Debuffs wear off faster when not attacking
Cannot be used while a boss is alive
[i:1612][c/00ff00:Recommended to use Fargo's Mutant Mod Debuff Display (in config)]
[c/ff0000:NOT INTENDED FOR USE WITH OTHER CONTENT MODS OR MODDED DIFFICULTIES]"); */
			//DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变体的礼物");
			//Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'用开/关受虐模式'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public static bool CanPlayMaso => WorldSavingSystem.CanPlayMaso || Main.LocalPlayer.active && Main.LocalPlayer.FargoSouls().Toggler.CanPlayMaso;

		public override void SafeModifyTooltips(List<TooltipLine> tooltips)
		{
			base.SafeModifyTooltips(tooltips);

			if (CanPlayMaso)
			{
				TooltipLine line = new(Mod, "tooltip", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Name}.ExtraTooltip"));
				tooltips.Add(line);
			}
		}

		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
		{
			if (CanPlayMaso)
			{
				if (line.Mod == "Terraria" && line.Name == "ItemName" || line.Mod == Mod.Name && line.Name == "tooltip")
				{
					Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
					Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
					ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Text");
					shader.TrySetParameter("mainColor", new Color(28, 222, 152));
					shader.TrySetParameter("secondaryColor", new Color(168, 245, 228));
					shader.Apply("PulseUpwards");
					Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
					Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
					Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
					return false;
				}
			}
			return true;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = false;
		}

		public override bool? UseItem(Player player)
		{
			if (FargoSoulsUtil.WorldIsExpertOrHarder())
			{
				if (!LumUtils.AnyBosses())
				{
					WorldSavingSystem.ShouldBeEternityMode = !WorldSavingSystem.ShouldBeEternityMode;

					int deviType = ModContent.NPCType<Deviantt>();
					if (FargoSoulsUtil.HostCheck && WorldSavingSystem.ShouldBeEternityMode && !WorldSavingSystem.SpawnedDevi && !NPC.AnyNPCs(deviType))
					{
						WorldSavingSystem.SpawnedDevi = true;

						Vector2 spawnPos = (Main.zenithWorld || Main.remixWorld) ? player.Center : player.Center - 1000 * Vector2.UnitY;
						Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawnPos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, deviType);

						FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.DisplayName"));
					}

					SoundEngine.PlaySound(SoundID.Roar, player.Center);

					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.WorldData); //sync world
				}
			}
			else
			{
				FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Items.{Name}.WrongDifficulty", new Color(175, 75, 255));
			}
			return true;
		}
	}
}