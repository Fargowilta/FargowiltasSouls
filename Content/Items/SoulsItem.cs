using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items
{
	/// <summary>
	/// Abstract class extended by the items of this mod. <br />
	/// Contains useful code for boilerplate reduction.
	/// </summary>
	public abstract class SoulsItem : ModItem
	{
		/// <summary>
		/// Whether or not this item is excluse to Eternity Mode. <br />
		/// If it is, the item's text color will automatically be set to a custom color (can manually be overriden) and "Eternity" will be added to the end of the item's tooltips.
		/// </summary>
		public virtual bool Eternity => false;

		/// <summary>
		/// A list of articles that this item may begin with depending on localization. <br />
		/// Used for the prefix-article fix.
		/// </summary>
		public virtual List<string> Articles => new() { "The" };

		/// <summary>
		/// Allows you to modify all the tooltips that display for this item. <br />
		/// Called directly after the code in <see cref="SafeModifyTooltips(List{TooltipLine})"/>.
		/// </summary>
		/// <param name="tooltips"></param>
		public virtual void SafeModifyTooltips(List<TooltipLine> tooltips)
		{
		}

		/// <summary>
		/// The location of the item's glowmask texture, defaults to the item's internal texture name with _glow
		/// </summary>
		public virtual string Glowmaskstring => Texture + "_glow";

		/// <summary>
		/// The amount of frames in the item's animation. <br />
		/// </summary>
		public virtual int NumFrames => 1;

		/// <summary>
		/// Whether this item currently has togglable effects that are disabled. Used for tooltip. <br />
		/// </summary>
		public bool HasDisabledEffects = false;

		/// <summary>
		/// Allows you to draw things in front of this item. This method is called even if PreDrawInWorld returns false. <br />
		/// Runs directly after the code for PostDrawInWorld in SoulsItem.
		/// </summary>
		public virtual void SafePostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) { }

		public sealed override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (Mod.RequestAssetIfExists(Glowmaskstring, out Asset<Texture2D> _))
			{
				Item item = Main.item[whoAmI];
				Texture2D texture = ModContent.Request<Texture2D>(Glowmaskstring, AssetRequestMode.ImmediateLoad).Value;
				int height = texture.Height / NumFrames;
				int width = texture.Width;
				int frame = NumFrames > 1 ? height * Main.itemFrame[whoAmI] : 0;
				SpriteEffects flipdirection = item.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				Rectangle Origin = new(0, frame, width, height);
				Vector2 DrawCenter = new(item.Center.X, item.position.Y + item.height - height / 2);
				Main.EntitySpriteDraw(texture, DrawCenter - Main.screenPosition, Origin, Color.White, rotation, Origin.Size() / 2, scale, flipdirection, 0);
			}
			SafePostDrawInWorld(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
		}


		public sealed override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
			{
				// This is often overridden.
				if (Eternity)
					itemNameLine.OverrideColor = FargowiltasSouls.EModeColor();

				// Call the artcle-prefix adjustment method.
				// This automatically handles fixing item names that begin with an article.
				itemNameLine.ArticlePrefixAdjustment(Articles.ToArray());
			}

			SafeModifyTooltips(tooltips);

			// Add the Eternity toolip after tooltip modification in order to be displayed underneath any manual tooltips (i.e. SoE cycling).
			if (Eternity)
				tooltips.Add(new TooltipLine(Mod, $"{Mod.Name}:Eternity", Language.GetTextValue($"Mods.FargowiltasSouls.Items.Extra.EternityItem")));
			if (HasDisabledEffects)
			{
				string text = $"[i:{ModContent.ItemType<TogglerIconItem>()}] [c/BC5252:{Language.GetTextValue($"Mods.FargowiltasSouls.Items.Extra.DisabledEffects")}]";
				tooltips.Add(new TooltipLine(Mod, $"{Mod.Name}:DisabledEffects", text));
			}
		}
	}
}