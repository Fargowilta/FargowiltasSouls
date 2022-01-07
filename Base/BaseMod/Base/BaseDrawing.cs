using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.UI.Chat;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace FargowiltasSouls
{
	public class DrawAnimationPrecise : DrawAnimation
	{
		int Width = 0, Height = 0, offsetX = 0, offsetY = 2;
		bool vertical = true;
		public DrawAnimationPrecise(int ticksperframe, int frameCount, int w, int h, bool v = true, int offX = 0, int offY = 2)
		{
			Width = w;
			Height = h;
			vertical = v;
			offsetX = offX;
			offsetY = offY;
			this.Frame = 0;
			this.FrameCounter = 0;
			this.FrameCount = frameCount;
			this.TicksPerFrame = ticksperframe;
		}

		public override void Update()
		{
			int num = this.FrameCounter + 1;
			this.FrameCounter = num;
			if (num >= this.TicksPerFrame)
			{
				this.FrameCounter = 0;
				num = this.Frame + 1;
				this.Frame = num;
				if (num >= this.FrameCount)
				{
					this.Frame = 0;
				}
			}
		}

		public override Rectangle GetFrame(Texture2D texture)
		{
			return new Rectangle(vertical ? 0 : (this.Width + offsetX) * this.Frame, vertical ? (this.Height + offsetY) * this.Frame : 0, this.Width, this.Height);
		}
	}	
	public class InterfaceLayer 
	{
		public string name;
		public Action<InterfaceLayer, SpriteBatch> drawAction;
		public GameInterfaceLayer listItem;
		
		public bool visible 
		{ 
			get
			{ 
				return (listItem == null ? false : listItem.Active); 
			} 
			set
			{
				if(listItem != null) listItem.Active = value; 
			} 
		}
		
		public InterfaceLayer(string n, Action<InterfaceLayer, SpriteBatch> action)
		{
			name = n;
			drawAction = action;
		}

		public void Draw()
		{
			drawAction(this, Main.spriteBatch);
		}
	}
    public class BaseDrawing
    {
		//public static ShaderContext shaderContext = new ShaderContext();
        //------------------------------------------------------//
        //----------------BASE DRAWING CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods for various drawing functions, such //
        // as guns, afterimages, drawing on the player, etc.    //
        //------------------------------------------------------//
        //  Author(s): Grox the Great, Yoraiz0r                 //
        //------------------------------------------------------//
	
		public static void DrawInvasionProgressBar(SpriteBatch sb, int progress, int progressMax, bool forceDisplay, ref int displayCount, ref float displayAlpha, Texture2D iconTex, string displayText, string percentText = null, Color backgroundColor = default, Vector2 offset = default)
		{
			if (Main.invasionProgressMode == 2 && forceDisplay && displayCount < 160)
			{
				displayCount = 160;
			}
			if (!Main.gamePaused && displayCount > 0) displayCount = Math.Max(0, displayCount - 1);
			if (displayCount > 0){ displayAlpha += 0.05f; }else { displayAlpha -= 0.05f; }
			if (displayAlpha < 0f) displayAlpha = 0f; if (displayAlpha > 1f) displayAlpha = 1f;
			if (displayAlpha <= 0f) return;
			float displayScalar = 0.5f + displayAlpha * 0.5f;

			int displayWidth = (int)(200f * displayScalar);
			int displayHeight = (int)(45f * displayScalar);
			Vector2 basePosition = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40) + offset;
			Rectangle displayRect = new Rectangle((int)basePosition.X - displayWidth / 2, (int)basePosition.Y - displayHeight / 2, displayWidth, displayHeight);
			Utils.DrawInvBG(Main.spriteBatch, displayRect, new Color(63, 65, 151, 255) * 0.785f);
			string displayText2;
			if (progressMax == 0){ displayText2 = progress.ToString(); }else{ displayText2 = ((int)(progress * 100f / progressMax)).ToString() + "%"; }
			if(percentText != null) displayText2 = percentText;
			//displayText2 = Language.GetTextValue("Game.WaveCleared", displayText2);
			Texture2D barTex = Main.colorBarTexture;
			if (progressMax != 0)
			{
				Main.spriteBatch.Draw(barTex, basePosition, null, Color.White * displayAlpha, 0f, new Vector2(barTex.Width / 2, 0f), displayScalar, SpriteEffects.None, 0f);
				float progressPercent = MathHelper.Clamp(progress / (float)progressMax, 0f, 1f);
				float scalarX = 169f * displayScalar;
				float scalarY = 8f * displayScalar;
				Vector2 vector4 = basePosition + Vector2.UnitY * scalarY + Vector2.UnitX * 1f;
				Utils.DrawBorderString(Main.spriteBatch, displayText2, vector4, Microsoft.Xna.Framework.Color.White * displayAlpha, displayScalar, 0.5f, 1f, -1);
				vector4 += Vector2.UnitX * (progressPercent - 0.5f) * scalarX;
				Main.spriteBatch.Draw(Main.magicPixel, vector4, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), new Microsoft.Xna.Framework.Color(255, 241, 51) * displayAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(scalarX * progressPercent, scalarY), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, vector4, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), new Microsoft.Xna.Framework.Color(255, 165, 0, 127) * displayAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, scalarY), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, vector4, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), Microsoft.Xna.Framework.Color.Black * displayAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(scalarX * (1f - progressPercent), scalarY), SpriteEffects.None, 0f);
			}

			Vector2 center = new Vector2(Main.screenWidth - 120, Main.screenHeight - 80) + offset;
			Vector2 stringLength = Main.fontItemStack.MeasureString(displayText);
			Microsoft.Xna.Framework.Rectangle textRect = Utils.CenteredRectangle(center, (stringLength + new Vector2(iconTex.Width + 20, 10f)) * displayScalar);
			Utils.DrawInvBG(Main.spriteBatch, textRect, backgroundColor);
			Main.spriteBatch.Draw(iconTex, textRect.Left() + Vector2.UnitX * displayScalar * 8f, null, Microsoft.Xna.Framework.Color.White * displayAlpha, 0f, new Vector2(0f, iconTex.Height / 2), displayScalar * 0.8f, SpriteEffects.None, 0f);
			Utils.DrawBorderString(Main.spriteBatch, displayText, textRect.Right() + Vector2.UnitX * displayScalar * -8f, Microsoft.Xna.Framework.Color.White * displayAlpha, displayScalar * 0.9f, 1f, 0.4f, -1);
		}
		
		public static void AddInterfaceLayer(Mod mod, List<GameInterfaceLayer> list, InterfaceLayer layer, string parent, bool first)
		{
			GameInterfaceLayer item = new LegacyGameInterfaceLayer(mod.Name + ":" + layer.name, delegate
			{
				layer.Draw();
				return true;
			}, InterfaceScaleType.UI);
			layer.listItem = item;

			int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                GameInterfaceLayer dl = list[m];
                if (dl.Name.Contains(parent)) { insertAt = m; break; }
            }
            if (insertAt == -1) list.Add(item); else list.Insert(first ? insertAt : insertAt + 1, item);		
		}
		
		//NOTE: HIGHLY UNSTABLE, ONLY USE IF YOU KNOW WHAT YOU ARE DOING!	
		public static Texture2D StitchTogetherTileTex(Texture2D tex, int tileType, int width = -1, int[] heights = null)
		{
			TileObjectData data = TileObjectData.GetTileData(tileType, 0);
			if(width == -1) width = data.CoordinateWidth; if(heights == null) heights = data.CoordinateHeights; int padding = data.CoordinatePadding;
			List<Texture2D> subTexs = new List<Texture2D>();
			//List<Texture2D> subTexs2 = new List<Texture2D>();		
			for(int w = 0; w < data.Width; w++)
			{
				//subTexs.Clear();
				for(int h = 0; h < data.Height; h++)
				{
					int currentHeight = 0, tempH = h;
					while(tempH > 0){ currentHeight += heights[tempH] + padding;  tempH--; }
					subTexs.Add(GetCroppedTex(tex, new Rectangle(w * (width + padding), currentHeight, width, heights[h])));
				}
				/*for(int m = 0; m < subTexs.Count; m++)
				{
					int currentHeight = 0, int tempH = (data.Height - 1);
					while(tempH > 0){ currentHeight += heights[tempH];  tempH--; }	
					Rectangle newBounds = new Rectangle(0, 0, data.Width * width, newHeight);
					Texture2D tex = new Texture2D(Main.instance.GraphicsDevice, newBounds);
				}*/
			}
			int newHeight = 0, tempH2 = (data.Height - 1);
			while(tempH2 > 0){ newHeight += heights[tempH2];  tempH2--; }	
			Rectangle newBounds = new Rectangle(0, 0, data.Width * width, newHeight);
			Texture2D tex2 = new Texture2D(Main.instance.GraphicsDevice, newBounds.Width, newBounds.Height);
			List<Vector2> drawPos = new List<Vector2>();
			for(int m = 0; m < subTexs.Count; m++) drawPos.Add(new Vector2(width * m, 0));
			return DrawTextureToTexture(tex2, subTexs.ToArray(), drawPos.ToArray());
		}
		
		//NOTE: HIGHLY UNSTABLE, ONLY USE IF YOU KNOW WHAT YOU ARE DOING!
		public static Texture2D DrawTextureToTexture(Texture2D toDrawTo, Texture2D[] toDraws, Vector2[] drawPos)
		{
			RenderTarget2D renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, toDrawTo.Width, toDrawTo.Height, false, Main.instance.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);			
			Main.instance.GraphicsDevice.SetRenderTarget(renderTarget);
			Main.instance.GraphicsDevice.Clear(Color.Black);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);			
			for(int m = 0; m < toDraws.Length; m++)
			{
				Texture2D toDraw = toDraws[m];
				DrawTexture(Main.spriteBatch, toDraw, 0, drawPos[m], toDraw.Width, toDraw.Height, 1f, 0f, 0, 1, toDraw.Bounds, null);
			}
			Main.spriteBatch.End();		
			Main.instance.GraphicsDevice.SetRenderTarget(null);
			return renderTarget;	
		}
		
		public static Texture2D GetCroppedTex(Texture2D texture, Rectangle rect)
		{
			return GetCroppedTex(texture, rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public static Texture2D GetCroppedTex(Texture2D texture, int startX, int startY, int newWidth, int newHeight)
		{
			Rectangle newBounds = texture.Bounds;
			newBounds.X += startX;
			newBounds.Y += startY;
			newBounds.Width = newWidth;
			newBounds.Height = newHeight;
			Texture2D croppedTexture = new Texture2D(Main.instance.GraphicsDevice, newBounds.Width, newBounds.Height);
			// Copy the data from the cropped region into a buffer, then into the new texture
			Color[] data = new Color[newBounds.Width * newBounds.Height];
			texture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
			croppedTexture.SetData(data);
			return croppedTexture;
		}
		

        public static Texture2D GetPlayerTex(Player p, string name)
        {
            return GetPlayerTex(p.skinVariant, name, p.Male);
        }

        public static Texture2D GetPlayerTex(int skinVariant, string name, bool male = true)
        {
            //TODO: FINISH THIS
            switch (name)
            {
                case "Head": return Main.playerTextures[skinVariant, 0];
                case "EyeWhite": return Main.playerTextures[skinVariant, 1];
                case "Eye": return Main.playerTextures[skinVariant, 2];
                case "Body": return (male ? Main.playerTextures[skinVariant, 4] : Main.playerTextures[skinVariant, 6]);
                case "Hand": return Main.playerTextures[skinVariant, 5];				
                case "Arms": return Main.playerTextures[skinVariant, 7];
                case "Legs": return Main.playerTextures[skinVariant, 10];
            }
            return null;
        }

        public static void AddPlayerLayer(List<PlayerLayer> list, PlayerLayer layer, PlayerLayer parent, bool first)
        {
            int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                PlayerLayer dl = list[m];
                if (dl.Name.Equals(parent.Name)) { insertAt = m; break; }
            }
            if (insertAt == -1) list.Add(layer); else list.Insert(first ? insertAt : insertAt + 1, layer);
        }

        public static void AddPlayerHeadLayer(List<PlayerHeadLayer> list, PlayerHeadLayer layer, PlayerHeadLayer parent, bool first)
        {
            int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                PlayerHeadLayer dl = list[m];
                if (dl.Name.Equals(parent.Name)){ insertAt = m; break; }
            }
            if (insertAt == -1) list.Add(layer); else list.Insert(first ? insertAt : insertAt + 1, layer);
        }

        /*
         * Returns a rectangle representing the frame on a texture, can offset on the x axis.
         * 
         * pixelSpaceX/pixelSpaceY : The 'pixel space' seperating two frames in the texture on the X/Y axis, respectively.
         */
        public static Rectangle GetAdvancedFrame(int currentFrame, int frameOffsetX, int frameWidth, int frameHeight, int pixelSpaceX = 0, int pixelSpaceY = 2)
        {
			int column = (currentFrame / frameOffsetX);
			currentFrame -= (column * frameOffsetX);
			pixelSpaceY *= currentFrame;
			int startX = (frameOffsetX == 0 ? 0 : column * (frameWidth + pixelSpaceX));			
            int startY = (frameHeight * currentFrame) + pixelSpaceY;
            return new Rectangle(startX, startY, frameWidth, frameHeight);
        }		
		
        /*
         * Returns a rectangle representing the frame on a texture.
         * 
         * pixelSpaceX/pixelSpaceY : The 'pixel space' seperating two frames in the texture on the X/Y axis, respectively.
         */
        public static Rectangle GetFrame(int currentFrame, int frameWidth, int frameHeight, int pixelSpaceX = 0, int pixelSpaceY = 2)
        {
            pixelSpaceY *= currentFrame;
            int startY = (frameHeight * currentFrame) + pixelSpaceY;
            return new Rectangle(0, startY, frameWidth - pixelSpaceX, frameHeight);
        }

        /*
         * Returns true if the given pass is not an effect one. This is primary used for things that don't want to be 
         * drawn in Shadow Aura (Hallow Armor), Shadow Afterimage (Necro Armor), or Glow (Chlorophyte Armor) effects.
         */
        public static bool IsNormalDrawPass(Player player, PlayerDrawInfo pdi = default(PlayerDrawInfo))
        {
            return player.ghostFade == 0f && player.shadow == 0f && (pdi.Equals(default(PlayerDrawInfo)) || pdi.shadow == 0f);
        }

		public static int GetDye(Player drawPlayer, int accSlot, bool social = false, bool wings = false)
		{
			int dye = accSlot % 10;
			if (!wings && accSlot < 10 && drawPlayer.hideVisual[dye]) return -1;
            return GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[dye].type);
		}

		/*
		 * Returns a color roughly associated with the given dye. (special dyes return null)
		 */
		public static Color? GetDyeColor(int dye)
		{
			Color? returnColor = null;
			float brightness = 1f;
			if (dye >= 13 && dye <= 24) { brightness = 0.7f; dye -= 12; } //black
			if (dye >= 45 && dye <= 56) { brightness = 1.3f; dye -= 44; } //silver
			if (dye >= 32 && dye <= 43) { brightness = 1.5f; dye -= 31; } //bright dyes
			switch (dye)
			{
				case 1: returnColor = new Color(248, 63, 63); break; //red
				case 2: returnColor = new Color(248, 148, 63); break; //orange
				case 3: returnColor = new Color(248, 242, 62); break; //yellow
				case 4: returnColor = new Color(157, 248, 70); break; //lime
				case 5: returnColor = new Color(48, 248, 70); break; //green
				case 6: returnColor = new Color(60, 248, 70); break; //teal
				case 7: returnColor = new Color(62, 242, 248); break; //cyan
				case 8: returnColor = new Color(64, 181, 247); break; //sky blue
				case 9: returnColor = new Color(66, 95, 247); break; //blue
				case 10: returnColor = new Color(159, 65, 247); break; //purple
				case 11: returnColor = new Color(212, 65, 247); break; //violet
				case 12: returnColor = new Color(242, 63, 131); break; //pink
				case 31: returnColor = new Color(226, 226, 226); break; //silver
				case 44: returnColor = new Color(40, 40, 40); break; //black
				case 62: returnColor = new Color(157, 248, 70); break; //yellow gradient dye
				case 63: returnColor = new Color(64, 181, 247); break; //cyan gradient dye
				case 64: returnColor = new Color(212, 65, 247); break; //violet gradient dye
			}
			if (returnColor != null && brightness != 1f) returnColor = BaseUtility.ColorMult((Color)returnColor, brightness);
			return returnColor;
		}

        /*
         * Returns a color associated with the given vanilla gem type.
         */
        public static Color GetGemColor(int type)
        {
            if(type == 181){ return Color.MediumOrchid; }else //Amethyst
            if(type == 180){ return Color.Gold; }else //Topaz
            if(type == 177){ return Color.DeepSkyBlue; }else //Sapphire
            if(type == 178){ return Color.Crimson; }else //Ruby
            if(type == 179){ return Color.LimeGreen; }else //Emerald
            if(type == 182){ return Color.GhostWhite; }else //Diamond
			if(type == 999){ return Color.Orange; } //Amber
            return Color.Black;
        }

		/*
         * Convenience method for getting lighting color with buff effects and such applied using an npc and a position. 
         * effects : if false, do not spawn dust- and light-related effects, only color the light.
         */
        public static Color GetNPCColor(NPC npc, Vector2? position = null, bool effects = true, float shadowOverride = 0f)
        {
            return npc.GetAlpha(BuffEffects(npc, GetLightColor(position != null ? (Vector2)position : npc.Center), (shadowOverride != 0f ? shadowOverride : 0f), effects, npc.poisoned, npc.onFire, npc.onFire2, Main.player[Main.myPlayer].detectCreature, false, false, false, npc.venom, npc.midas, npc.ichor, npc.onFrostBurn, false, false, npc.dripping, npc.drippingSlime, npc.loveStruck, npc.stinky));
        }

        /*
         * Convenience method for getting lighting color with buff effects and such applied using a player and a position. 
         * effects : if false, do not spawn dust- and light-related effects, only color the light.
         */
		public static Color GetPlayerColor(Player p, Vector2? position = null, bool effects = false, float shadowOverride = 0f)
        {
            return p.GetImmuneAlpha(BuffEffects(p, GetLightColor(position != null ? (Vector2)position : p.Center), (shadowOverride != 0f ? shadowOverride : p.shadow), effects, p.poisoned, p.onFire, p.onFire2, false, p.noItems, p.blind, p.bleed, p.venom, false, p.ichor, p.onFrostBurn, p.burned, p.honey, p.dripping, p.drippingSlime, p.loveStruck, p.stinky), p.shadow);
        }

        /*
         * Convenience method for getting lighting color using an npc or projectile position.
         */
        public static Color GetLightColor(Vector2 position)
        {
            return Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));
        }

        /*
         * Convenience method for adding lighting using an npc or projectile position, using a Color instance for color.
         */
        public static void AddLight(Vector2 position, Color color, float brightnessDivider = 1F)
        {
            AddLight(position, color.R / 255F, color.G / 255F, color.B / 255F, brightnessDivider);
        }
        /*
         * Convenience method for adding lighting using an npc or projectile position with 0F - 1F color values.
         */
        public static void AddLight(Vector2 position, float colorR, float colorG, float colorB, float brightnessDivider = 1F)
        {
            Lighting.AddLight((int)(position.X / 16f), (int)(position.Y / 16f), colorR / brightnessDivider, colorG / brightnessDivider, colorB / brightnessDivider);
        }

        /*
         * Applies buff coloring and spawns effects based on if the effect is true or false. 
         */
        public static Color BuffEffects(Entity codable, Color lightColor, float shadow = 0f, bool effects = true, bool poisoned = false, bool onFire = false, bool onFire2 = false, bool hunter = false, bool noItems = false, bool blind = false, bool bleed = false, bool venom = false, bool midas = false, bool ichor = false, bool onFrostBurn = false, bool burned = false, bool honey = false, bool dripping = false, bool drippingSlime = false, bool loveStruck = false, bool stinky = false)
        {
            float cr = 1f; float cg = 1f; float cb = 1f; float ca = 1f;
			if (effects && honey && Main.rand.Next(30) == 0)
			{
				int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 152, 0f, 0f, 150, default, 1f);
				Main.dust[dustID].velocity.Y = 0.3f;
				Main.dust[dustID].velocity.X *= 0.1f;
				Main.dust[dustID].scale += Main.rand.Next(3, 4) * 0.1f;
				Main.dust[dustID].alpha = 100;
				Main.dust[dustID].noGravity = true;
				Main.dust[dustID].velocity += codable.velocity * 0.1f;
				if (codable is Player) Main.playerDrawDust.Add(dustID);
			}
            if (poisoned)
            {
				if (effects && Main.rand.Next(30) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 46, 0f, 0f, 120, default, 0.2f);
					Main.dust[dustID].noGravity = true;
					Main.dust[dustID].fadeIn = 1.9f;
					if (codable is Player) Main.playerDrawDust.Add(dustID);
				}
                cr *= 0.65f;
                cb *= 0.75f;
            }
			if (venom)
			{
				if (effects && Main.rand.Next(10) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 171, 0f, 0f, 100, default, 0.5f);
					Main.dust[dustID].noGravity = true;
					Main.dust[dustID].fadeIn = 1.5f;
					if (codable is Player) Main.playerDrawDust.Add(dustID);
				}
				cg *= 0.45f;
				cr *= 0.75f;
			}
			if (midas)
			{
				cb *= 0.3f;
				cr *= 0.85f;
			}
			if (ichor)
			{
				if (codable is NPC) { lightColor = new Color(255, 255, 0, 255); } else { cb = 0f; }
			}
			if (burned)
			{
				if (effects)
				{
					int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 2f);
					Main.dust[dustID].noGravity = true;
					Main.dust[dustID].velocity *= 1.8f;
					Main.dust[dustID].velocity.Y -= 0.75f;
					if (codable is Player) Main.playerDrawDust.Add(dustID);
				}
				if (codable is Player)
				{
					cr = 1f;
					cb *= 0.6f;
					cg *= 0.7f;
				}
			}
			if (onFrostBurn)
			{
				if (effects)
				{
					if (Main.rand.Next(4) < 3)
					{
						int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 135, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].velocity *= 1.8f;
						Main.dust[dustID].velocity.Y -= 0.5f;
						if (Main.rand.Next(4) == 0)
						{
							Main.dust[dustID].noGravity = false;
							Main.dust[dustID].scale *= 0.5f;
						}
						if (codable is Player) Main.playerDrawDust.Add(dustID);
					}
					Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 0.1f, 0.6f, 1f);
				}
				if (codable is Player)
				{
					cr *= 0.5f;
					cg *= 0.7f;
				}
			}
            if (onFire)
            {
				if (effects)
				{
					if (Main.rand.Next(4) != 0)
					{
						int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].velocity *= 1.8f;
						Main.dust[dustID].velocity.Y -= 0.5f;
						if (Main.rand.Next(4) == 0)
						{
							Main.dust[dustID].noGravity = false;
							Main.dust[dustID].scale *= 0.5f;
						}
						if (codable is Player) Main.playerDrawDust.Add(dustID);
					}
					Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
				}
				if (codable is Player)
				{
					cb *= 0.6f;
					cg *= 0.7f;
				}
            }
			if (dripping && shadow == 0f && Main.rand.Next(4) != 0)
			{
				Vector2 position = codable.position;
				position.X -= 2f; position.Y -= 2f;
				if (Main.rand.Next(2) == 0)
				{
					int dustID = Dust.NewDust(position, codable.width + 4, codable.height + 2, 211, 0f, 0f, 50, default, 0.8f);
					if (Main.rand.Next(2) == 0) Main.dust[dustID].alpha += 25;
					if (Main.rand.Next(2) == 0) Main.dust[dustID].alpha += 25;
					Main.dust[dustID].noLight = true;
					Main.dust[dustID].velocity *= 0.2f;
					Main.dust[dustID].velocity.Y += 0.2f;
					Main.dust[dustID].velocity += codable.velocity;
					if(codable is Player) Main.playerDrawDust.Add(dustID);
				}else
				{
					int dustID = Dust.NewDust(position, codable.width + 8, codable.height + 8, 211, 0f, 0f, 50, default, 1.1f);
					if (Main.rand.Next(2) == 0) Main.dust[dustID].alpha += 25;
					if (Main.rand.Next(2) == 0) Main.dust[dustID].alpha += 25;
					Main.dust[dustID].noLight = true;
					Main.dust[dustID].noGravity = true;
					Main.dust[dustID].velocity *= 0.2f;
					Main.dust[dustID].velocity.Y += 1f;
					Main.dust[dustID].velocity += codable.velocity;
					if(codable is Player) Main.playerDrawDust.Add(dustID);
				}
			}
			if (drippingSlime && shadow == 0f)
			{
				int alpha = 175;
				Color newColor = new Color(0, 80, 255, 100);
				if (Main.rand.Next(4) != 0)
				{
					if (Main.rand.Next(2) == 0)
					{
						Vector2 position2 = codable.position;
						position2.X -= 2f; position2.Y -= 2f;
						int dustID = Dust.NewDust(position2, codable.width + 4, codable.height + 2, 4, 0f, 0f, alpha, newColor, 1.4f);
						if (Main.rand.Next(2) == 0) Main.dust[dustID].alpha += 25;
						if (Main.rand.Next(2) == 0) Main.dust[dustID].alpha += 25;
						Main.dust[dustID].noLight = true;
						Main.dust[dustID].velocity *= 0.2f;
						Main.dust[dustID].velocity.Y += 0.2f;
						Main.dust[dustID].velocity += codable.velocity;
						if(codable is Player) Main.playerDrawDust.Add(dustID);
					}
				}
				cr *= 0.8f;
				cg *= 0.8f;
			}
            if (onFire2)
            {
				if (effects)
				{
					if (Main.rand.Next(4) != 0)
					{
						int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 75, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].velocity *= 1.8f;
						Main.dust[dustID].velocity.Y -= 0.5f;
						if (Main.rand.Next(4) == 0)
						{
							Main.dust[dustID].noGravity = false;
							Main.dust[dustID].scale *= 0.5f;
						}
						if (codable is Player) Main.playerDrawDust.Add(dustID);
					}
					Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
				}
				if (codable is Player)
				{
					cb *= 0.6f;
					cg *= 0.7f;
				}
            }
            if (noItems)
            {
                cr *= 0.65f;
                cg *= 0.8f;
            }
            if (blind)
            {
                cr *= 0.7f;
                cg *= 0.65f;
            }
            if (bleed)
            {
				bool dead = (codable is Player ? ((Player)codable).dead : codable is NPC ? ((NPC)codable).life <= 0 : false);
				if (effects && !dead && Main.rand.Next(30) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 5, 0f, 0f, 0, default, 1f);
					Main.dust[dustID].velocity.Y += 0.5f;
					Main.dust[dustID].velocity *= 0.25f;
					if (codable is Player) Main.playerDrawDust.Add(dustID);
				}
                cg *= 0.9f;
                cb *= 0.9f;
            }
			if (loveStruck && effects && shadow == 0f && Main.instance.IsActive && !Main.gamePaused && Main.rand.Next(5) == 0)
			{
				Vector2 value = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				value.Normalize();
				value.X *= 0.66f;
				int goreID = Gore.NewGore(codable.position + new Vector2(Main.rand.Next(codable.width + 1), Main.rand.Next(codable.height + 1)), value * Main.rand.Next(3, 6) * 0.33f, 331, Main.rand.Next(40, 121) * 0.01f);
				Main.gore[goreID].sticky = false;
				Main.gore[goreID].velocity *= 0.4f;
				Main.gore[goreID].velocity.Y -= 0.6f;
				if (codable is Player) Main.playerDrawGore.Add(goreID);
			}
			if (stinky && shadow == 0f)
			{
				cr *= 0.7f;
				cb *= 0.55f;
				if (effects && Main.rand.Next(5) == 0 && Main.instance.IsActive && !Main.gamePaused)
				{
					Vector2 value2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
					value2.Normalize(); value2.X *= 0.66f; value2.Y = Math.Abs(value2.Y);
					Vector2 vector = value2 * Main.rand.Next(3, 5) * 0.25f;
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 188, vector.X, vector.Y * 0.5f, 100, default, 1.5f);
					Main.dust[dustID].velocity *= 0.1f;
					Main.dust[dustID].velocity.Y -= 0.5f;
					if (codable is Player) Main.playerDrawDust.Add(dustID);
				}
			}
			lightColor.R = (byte)(lightColor.R * cr);
			lightColor.G = (byte)(lightColor.G * cg);
			lightColor.B = (byte)(lightColor.B * cb);
			lightColor.A = (byte)(lightColor.A * ca);			
			if(codable is NPC) NPCLoader.DrawEffects((NPC)codable, ref lightColor);			
            if (hunter && (codable is NPC ? ((NPC)codable).lifeMax > 1 : true))
            {
				if (effects && !Main.gamePaused && Main.instance.IsActive && Main.rand.Next(50) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 15, 0f, 0f, 150, default, 0.8f);
					Main.dust[dustID].velocity *= 0.1f;
					Main.dust[dustID].noLight = true;
					if (codable is Player) Main.playerDrawDust.Add(dustID);
				}
				byte colorR = 50, colorG = 255, colorB = 50;
				if(codable is NPC && !(((NPC)codable).friendly || ((NPC)codable).catchItem > 0 || (((NPC)codable).damage == 0 && ((NPC)codable).lifeMax == 5)))
				{
					colorR = 255; colorG = 50;
				}
                if (!(codable is NPC) && lightColor.R < 150) { lightColor.A = Main.mouseTextColor; }
                if (lightColor.R < colorR) { lightColor.R = colorR; }
                if (lightColor.G < colorG) { lightColor.G = colorG; }
                if (lightColor.B < colorB) { lightColor.B = colorB; }
            }
            return lightColor;
        }




		/*
		 * Returns true if the requirements for drawing the player's held item are satisfied.
		 */
		public static bool ShouldDrawHeldItem(Player drawPlayer)
        {
            return ShouldDrawHeldItem(drawPlayer.inventory[drawPlayer.selectedItem], drawPlayer.itemAnimation, drawPlayer.wet, drawPlayer.dead);
        }

        /*
         * Returns true if the requirements for drawing the held item are satisfied.
         * 
         * isDead : should be false, is for players mostly.
         */
        public static bool ShouldDrawHeldItem(Item item, int itemAnimation, bool isWet, bool isDead = false)
        {
            return ((itemAnimation > 0 || item.holdStyle > 0) && item.type > ItemID.None && !isDead && !item.noUseGraphic && (!isWet || !item.noWet));
        }

        /*
         * Draw a weapon in a sword-like fashion. (ie only when used, centered and rotating)
         * 
         * Returns : the value for LetDraw.
         * wepColor : weapon's tint.
         * XOffset / YOffset : Offsets the sword's position on the X/Y axis.
         */
        public static bool DrawHeldSword(object sb, int shader, Player drawPlayer, Color lightColor = default, float scale = 0f, float xOffset = 0, float yOffset = 0, Rectangle? frame = null, int frameCount = 1, Texture2D overrideTex = null)
        {
            if(ShouldDrawHeldItem(drawPlayer))
            {
                Item item = drawPlayer.inventory[drawPlayer.selectedItem];
                DrawHeldSword(sb, (overrideTex != null ? overrideTex : Main.itemTexture[item.type]), shader, drawPlayer.itemLocation, item, drawPlayer.direction, drawPlayer.itemRotation, scale <= 0f ? item.scale : scale, lightColor, item.color, xOffset, yOffset, drawPlayer.gravDir, drawPlayer, frame, frameCount);
                return false;
            }
            return true;
        }

        /*
         * Draw a texture in a sword-like fashion. (ie only when used, centered and rotating)
         *
         * wepColor : weapon's tint.
         * XOffset / YOffset : Offsets the sword's position on the X/Y axis.
         */
        public static void DrawHeldSword(object sb, Texture2D tex, int shader, Vector2 position, Item item, int direction, float itemRotation, float itemScale, Color lightColor = default, Color wepColor = default, float xOffset = 0, float yOffset = 0, float gravDir = -1f, Entity entity = null, Rectangle? frame = null, int frameCount = 1)
        {
            if (frame == null) { frame = new Rectangle(0, 0, tex.Width, tex.Height); }
            if (lightColor == default) { lightColor = GetLightColor(position); }
            xOffset *= direction;
            SpriteEffects spriteEffect = direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (gravDir == -1f) { yOffset *= -1; spriteEffect |= SpriteEffects.FlipVertically; }
			if(entity is Player)
			{
				Player drawPlayer = (Player)entity; yOffset -= drawPlayer.gfxOffY;			
			}else
			if(entity is NPC)
			{
				NPC drawNPC = (NPC)entity; yOffset -= drawNPC.gfxOffY;
			}
            int drawType = item.type;

            Vector2 drawPos = position - Main.screenPosition;
            Vector2 texOrigin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f / frameCount);
            Vector2 rotOrigin = new Vector2((texOrigin.X - texOrigin.X * direction), gravDir == -1f ? 0 : tex.Height) + new Vector2(xOffset, -yOffset);

            if (gravDir == -1f) //reverse gravity
            {
				if (sb is List<DrawData>)
				{
					DrawData dd = new DrawData(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
					dd.shader = shader;
					((List<DrawData>)sb).Add(dd);
				}else
				if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
				
				if (wepColor != default)
				{
					if (sb is List<DrawData>)
					{
						DrawData dd = new DrawData(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
						dd.shader = shader;
						((List<DrawData>)sb).Add(dd);
					}else
					if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
				}
            }else //normal gravity
            {
                if (drawType == 425 || drawType == 507)
                {
                    if (direction == 1) { spriteEffect = SpriteEffects.FlipVertically; } else { spriteEffect = (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically); }
                }
				if (sb is List<DrawData>)
				{
					DrawData dd = new DrawData(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
					dd.shader = shader;
					((List<DrawData>)sb).Add(dd);
				}else
				if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                
				if (wepColor != default)
                {
					if (sb is List<DrawData>)
					{
						DrawData dd = new DrawData(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
						dd.shader = shader;
						((List<DrawData>)sb).Add(dd);
					}else
					if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                }
            }
        }


        public static bool DrawHeldGun(object sb, int shader, Player drawPlayer, Color lightColor = default, float scale = 0f, float xOffset = 0, float yOffset = 0, bool shakeX = false, bool shakeY = false, float shakeScalarX = 1.0f, float shakeScalarY = 1.0f, Rectangle? frame = null, int frameCount = 1, Texture2D overrideTex = null)
        {
            if(ShouldDrawHeldItem(drawPlayer))
            {
                Item item = drawPlayer.inventory[drawPlayer.selectedItem];
                DrawHeldGun(sb, (overrideTex != null ? overrideTex : Main.itemTexture[item.type]), shader, drawPlayer.itemLocation, item, drawPlayer.direction, drawPlayer.itemRotation, scale <= 0f ? item.scale : scale, lightColor, item.color, xOffset, yOffset, shakeX, shakeY, shakeScalarX, shakeScalarY, drawPlayer.gravDir, drawPlayer, frame, frameCount);
                return false;
            }
            return true;
        }

        /*
         * Draws a texture in a gun-like fashion. (ie only when used and in the direction of the cursor)
         * 
         * direction : the direction the sprite should point. (-1 for left, 1 for right)
         * itemRotation : Rotation of the item.
         * itemScale : Scale of the item.
         * lightColor : color of the light the weapon is at.
         * wepColor : weapon's tint.
         * XOffset / YOffset : Offsets the gun's position on the X/Y axis.
         * shakeX / shakeY : If true, shakes the sprite on the X/Y axis.
         * shakeScaleX / shakeScaleY : If shakeX/shakeY is true, this scales the amount it shakes by.
         * gravDir : the direction of gravity.
         * entity : If drawing for a player or npc, the instance of them. (can be null)
         */
        public static void DrawHeldGun(object sb, Texture2D tex, int shader, Vector2 position, Item item, int direction, float itemRotation, float itemScale, Color lightColor = default, Color wepColor = default, float xOffset = 0, float yOffset = 0, bool shakeX = false, bool shakeY = false, float shakeScalarX = 1.0f, float shakeScalarY = 1.0f, float gravDir = 1f, Entity entity = null, Rectangle? frame = null, int frameCount = 1)
        {
            if(frame == null){ frame = new Rectangle(0, 0, tex.Width, tex.Height); }
            if(lightColor == default){ lightColor = GetLightColor(position); }
            SpriteEffects spriteEffect = direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if(gravDir == -1f){ yOffset *= -1; spriteEffect |= SpriteEffects.FlipVertically; }
            int type = item.type;
            int fakeType = type;
            Vector2 texOrigin = new Vector2(tex.Width / 2, (float)(tex.Height / 2) / frameCount);
			if(entity is Player)
			{
				Player drawPlayer = (Player)entity; yOffset += drawPlayer.gfxOffY;
			}else
			if(entity is NPC)
			{
				NPC drawNPC = (NPC)entity; yOffset += drawNPC.gfxOffY;
			}			
            Vector2 rotOrigin = new Vector2(-xOffset, ((float)(tex.Height / 2) / frameCount) - yOffset);
            if(direction == -1)
            {
                rotOrigin = new Vector2(tex.Width + xOffset, ((float)(tex.Height / 2) / frameCount) - yOffset);
            }
            Vector2 pos = new Vector2((int)(position.X - Main.screenPosition.X + texOrigin.X), (int)(position.Y - Main.screenPosition.Y + texOrigin.Y));

            if (shakeX) { pos.X += shakeScalarX * (Main.rand.Next(-5, 6) / 9f); }
            if (shakeY) { pos.Y += shakeScalarY * (Main.rand.Next(-5, 6) / 9f); }

			if (sb is List<DrawData>)
			{
				DrawData dd = new DrawData(tex, pos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
				dd.shader = shader;
				((List<DrawData>)sb).Add(dd);
			}else
			if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(tex, pos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
            
			if (wepColor != default)
            {
				if (sb is List<DrawData>)
				{
					DrawData dd = new DrawData(tex, pos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
					dd.shader = shader;
					((List<DrawData>)sb).Add(dd);
				}else
				if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(tex, pos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
            }
            try { if (type != fakeType) { item.type = type; } }
            catch { }
        }

        /*
         * Draws the given texture in a spear-like fashion (texture is oriented at the upper-right corner) using the projectile provided.
         */
        public static void DrawProjectileSpear(object sb, Texture2D texture, int shader, Projectile p, Color? overrideColor = null, float offsetX = 0f, float offsetY = 0f)
        {
            offsetX += (-texture.Width * 0.5f);
            Color lightColor = overrideColor != null ? (Color)overrideColor : p.GetAlpha(GetLightColor(Main.player[p.owner].Center));
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			offsetY -= Main.player[p.owner].gfxOffY;		
            Vector2 offset = BaseUtility.RotateVector(p.Center, p.Center + new Vector2(p.direction == -1 ? offsetX : offsetY, p.direction == 1 ? offsetX : offsetY), p.rotation - 2.355f) - p.Center;
			if (sb is List<DrawData>)
			{
				DrawData dd = new DrawData(texture, p.Center - Main.screenPosition + offset, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, p.rotation, origin, p.scale, p.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
				dd.shader = shader;
				((List<DrawData>)sb).Add(dd);
			}else
			if (sb is SpriteBatch) ((SpriteBatch)sb).Draw(texture, p.Center - Main.screenPosition + offset, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, p.rotation, origin, p.scale, p.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

		public static void DrawAura(object sb, Texture2D texture, int shader, Entity codable, float auraPercent, float distanceScalar = 1f, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null)
		{
			int frameCount = (codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1);
			Rectangle frame = (codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Height, texture.Width));
			float scale = (codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale);
			float rotation = (codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation);
			int spriteDirection = (codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection);
			float offsetY2 = (codable is NPC ? ((NPC)codable).gfxOffY : 0f);
			DrawAura(sb, texture, shader, codable.position + new Vector2(0f, offsetY2), codable.width, codable.height, auraPercent, distanceScalar, scale, rotation, spriteDirection, frameCount, frame, offsetX, offsetY, overrideColor);
		}

		public static void DrawAura(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float auraPercent, float distanceScalar = 1f, float scale = 1f, float rotation = 0f, int direction = 0, int framecount = 1, Rectangle frame = default(Rectangle), float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null)
		{
			Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
			float percentHalf = auraPercent * 5f * distanceScalar;
			float percentLight = MathHelper.Lerp(0.8f, 0.2f, auraPercent);
			lightColor.R = (byte)(lightColor.R * percentLight);
			lightColor.G = (byte)(lightColor.G * percentLight);
			lightColor.B = (byte)(lightColor.B * percentLight);
			lightColor.A = (byte)(lightColor.A * percentLight);
			Vector2 position2 = position;
			for (int m = 0; m < 4; m++)
			{
				float offX = offsetX;
				float offY = offsetY;
				switch (m)
				{
					case 0: offX += percentHalf; break;
					case 1: offX -= percentHalf; break;
					case 2: offY += percentHalf; break;
					case 3: offY -= percentHalf; break;
				}
				position2 = new Vector2(position.X + offX, position.Y + offY);
				DrawTexture(sb, texture, shader, position2, width, height, scale, rotation, direction, framecount, frame, lightColor);	
			}
		}

		public static void DrawYoyoLine(SpriteBatch sb, Projectile projectile, Texture2D overrideTex = null, Color? overrideColor = null)
		{
			DrawYoyoLine(sb, projectile, Main.player[projectile.owner], projectile.Center, Main.player[projectile.owner].MountedCenter, overrideTex, overrideColor);
		}

		public static void DrawYoyoLine(SpriteBatch sb, Projectile projectile, Entity owner,  Vector2 yoyoLoc, Vector2 connectionLoc, Texture2D overrideTex = null, Color? overrideColor = null)
		{
			Vector2 mountedCenter = connectionLoc;
			if(owner is Player) mountedCenter.Y += Main.player[projectile.owner].gfxOffY;
			float centerDistX = yoyoLoc.X - mountedCenter.X;
			float centerDistY = yoyoLoc.Y - mountedCenter.Y;
			Math.Sqrt(centerDistX * centerDistX + centerDistY * centerDistY);
			float rotation = (float)Math.Atan2(centerDistY, centerDistX) - 1.57f;
			if (owner is Player && !projectile.counterweight)
			{
				int projDir = -1;
				if (projectile.position.X + projectile.width / 2 < Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2) projDir = 1;
				projDir *= -1;
				((Player)owner).itemRotation = (float)Math.Atan2(centerDistY * projDir, centerDistX * projDir);
			}
			bool flag = true;
			if (centerDistX == 0f && centerDistY == 0f){ flag = false; }else
			{
				float sqrtCenter = (float)Math.Sqrt(centerDistX * centerDistX + centerDistY * centerDistY);
				sqrtCenter = 12f / sqrtCenter;
				centerDistX *= sqrtCenter;
				centerDistY *= sqrtCenter;
				mountedCenter.X -= centerDistX * 0.1f;
				mountedCenter.Y -= centerDistY * 0.1f;
				centerDistX = yoyoLoc.X - mountedCenter.X;
				centerDistY = yoyoLoc.Y - mountedCenter.Y;
			}
			while (flag)
			{
				float textureHeight = 12f;
				float sqrtCenter = (float)Math.Sqrt(centerDistX * centerDistX + centerDistY * centerDistY);
				float sqrtCenter2 = sqrtCenter;
				if (float.IsNaN(sqrtCenter) || float.IsNaN(sqrtCenter2)){ flag = false; }
				else
				{
					if (sqrtCenter < 20f){ textureHeight = sqrtCenter - 8f; flag = false; }
					sqrtCenter = 12f / sqrtCenter;
					centerDistX *= sqrtCenter;
					centerDistY *= sqrtCenter;
					mountedCenter.X += centerDistX;
					mountedCenter.Y += centerDistY;
					centerDistX = yoyoLoc.X - mountedCenter.X;
					centerDistY = yoyoLoc.Y - mountedCenter.Y;
					if (sqrtCenter2 > 12f)
					{
						float scalar = 0.3f;
						float velocityAverage = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
						if (velocityAverage > 16f) velocityAverage = 16f;
						velocityAverage = 1f - velocityAverage / 16f;
						scalar *= velocityAverage;
						velocityAverage = sqrtCenter2 / 80f;
						if (velocityAverage > 1f) velocityAverage = 1f;
						scalar *= velocityAverage;
						if (scalar < 0f) scalar = 0f;
						scalar *= velocityAverage;
						scalar *= 0.5f;
						if (centerDistY > 0f)
						{
							centerDistY *= 1f + scalar;
							centerDistX *= 1f - scalar;
						}else
						{
							velocityAverage = Math.Abs(projectile.velocity.X) / 3f;
							if (velocityAverage > 1f) velocityAverage = 1f;
							velocityAverage -= 0.5f;
							scalar *= velocityAverage;
							if (scalar > 0f) scalar *= 2f;
							centerDistY *= 1f + scalar;
							centerDistX *= 1f - scalar;
						}
					}
					rotation = (float)Math.Atan2(centerDistY, centerDistX) - 1.57f;
					int stringColor = Main.player[projectile.owner].stringColor;
					Color color = (overrideColor != null && stringColor <= 0 ? (Color)overrideColor : WorldGen.paintColor(stringColor));
					if (color.R < 75) color.R = 75; if (color.G < 75) color.G = 75; if (color.B < 75) color.B = 75;
					if (stringColor == 13){ color = new Color(20, 20, 20); }
					else if (stringColor == 14 || stringColor == 0){ color = new Color(200, 200, 200); }
					else if (stringColor == 28){ color = new Color(163, 116, 91); }
					else if (stringColor == 27){ color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB); }
					color.A = (byte)(color.A * 0.4f);
					float colorScalar = 0.5f;
					if(overrideColor == null)
					{
						color = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f), color);
						color = new Microsoft.Xna.Framework.Color((byte)(color.R * colorScalar), (byte)(color.G * colorScalar), (byte)(color.B * colorScalar), (byte)(color.A * colorScalar));
					}
					Texture2D tex = (overrideTex != null ? overrideTex : Main.fishingLineTexture);
					Vector2 texCenter = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);	
					Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(mountedCenter.X - Main.screenPosition.X + texCenter.X, mountedCenter.Y - Main.screenPosition.Y + texCenter.Y) - new Vector2(6f, 0f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, tex.Width, (int)textureHeight)), color, rotation, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
				}
			}
		}
		
        /*
          * Draws a fishing line from the given projectile bobber to the player owning it.
          */
		public static void DrawFishingLine(SpriteBatch sb, Projectile projectile, Vector2 rodLoc, Vector2 bobberLoc, Texture2D overrideTex = null, Color? overrideColor = null)
		{
			Player player = Main.player[projectile.owner];
			if (projectile.bobber && Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].holdStyle > 0)
			{
				float mountedCenterX = player.MountedCenter.X;
				float mountedCenterY = player.MountedCenter.Y;
				mountedCenterY += Main.player[projectile.owner].gfxOffY;
				int type = Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type;
				float gravDir = Main.player[projectile.owner].gravDir;

				mountedCenterX += rodLoc.X * Main.player[projectile.owner].direction;
				if (Main.player[projectile.owner].direction < 0) mountedCenterX -= 13f;
				mountedCenterY -= rodLoc.Y * gravDir;
				
				if (gravDir == -1f) mountedCenterY -= 12f;
				Vector2 mountedCenter = new Vector2(mountedCenterX, mountedCenterY);
				mountedCenter = Main.player[projectile.owner].RotatedRelativePoint(mountedCenter + new Vector2(8f), true) - new Vector2(8f);
				float projLineCenterX = projectile.position.X + projectile.width * 0.5f - mountedCenter.X;
				float projLineCenterY = projectile.position.Y + projectile.height * 0.5f - mountedCenter.Y;
                projLineCenterX += bobberLoc.X; projLineCenterY += bobberLoc.Y;
				Math.Sqrt(projLineCenterX * projLineCenterX + projLineCenterY * projLineCenterY);
				float rotation2 = (float)Math.Atan2(projLineCenterY, projLineCenterX) - 1.57f;
				bool flag2 = true;
				if (projLineCenterX == 0f && projLineCenterY == 0f){ flag2 = false;}else
				{
					float num15 = (float)Math.Sqrt(projLineCenterX * projLineCenterX + projLineCenterY * projLineCenterY);
					num15 = 12f / num15;
					projLineCenterX *= num15;
					projLineCenterY *= num15;
					mountedCenter.X -= projLineCenterX;
					mountedCenter.Y -= projLineCenterY;
					projLineCenterX = projectile.position.X + projectile.width * 0.5f - mountedCenter.X;
					projLineCenterY = projectile.position.Y + projectile.height * 0.5f - mountedCenter.Y;
				}
				while (flag2)
				{
					float num16 = 12f;
					float num17 = (float)Math.Sqrt(projLineCenterX * projLineCenterX + projLineCenterY * projLineCenterY);
					float num18 = num17;
					if (float.IsNaN(num17) || float.IsNaN(num18)){ flag2 = false; }else
					{
						if (num17 < 20f)
						{
							num16 = num17 - 8f;
							flag2 = false;
						}
						num17 = 12f / num17;
						projLineCenterX *= num17;
						projLineCenterY *= num17;
						mountedCenter.X += projLineCenterX;
						mountedCenter.Y += projLineCenterY;
						projLineCenterX = projectile.position.X + projectile.width * 0.5f - mountedCenter.X;
						projLineCenterY = projectile.position.Y + projectile.height * 0.1f - mountedCenter.Y;
						if (num18 > 12f)
						{
							float num19 = 0.3f;
							float num20 = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
							if (num20 > 16f) num20 = 16f;
							num20 = 1f - num20 / 16f;
							num19 *= num20;
							num20 = num18 / 80f;
							if (num20 > 1f) num20 = 1f;
							num19 *= num20;
							if (num19 < 0f) num19 = 0f;
							num20 = 1f - projectile.localAI[0] / 100f;
							num19 *= num20;
							if (projLineCenterY > 0f)
							{
								projLineCenterY *= 1f + num19;
								projLineCenterX *= 1f - num19;
							}else
							{
								num20 = Math.Abs(projectile.velocity.X) / 3f;
								if (num20 > 1f) num20 = 1f;
								num20 -= 0.5f;
								num19 *= num20;
								if (num19 > 0f) num19 *= 2f;
								projLineCenterY *= 1f + num19;
								projLineCenterX *= 1f - num19;
							}
						}
						rotation2 = (float)Math.Atan2(projLineCenterY, projLineCenterX) - 1.57f;
						Color color2 = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f), (overrideColor != null ? (Color)overrideColor : new Color(200, 200, 200, 100)));
						Texture2D tex = (overrideTex != null ? overrideTex : Main.fishingLineTexture);
						Vector2 texCenter = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);
						sb.Draw(tex, new Vector2(mountedCenter.X - Main.screenPosition.X + texCenter.X * 0.5f, mountedCenter.Y - Main.screenPosition.Y + texCenter.Y * 0.5f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, tex.Width, (int)num16)), color2, rotation2, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
					}
				}
			}
		}

        /*
         * Draws the given texture multiple times with each one being farther away and more faded depending on velocity.
         * Uses a Entity(NPC/Projectile) for width, height, position, rotation, sprite direction, and velocity. If an npc, also uses framecount and frame.
         */
        public static void DrawAfterimage(object sb, Texture2D texture, int shader, Entity codable, float distanceScalar = 1.0F, float sizeScalar = 1.0f, int imageCount = 7, bool useOldPos = true, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null, Rectangle? overrideFrame = null, int overrideFrameCount = 0)
        {
            int frameCount = (overrideFrameCount > 0 ? overrideFrameCount : codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1);
            Rectangle frame = (overrideFrame != null ? (Rectangle)overrideFrame : codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height));
            float scale = (codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale);
			float rotation = (codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation);
            int spriteDirection = (codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection);
            Vector2[] velocities = new Vector2[] { codable.velocity };
            if(useOldPos)
            {
                velocities = (codable is NPC ? ((NPC)codable).oldPos : ((Projectile)codable).oldPos);
            }
			float offsetY2 = (codable is NPC ? ((NPC)codable).gfxOffY : 0f);
            DrawAfterimage(sb, texture, shader, codable.position + new Vector2(0f, offsetY2), codable.width, codable.height, velocities, scale, rotation, spriteDirection, frameCount, frame, distanceScalar, sizeScalar, imageCount, useOldPos, offsetX, offsetY, overrideColor);
        }

        /*
         * Draws the given texture multiple times with each one being farther away and more faded depending on velocity.
         * 
         * oldPoints : an array of points used to draw the afterimage.
         * distanceScalar : How far away from each other each image is.
         * sizeScalar : the amount to scale by for each image. (NOTE: this is ADDITIVE!)
         * fullbright : If the images are fullbright or not.
         * alphaAmt : The amount of alpha to subtract with each image. (0-255)
         * imageCount : How many images to draw.
         * useOldPos : If true, considers the given array as old positions instead of old oldPoints.
         */
        public static void DrawAfterimage(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, Vector2[] oldPoints, float scale = 1f, float rotation = 0f, int direction = 0, int framecount = 1, Rectangle frame = default(Rectangle), float distanceScalar = 1.0F, float sizeScalar = 1f, int imageCount = 7, bool useOldPos = true, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / framecount / 2);
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
            Vector2 velAddon = default;
            Vector2 originalpos = position;
			Vector2 offset = new Vector2(offsetX, offsetY);
            for(int m = 1; m <= imageCount; m++)
            {
                scale *= sizeScalar;
                Color newLightColor = lightColor;
                newLightColor.R = (byte)(newLightColor.R * (imageCount + 3 - m) / (imageCount + 9));
                newLightColor.G = (byte)(newLightColor.G * (imageCount + 3 - m) / (imageCount + 9));
                newLightColor.B = (byte)(newLightColor.B * (imageCount + 3 - m) / (imageCount + 9));
                newLightColor.A = (byte)(newLightColor.A * (imageCount + 3 - m) / (imageCount + 9));
                if (useOldPos)
                {
                    position = Vector2.Lerp(originalpos, (m - 1 >= oldPoints.Length ? oldPoints[oldPoints.Length - 1] : oldPoints[m - 1]), distanceScalar);
					DrawTexture(sb, texture, shader, position + offset, width, height, scale, rotation, direction, framecount, frame, newLightColor);
                }else
                {
                    Vector2 velocity = (m - 1 >= oldPoints.Length ? oldPoints[oldPoints.Length - 1] : oldPoints[m - 1]);
                    velAddon += velocity * distanceScalar;
					DrawTexture(sb, texture, shader, position + offset - velAddon, width, height, scale, rotation, direction, framecount, frame, newLightColor);
                }
            }
        }

        public static void DrawChain(object sb, Texture2D texture, int shader, Vector2 start, Vector2 end, float Jump = 0f, Color? overrideColor = null, float scale = 1f, bool drawEndsUnder = false, Func<Texture2D, Vector2, Vector2, Vector2, Rectangle, Color, float, float, int, bool> OnDrawTex = null)
        {
			DrawChain(sb, new Texture2D[]{ texture, texture, texture }, shader, start, end, Jump, overrideColor, scale, drawEndsUnder, OnDrawTex);
		}

        //code written by Yoraiz0r, heavily edited by GroxTheGreat
        /*
         * Draws a chain from the start position to the end position using the texture provided.
         * 
         * textures : an array of 3 textures: the 'start' texture, the segment texture and the 'end' texture.
         * start : the starting point of the chain.
         * end : the ending point of the chain.
         * Jump : The amount to 'jump' to draw the next piece of chain. If -1, will use the texture height.
         * overrideColor : the color to draw the chain with.
         * scale : the scalar of the chain.
         * drawEndsUnder : If true, the end textures will be drawn under the segment texture. Otherwise, drawn above it.
         * OnDrawTex : If not null, called when the chain draws a texture. Return true to draw the chain piece, false to not draw it. Parameters, in order:
         *             1 - The texture.
         *             2 - The world position of the chain.
         *             3 - The draw position of the chain.
         *             4 - The center of the texture.
         *             5 - The frame of the texture being used.
         *             6 - The color the texture is being drawn.
         *             7 - The rotation of the chain.
         *             8 - The scale of the chain.
         *             9 - The count of this chain piece in the entire thing. (-1 for start tex, -2 for end tex)
         */
        public static void DrawChain(object sb, Texture2D[] textures, int shader, Vector2 start, Vector2 end, float Jump = 0f, Color? overrideColor = null, float scale = 1f, bool drawEndsUnder = false, Func<Texture2D, Vector2, Vector2, Vector2, Rectangle, Color, float, float, int, bool> OnDrawTex = null)
        {
            if(Jump <= 0f){ Jump = (textures[1].Height - 2f) * scale; }
            Vector2 dir = end - start;
            dir.Normalize();
            float length = Vector2.Distance(start, end);
            float Way = 0f;
            float rotation = BaseUtility.RotationTo(start, end) - 1.57f;
            int texID = 0;
            int maxTextures = textures.Length - 2;
            int currentChain = 0;
            while (Way < length)
            {
                float texWidth;
                float texHeight;
                Vector2 texCenter;
                Vector2 v;
                Color lightColor;
                Action drawEnds = () =>
                {
                    if (textures[0] != null && Way == 0f)
                    {
                        float texWidth2 = textures[0].Width;
                        float texHeight2 = textures[0].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = start - Main.screenPosition + texCenter2;
                        Color lightColor2 = (overrideColor != null ? (Color)overrideColor : GetLightColor(start + texCenter2));
                        if (OnDrawTex != null && !OnDrawTex(textures[0], start + texCenter2, v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -1)) { }
                        else
                        {
							if (sb is List<DrawData>)
							{
								DrawData dd = new DrawData(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
								dd.shader = shader;
								((List<DrawData>)sb).Add(dd);
							}else
							if (sb is SpriteBatch)
							{
								((SpriteBatch)sb).Draw(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
							}
                        }
                    }
                    if (textures[maxTextures + 1] != null && Way + Jump >= length)
                    {
                        float texWidth2 = textures[maxTextures + 1].Width;
                        float texHeight2 = textures[maxTextures + 1].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = end - Main.screenPosition + texCenter2;
                        Color lightColor2 = (overrideColor != null ? (Color)overrideColor : GetLightColor(end + texCenter2));
                        if (OnDrawTex != null && !OnDrawTex(textures[maxTextures + 1], end + texCenter2,  v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -2)) { }
                        else
                        {
							if (sb is List<DrawData>)
							{
								DrawData dd = new DrawData(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
								dd.shader = shader;
								((List<DrawData>)sb).Add(dd);
							}else
							if (sb is SpriteBatch)
							{
								((SpriteBatch)sb).Draw(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
							}
						}
                    }
                };
                texWidth = textures[1].Width;
                texHeight = textures[1].Height;
                texCenter = new Vector2(texWidth / 2f, texHeight / 2f) * scale;
				
				v = (start + dir * Way) + texCenter;
				if(InDrawZone(v))
				{
					v -= Main.screenPosition;
					if((Way == 0f || Way + Jump >= length) && drawEndsUnder){ drawEnds(); }
					lightColor = (overrideColor != null ? (Color)overrideColor : GetLightColor((start + dir * Way) + texCenter));
					texID++;
					if(texID >= maxTextures){ texID = 0; }
					if (OnDrawTex != null && !OnDrawTex(textures[texID + 1], (start + dir * Way) + texCenter, v - texCenter, texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, scale, currentChain)) { }
					else
					{
						if (sb is List<DrawData>)
						{
							DrawData dd = new DrawData(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0);
							dd.shader = shader;
							((List<DrawData>)sb).Add(dd);
						}else
						if (sb is SpriteBatch)
						{
							((SpriteBatch)sb).Draw(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0);
						}
					}
					currentChain++;
					if((Way == 0f || Way + Jump >= length) && !drawEndsUnder){ drawEnds(); }
				}
				Way += Jump;
            }
        }
	
        public static void DrawVectorChain(object sb, Texture2D[] textures, int shader, Vector2[] chain, float Jump = 0f, Color? overrideColor = null, float scale = 1f, bool drawEndsUnder = false, Func<Texture2D, Vector2, Vector2, Vector2, Rectangle, Color, float, float, int, bool> OnDrawTex = null)
        {
            if(Jump <= 0f){ Jump = (textures[1].Height - 2f) * scale; }

            float length = 0f;	
			for(int m = 0; m < chain.Length - 1; m++)
			{
				length += Vector2.Distance(chain[m], chain[m + 1]);
			}
			Vector2 start = chain[0];
			Vector2 end = chain[chain.Length - 1];
            Vector2 dir = end - start;
            dir.Normalize();			
            float Way = 0f;
            float rotation = BaseUtility.RotationTo(chain[0], chain[1]) - 1.57f;
            int texID = 0;
            int maxTextures = textures.Length - 2;
            int currentChain = 0;
			Vector2 lastV = chain[0];
            while (Way < length)
            {
                float texWidth;
                float texHeight;
                Vector2 texCenter;
                Vector2 v;
                Color lightColor;
                Action drawEnds = () =>
                {
                    if (textures[0] != null && Way == 0f)
                    {
                        float texWidth2 = textures[0].Width;
                        float texHeight2 = textures[0].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = start - Main.screenPosition + texCenter2;
                        Color lightColor2 = (overrideColor != null ? (Color)overrideColor : GetLightColor(start + texCenter2));
                        if (OnDrawTex != null && !OnDrawTex(textures[0], start + texCenter2, v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -1)) { }
                        else
                        {
							if (sb is List<DrawData>)
							{
								DrawData dd = new DrawData(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
								dd.shader = shader;
								((List<DrawData>)sb).Add(dd);
							}else
							if (sb is SpriteBatch)
							{
								((SpriteBatch)sb).Draw(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
							}
                        }
                    }
                    if (textures[maxTextures + 1] != null && Way + Jump >= length)
                    {
                        float texWidth2 = textures[maxTextures + 1].Width;
                        float texHeight2 = textures[maxTextures + 1].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = end - Main.screenPosition + texCenter2;
                        Color lightColor2 = (overrideColor != null ? (Color)overrideColor : GetLightColor(end + texCenter2));
                        if (OnDrawTex != null && !OnDrawTex(textures[maxTextures + 1], end + texCenter2,  v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -2)) { }
                        else
                        {
							if (sb is List<DrawData>)
							{
								DrawData dd = new DrawData(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
								dd.shader = shader;
								((List<DrawData>)sb).Add(dd);
							}else
							if (sb is SpriteBatch)
							{
								((SpriteBatch)sb).Draw(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
							}
						}
                    }
                };
                texWidth = textures[1].Width;
                texHeight = textures[1].Height;
                texCenter = new Vector2(texWidth / 2f, texHeight / 2f) * scale;

				v = BaseUtility.MultiLerpVector(Way / length, chain) + texCenter;
				Vector2 nextV = BaseUtility.MultiLerpVector(Math.Max(length - 1, Way + 1) / length, chain) + texCenter;
				if(v != nextV)
				{
					rotation = BaseUtility.RotationTo(v, nextV) - 1.57f;
				}

				if(InDrawZone(v))
				{
					v -= Main.screenPosition;
					if((Way == 0f || Way + Jump >= length) && drawEndsUnder){ drawEnds(); }
					lightColor = (overrideColor != null ? (Color)overrideColor : GetLightColor((start + dir * Way) + texCenter));
					texID++;
					if(texID >= maxTextures){ texID = 0; }
					if (OnDrawTex != null && !OnDrawTex(textures[texID + 1], (start + dir * Way) + texCenter, v - texCenter, texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, scale, currentChain)) { }
					else
					{
						if (sb is List<DrawData>)
						{
							DrawData dd = new DrawData(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0);
							dd.shader = shader;
							((List<DrawData>)sb).Add(dd);
						}else
						if (sb is SpriteBatch)
						{
							((SpriteBatch)sb).Draw(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0);
						}
					}
					currentChain++;
					if((Way == 0f || Way + Jump >= length) && !drawEndsUnder){ drawEnds(); }
				}
				Way += Jump;
            }
        }		
		
		

        /*
         * Draws the given texture using the override color.
         * Uses a Entity for width, height, position, rotation, and sprite direction.
         */
        public static void DrawTexture(object sb, Texture2D texture, int shader, Entity codable, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
			DrawTexture(sb, texture, shader, codable, 1, overrideColor, drawCentered, overrideOrigin);
		}

        /*
         * Draws the given texture using the override color.
         * Uses a Entity for width, height, position, rotation, and sprite direction.
         */
        public static void DrawTexture(object sb, Texture2D texture, int shader, Entity codable, int framecountX, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
            Color lightColor = (overrideColor != null ? (Color)overrideColor : codable is Item ? ((Item)codable).GetAlpha(GetLightColor(codable.Center)) : codable is NPC ? GetNPCColor(((NPC)codable), codable.Center, false) : codable is Projectile ? ((Projectile)codable).GetAlpha(GetLightColor(codable.Center)) : GetLightColor(codable.Center));
            int frameCount = (codable is Item ? 1 : codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1);
            Rectangle frame = (codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height));
            float scale = (codable is Item ? ((Item)codable).scale : codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale);
            float rotation = (codable is Item ? 0 : codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation);
            int spriteDirection = (codable is Item ? 1 : codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection);
			float offsetY = (codable is NPC ? ((NPC)codable).gfxOffY : 0f);
            DrawTexture(sb, texture, shader, codable.position + new Vector2(0f, offsetY), codable.width, codable.height, scale, rotation, spriteDirection, frameCount, framecountX, frame, lightColor, drawCentered, overrideOrigin);
        }

        public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, Rectangle frame, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
			DrawTexture(sb, texture, shader, position, width, height, scale, rotation, direction, framecount, 1, frame, overrideColor, drawCentered, overrideOrigin);
		}

        /*
         * Draws the given texture using lighting nearby, or the overriden color given.
         */
        public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, int framecountX, Rectangle frame, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
            Vector2 origin = overrideOrigin != default ? overrideOrigin : new Vector2(frame.Width / framecountX / 2, texture.Height / framecount / 2);
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
			if (sb is List<DrawData>)
			{
				DrawData dd = new DrawData(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, frame, framecount, framecountX, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
				dd.shader = shader;
				((List<DrawData>)sb).Add(dd);
			}else if (sb is SpriteBatch)
			{
				bool applyDye = shader > 0;
				if (applyDye)
				{
					((SpriteBatch)sb).End();
					((SpriteBatch)sb).Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
					GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);				
				}
				((SpriteBatch)sb).Draw(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, frame, framecount, framecountX, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);				
				if (applyDye)
				{
					((SpriteBatch)sb).End();
					((SpriteBatch)sb).Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				}
			}
        }
		
        /*
         * Debug draw method, draws a hitbox with absolutes, not taking into account anything else.
         */
        public static void DrawHitbox(SpriteBatch sb, Rectangle hitbox, Color? overrideColor = null)
        {
            Vector2 origin = default;
            Color lightColor = (overrideColor != null ? (Color)overrideColor : Color.White);
			Vector2 position = new Vector2(hitbox.Left, hitbox.Top) - Main.screenPosition; 
			sb.Draw(Main.magicPixel, position, hitbox, lightColor, 0f, origin, 1f, SpriteEffects.None, 0);
        }		

		public static void DrawTileTexture(SpriteBatch sb, Texture2D texture, int x, int y, bool slopeDraw = true, bool flipTex = false, bool ignoreHalfBricks = false, bool? overrideHalfBrick = null, Func<Color, Color> overrideColor = null, Vector2 offset = default)
		{
			Tile tile = Main.tile[x, y]; 
			int frameX = (tile != null && tile.active() ? tile.frameX : 0);
			int frameY = (tile != null && tile.active() ? tile.frameY : 0);
			DrawTileTexture(sb, texture, x, y, 16, 16, frameX, frameY, slopeDraw, flipTex, ignoreHalfBricks, overrideHalfBrick, overrideColor, offset);
		}

		/*
		 * Draws the given texture using a tile and the lighting nearby, or the overrideColor.
		 * 
		 * x/y : the x/y cooridinates of the tile.
		 * fwidth/fheight : The width and height of one tile. (usually this is 16)
		 * slopeDraw : If true, draw slopelike textures if the tile is sloped.
		 * flipTex : If true, flips the texture on the X axis.
		 * ignoreHalfBricks : If true, ignores half bricks and will not merge with them. THIS IS NOT NORMAL BEHAVIOR!
		 * overrideHalfBrick : If not null, overrides wether or not this is a halfbrick tile by the value given.
		 * overrideColor : An override color used to override the color drawn with.
		 */
		public static void DrawTileTexture(SpriteBatch sb, Texture2D texture, int x, int y, int fwidth = 16, int fheight = 16, int frameX = 0, int frameY = 0, bool slopeDraw = true, bool flipTex = false, bool ignoreHalfBricks = false, bool? overrideHalfBrick = null, Func<Color, Color> overrideColor = null, Vector2 offset = default)
		{
			Tile tile = Main.tile[x, y];
			//if(!tile.active()){ return; }
			bool halfBrick = (overrideHalfBrick != null ? (bool)overrideHalfBrick : tile.halfBrick());
			int halfBrickOffset = halfBrick ? 8 : 0;
			Color color = Lighting.GetColor(x, y);
			Vector2 drawOffset = (Main.drawToScreen ? default : new Vector2(Main.offScreenRange, Main.offScreenRange)) + offset;
			if (tile.inActive()){ color = tile.actColor(color); }
			SpriteEffects effects = (flipTex ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			Vector2 drawPos = GetTileDrawPosition(x, y, fwidth, fheight, drawOffset);
			int gfxCheck = (int)(255f * (1f - Main.gfxQuality) + 30f * Main.gfxQuality);
			int gfxCheck2 = (int)(50f * (1f - Main.gfxQuality) + 2f * Main.gfxQuality);	
			if (slopeDraw && tile.slope() > 0) //slopes
			{
				bool rightSlope = tile.rightSlope();
				bool topSlope = tile.topSlope();
				for (int m = 0; m < 8; m++)
				{
					int xOffset = (rightSlope ? (m * 2) : (16 - m * 2 - 2));
					int yOffset = (topSlope ? (m * 2) : 0);
					int frameOffsetX = xOffset;
					int height = 14 - m * 2;
					sb.Draw(texture, drawPos + new Vector2(xOffset, yOffset), new Rectangle(frameX + frameOffsetX, frameY, 2, height), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				}
				if (topSlope) sb.Draw(texture, drawPos + new Vector2(0f, 14f), new Rectangle(frameX, frameY + 14, 16, 2), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				else sb.Draw(texture, drawPos, new Rectangle(frameX, frameY, 16, 2), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
			}else //sidebricks
			if(!ignoreHalfBricks && Main.tileSolid[tile.type] && !halfBrick && (Main.tile[x - 1, y].halfBrick() || Main.tile[x + 1, y].halfBrick()))
			{
				if (Main.tile[x - 1, y].halfBrick() && Main.tile[x + 1, y].halfBrick())
				{
					sb.Draw(texture, drawPos + new Vector2(0f, 8f), new Rectangle(frameX, frameY + 8, fwidth, 8), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
					sb.Draw(texture, drawPos, new Rectangle(126, 0, 16, 8), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				}else
				if (Main.tile[x - 1, y].halfBrick())
				{
					sb.Draw(texture, drawPos + new Vector2(0f, 8f), new Rectangle(frameX, frameY + 8, fwidth, 8), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
					sb.Draw(texture, drawPos + new Vector2(4f, 0f), new Rectangle(frameX + 4, frameY, fwidth - 4, fheight), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
					sb.Draw(texture, drawPos, new Rectangle(126, 0, 4, 8), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				}else
				if (Main.tile[x + 1, y].halfBrick())
				{
					sb.Draw(texture, drawPos + new Vector2(0f, 8f), new Rectangle(frameX, frameY + 8, fwidth, 8), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
					sb.Draw(texture, drawPos, new Rectangle(frameX, frameY, fwidth - 4, fheight), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
					sb.Draw(texture, drawPos + new Vector2(12f, 0f), new Rectangle(138, 0, 4, 8), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				}else
				{
					sb.Draw(texture, drawPos, new Rectangle(frameX, frameY, fwidth, fheight), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				}
			}else
			if (Lighting.lightMode < 2 && Main.tileSolid[tile.type] && !halfBrick && !tile.inActive())
			{
				if (color.R > gfxCheck || color.G > gfxCheck * 1.1 || color.B > gfxCheck * 1.2)
				{
					Color[] lightArray = new Color[9];
					Lighting.GetColor9Slice(x, y, ref lightArray);
					for (int m = 0; m < 9; m++)
					{
						int offsetX = 0;
						int offsetY = 0;
						int width = 4;
						int height = 4;
						Color mixedColor = color;
						Color lightColor = lightArray[m];
						if (m == 1){ width = 8; offsetX = 4; }else
						if (m == 2){ offsetX = 12; }else
						if (m == 3){ height = 8; offsetY = 4; }else
						if (m == 4){ width = 8; height = 8; offsetX = 4; offsetY = 4; }else
						if (m == 5){ offsetX = 12; offsetY = 4; height = 8; }else
						if (m == 6){ offsetY = 12; }else
						if (m == 7){ width = 8; height = 4; offsetX = 4; offsetY = 12; }else
						if (m == 8){ offsetX = 12; offsetY = 12; }
						mixedColor.R = (byte)((color.R + lightColor.R) / 2);
						mixedColor.G = (byte)((color.G + lightColor.G) / 2);
						mixedColor.B = (byte)((color.B + lightColor.B) / 2);
						sb.Draw(texture, drawPos + new Vector2(offsetX, offsetY), new Rectangle(frameX + offsetX, frameY + offsetY, width, height), (overrideColor != null ? overrideColor(mixedColor) : mixedColor), 0f, default, 1f, effects, 0f);
					}
				}else
				if (color.R > gfxCheck2 || color.G > gfxCheck2 * 1.1 || color.B > gfxCheck2 * 1.2)
				{
					Color[] lightArray = new Color[4];
					Lighting.GetColor4Slice(x, y, ref lightArray);
					for (int m = 0; m < 4; m++)
					{
						int offsetX = 0;
						int offsetY = 0;
						Color mixedColor = color;
						Color lightColor = lightArray[m];
						if (m == 1){ offsetX = 8; }
						if (m == 2){ offsetY = 8; }
						if (m == 3){ offsetX = 8; offsetY = 8; }
						mixedColor.R = (byte)((color.R + lightColor.R) / 2);
						mixedColor.G = (byte)((color.G + lightColor.G) / 2);
						mixedColor.B = (byte)((color.B + lightColor.B) / 2);
						sb.Draw(texture, drawPos + new Vector2(offsetX, offsetY), new Rectangle(frameX + offsetX, frameY + offsetY, 8, 8), (overrideColor != null ? overrideColor(mixedColor) : mixedColor), 0f, default, 1f, effects, 0f);
					}
				}else
				{
					sb.Draw(texture, drawPos, new Rectangle(frameX, frameY, fwidth, fheight), color, 0f, default, 1f, effects, 0f);
				}
			}else
			if (halfBrickOffset == 8 && (!Main.tile[x, y + 1].active() || !Main.tileSolid[Main.tile[x, y + 1].type] || Main.tile[x, y + 1].halfBrick()))
			{
				sb.Draw(texture, drawPos + new Vector2(0, halfBrickOffset), new Rectangle(frameX, frameY, fwidth, fheight - halfBrickOffset - 4), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
				sb.Draw(texture, drawPos + new Vector2(0, 12f), new Rectangle(144, 66, fwidth, 4), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
			}else
			{
				sb.Draw(texture, drawPos + new Vector2(0, halfBrickOffset), new Rectangle(frameX, frameY, fwidth, fheight), (overrideColor != null ? overrideColor(color) : color), 0f, default, 1f, effects, 0f);
			}
		}


		public static void DrawWallTexture(SpriteBatch sb, Texture2D texture, int x, int y, bool drawOutline = false, Func<Color, Color> overrideColor = null, Vector2 offset = default)
		{
			Tile tile = Main.tile[x, y];
			bool hasWall = tile != null && tile.wall > 0;
			int wallFrameX = (hasWall ? tile.wallFrameX() : 0);
			int wallFrameY = (hasWall ? tile.wallFrameY() : 0);
			int frameOffsetY = (hasWall ? Main.wallFrame[tile.wall] * 180 : 0);
			DrawWallTexture(sb, texture, x, y, wallFrameX, wallFrameY, frameOffsetY, drawOutline, overrideColor, offset);
		}

		/*
		 * Draws the given texture using a wall and the lighting nearby, or the overrideColor.
		 * 
		 * x/y : the x/y cooridinates of the wall.
		 * drawOutline : If true, draws the merging outline when two wall types meet.
		 * overrideColor : An override color used to override the color drawn with.
		 */
		public static void DrawWallTexture(SpriteBatch sb, Texture2D texture, int x, int y, int wallFrameX, int wallFrameY, int frameOffsetY, bool drawOutline = false, Func<Color, Color> overrideColor = null, Vector2 offset = default)
		{
			int gfxCheck = (int)(255f * (1f - Main.gfxQuality) + 100f * Main.gfxQuality);
			int gfxCheck2 = (int)(120f * (1f - Main.gfxQuality) + 40f * Main.gfxQuality);
			Vector2 drawOffset = (Main.drawToScreen ? default : new Vector2(Main.offScreenRange, Main.offScreenRange)) + offset;
			int tileColor = (Main.tileColor.R + Main.tileColor.G + Main.tileColor.B) / 3;
			float tileColorFloat = (float)(tileColor * 0.53) / 255f;
			if (Lighting.lightMode == 2){ tileColorFloat = (Main.tileColor.R - 12) / 255f; }else
			if (Lighting.lightMode == 3){ tileColorFloat = (tileColor - 12) / 255f; }
			Color color = (overrideColor != null ? overrideColor(default) : Lighting.GetColor(x, y)); 
			if (Lighting.lightMode < 2)
			{
				if (color.R > gfxCheck || color.G > gfxCheck * 1.1 || color.B > gfxCheck * 1.2)
				{
					Color[] lightArray = new Color[9];
					Lighting.GetColor9Slice(x, y, ref lightArray);
					for (int m = 0; m < 9; m++)
					{
						int offsetX = 0;
						int offsetY = 0;
						int width = 12;
						int height = 12;
						Color color2 = color;
						Color color3 = lightArray[m];
						if (m == 1){ width = 8; offsetX = 12; }
						if (m == 2){ offsetX = 20; }
						if (m == 3){ height = 8; offsetY = 12; }
						if (m == 4){ width = 8; height = 8; offsetX = 12; offsetY = 12; }
						if (m == 5){ offsetX = 20; offsetY = 12; height = 8; }
						if (m == 6){ offsetY = 20; }
						if (m == 7){ width = 12; offsetX = 12; offsetY = 20; }
						if (m == 8){ offsetX = 20; offsetY = 20; }
						color2.R = (byte)((color.R + color3.R) / 2);
						color2.G = (byte)((color.G + color3.G) / 2);
						color2.B = (byte)((color.B + color3.B) / 2);
						sb.Draw(texture, new Vector2(x * 16 - (int)Main.screenPosition.X - 8 + offsetX, y * 16 - (int)Main.screenPosition.Y - 8 + offsetY) + drawOffset, new Rectangle(wallFrameX + offsetX, wallFrameY + offsetY + frameOffsetY, width, height), (overrideColor != null ? overrideColor(color2) : color2), 0f, default, 1f, SpriteEffects.None, 0f);
					}
				}else
				if (color.R > gfxCheck2 || color.G > gfxCheck2 * 1.1 || color.B > gfxCheck2 * 1.2)
				{
					Color[] lightArray = new Color[4];
					Lighting.GetColor4Slice(x, y, ref lightArray);
					for (int n = 0; n < 4; n++)
					{
						int offsetX = 0;
						int offsetY = 0;
						Color color4 = color;
						Color color5 = lightArray[n];
						if (n == 1){ offsetX = 16; }
						if (n == 2){ offsetY = 16; }
						if (n == 3){ offsetX = 16; offsetY = 16; }
						color4.R = (byte)((color.R + color5.R) / 2);
						color4.G = (byte)((color.G + color5.G) / 2);
						color4.B = (byte)((color.B + color5.B) / 2);
						sb.Draw(texture, new Vector2(x * 16 - (int)Main.screenPosition.X - 8 + offsetX, y * 16 - (int)Main.screenPosition.Y - 8 + offsetY) + drawOffset, new Rectangle(wallFrameX + offsetX, wallFrameY + offsetY + frameOffsetY, 16, 16), (overrideColor != null ? overrideColor(color4) : color4), 0f, default, 1f, SpriteEffects.None, 0f);
					}
				}else
				{
					Rectangle rect = new Rectangle(wallFrameX, wallFrameY + frameOffsetY, 32, 32);
					sb.Draw(texture, new Vector2(x * 16 - (int)Main.screenPosition.X - 8, y * 16 - (int)Main.screenPosition.Y - 8) + drawOffset, rect, color, 0f, default, 1f, SpriteEffects.None, 0f);
				}
			}
			if (drawOutline && (color.R > gfxCheck2 * 0.4 || color.G > gfxCheck2 * 0.35 || color.B > gfxCheck2 * 0.3))
			{
				bool outlineLeft = Main.tile[x - 1, y].wall > 0 && Main.wallBlend[Main.tile[x - 1, y].wall] != Main.wallBlend[Main.tile[x, y].wall];
				bool outlineRight = Main.tile[x + 1, y].wall > 0 && Main.wallBlend[Main.tile[x + 1, y].wall] != Main.wallBlend[Main.tile[x, y].wall];
				bool outlineUp = Main.tile[x, y - 1].wall > 0 && Main.wallBlend[Main.tile[x, y - 1].wall] != Main.wallBlend[Main.tile[x, y].wall];
				bool outlineDown = Main.tile[x, y + 1].wall > 0 && Main.wallBlend[Main.tile[x, y + 1].wall] != Main.wallBlend[Main.tile[x, y].wall];
				if (outlineLeft) sb.Draw(Main.wallOutlineTexture, new Vector2(x * 16 - (int)Main.screenPosition.X, y * 16 - (int)Main.screenPosition.Y) + drawOffset, new Rectangle(0, 0, 2, 16), color, 0f, default, 1f, SpriteEffects.None, 0f);
				if (outlineRight) sb.Draw(Main.wallOutlineTexture, new Vector2(x * 16 - (int)Main.screenPosition.X + 14, y * 16 - (int)Main.screenPosition.Y) + drawOffset, new Rectangle(14, 0, 2, 16), color, 0f, default, 1f, SpriteEffects.None, 0f);
				if (outlineUp) sb.Draw(Main.wallOutlineTexture, new Vector2(x * 16 - (int)Main.screenPosition.X, y * 16 - (int)Main.screenPosition.Y) + drawOffset, new Rectangle(0, 0, 16, 2), color, 0f, default, 1f, SpriteEffects.None, 0f);
				if (outlineDown) sb.Draw(Main.wallOutlineTexture, new Vector2(x * 16 - (int)Main.screenPosition.X, y * 16 - (int)Main.screenPosition.Y + 14) + drawOffset, new Rectangle(0, 14, 16, 2), color, 0f, default, 1f, SpriteEffects.None, 0f);
			}
		}

		/*
		 * Returns the draw position of a texture for tiles.
		 */
		public static Vector2 GetTileDrawPosition(int x, int y, int width, int height, Vector2 drawOffset)
		{
			return new Vector2((x * 16 - (int)Main.screenPosition.X) - (width - 16f) / 2f, y * 16 - (int)Main.screenPosition.Y) + drawOffset;
		}

        /*
         * Returns the draw position of a texture for npcs and projectiles.
         */
        public static Vector2 GetDrawPosition(Vector2 position, Vector2 origin, int width, int height, int texWidth, int texHeight, Rectangle frame, int framecount, float scale, bool drawCentered = false)
        {
			return GetDrawPosition(position, origin, width, height, texWidth, texHeight, frame, framecount, 1, scale, drawCentered);
		}
	
        /*
         * Returns the draw position of a texture for npcs and projectiles.
         */
        public static Vector2 GetDrawPosition(Vector2 position, Vector2 origin, int width, int height, int texWidth, int texHeight, Rectangle frame, int framecount, int framecountX, float scale, bool drawCentered = false)
        {
			Vector2 screenPos = new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
			if(drawCentered)
			{
				Vector2 texHalf = new Vector2(texWidth / framecountX / 2, texHeight / framecount / 2);
				return position + new Vector2(width / 2, height / 2) - (texHalf * scale) + (origin * scale) - screenPos;
			}
			return position - screenPos + new Vector2(width / 2, height) - (new Vector2(texWidth / framecountX / 2, texHeight / framecount) * scale) + (origin * scale) + new Vector2(0f, 5f);
        }

        /*
         * Draws the given texture using a player and lighting nearby, or the overriden color given.
         * 
         * locationType : 0 == head, 1 == body, 2 == legs.
         */
        public static void DrawPlayerTexture(object sb, Texture2D texture, int shader, Player drawPlayer, Vector2 ediPos, int locationType, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null, Rectangle? frameRect = null, float scaleOverride = 0f)
        {
            Vector2 locationPos = (locationType == 0 ? drawPlayer.headPosition : locationType == 1 ? drawPlayer.bodyPosition : drawPlayer.legPosition);
            float locationRot = (locationType == 0 ? drawPlayer.headRotation : locationType == 1 ? drawPlayer.bodyRotation : drawPlayer.legRotation);
            Rectangle locationFrame = (locationType == 0 ? drawPlayer.headFrame : locationType == 1 ? drawPlayer.bodyFrame : drawPlayer.legFrame);
            DrawPlayerTexture(sb, texture, shader, drawPlayer, ediPos, locationPos, locationRot, locationFrame, offsetX, offsetY, overrideColor, frameRect, scaleOverride);
        }

        /*
         * Draws the given texture using a player and lighting nearby, or the overriden color given.
         * It draws using the player's current frame and thus expects a texture with the same number of frames.
         * 
         * locationPos : the location position. (in Player: headPosition, bodyPosition, legPosition)
         * locationRot : the location rotation. (in Player: headRotation, bodyRotation, legRotation)
         * locationFrame : The location frame. (in Player: headFrame, bodyFrame, legFrame)
         * offsetX, offsetY: offset the drawing on the X and Y axis respectively.
         * frameRect : the bounding box of the drawing area. (usually drawPlayer.bodyFrame)
         */
        public static void DrawPlayerTexture(object sb, Texture2D texture, int shader, Player drawPlayer, Vector2 ediPos, Vector2 locationPos, float locationRot, Rectangle locationFrame, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null, Rectangle? frameRect = null, float scaleOverride = 0f)
        {
            offsetX = (drawPlayer.direction == -1 ? -offsetX : offsetX);
            offsetY += 4f; //adding y offset to put properly into place.

            Vector2 frameCenter = new Vector2(locationFrame.Width * 0.5f, locationFrame.Height * 0.5f);
            Color color = (overrideColor != null ? (Color)overrideColor : GetPlayerColor(drawPlayer, drawPlayer.Center));
            Rectangle frame = frameRect != null ? (Rectangle)frameRect : drawPlayer.bodyFrame;

            SpriteEffects effect = drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if(drawPlayer.gravDir == -1.0f) { effect |= SpriteEffects.FlipVertically; }

			float scale = scaleOverride > 0f ? scaleOverride : 1f;
			if (sb is List<DrawData>)
			{
				DrawData dd = new DrawData(texture, new Vector2((int)(ediPos.X - (int)Main.screenPosition.X - frame.Width / 2 + drawPlayer.width / 2), (int)(ediPos.Y - (int)Main.screenPosition.Y + drawPlayer.height - frame.Height)) + new Vector2(offsetX * scale, offsetY * scale) + locationPos + frameCenter, frame, color, locationRot, frameCenter, scale, effect, 0);
				dd.shader = shader;
				((List<DrawData>)sb).Add(dd);
			}
			else if (sb is SpriteBatch)
			{
				bool applyDye = shader > 0;
				if (applyDye)
				{
					((SpriteBatch)sb).End();
					((SpriteBatch)sb).Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
					GameShaders.Armor.ApplySecondary(shader, drawPlayer, null);				
				}
				((SpriteBatch)sb).Draw(texture, new Vector2((int)(ediPos.X - (int)Main.screenPosition.X - frame.Width / 2 + drawPlayer.width / 2), (int)(ediPos.Y - (int)Main.screenPosition.Y + drawPlayer.height - frame.Height)) + new Vector2(offsetX * scale, offsetY * scale) + locationPos + frameCenter, frame, color, locationRot, frameCenter, scale, effect, 0);
				if (applyDye)
				{
					((SpriteBatch)sb).End();
					((SpriteBatch)sb).Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				}				
			}
        }

        public static float GetYOffset(Player player)
        {
            return GetYOffset(player.bodyFrame, player.gravDir);
        }

        /*
         * Returns an offset for Y that simulates how player frames offset normally. 
         * This allows you to have a one-frame .png file that still 'bobs' up and down even if it doesn't animate.
         */
        public static float GetYOffset(Rectangle frame, float gravDir = 0f)
        {
			int frameID = frame.Y / frame.Height;
            if(frameID == 7 || frameID == 8 || frameID == 9 || frameID == 14 || frameID == 15 || frameID == 16)
            {
                return gravDir < 0f ? 2f : -2f;
            }
            return 0f;
        }

		//used by InDrawZone to prevent making a new rectangle every time the method is called
		private static Rectangle drawZoneRect = default(Rectangle);

		public static bool InDrawZone(Vector2 vec, bool noScreenPos = false)
		{
			if ((int)Main.screenPosition.X - 300 != drawZoneRect.X || (int)Main.screenPosition.Y - 300 != drawZoneRect.Y) drawZoneRect = new Rectangle((int)Main.screenPosition.X - 300, (int)Main.screenPosition.Y - 300, Main.screenWidth + 600, Main.screenHeight + 600);	
			if(noScreenPos) vec += Main.screenPosition;
			return drawZoneRect.Contains((int)vec.X, (int)vec.Y);
		}

		public static bool InDrawZone(Rectangle rect)
		{
			if ((int)Main.screenPosition.X - 300 != drawZoneRect.X || (int)Main.screenPosition.Y - 300 != drawZoneRect.Y) drawZoneRect = new Rectangle((int)Main.screenPosition.X - 300, (int)Main.screenPosition.Y - 300, Main.screenWidth + 600, Main.screenHeight + 600);
			return drawZoneRect.Intersects(rect);
		}
    }
    public class AmmoSlotRender
    {
        //------------------------------------------------------//
        //----------------AMMO SLOT RENDERER--------------------//
        //------------------------------------------------------//
        // A basic class that renders an ammo-like string over  //
        // an item when registed.                               //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        public int itemType = -1;
        public int[] ammoItemTypes = new int[0];
        public int[] ammoTypes = new int[0];

		public AmmoSlotRender() //dummy constructor
		{
		}

        /*
         * typeisammo : If true, ammoitemtype is considered an ammo type. If false, it is considered an item type.
         */
        public AmmoSlotRender(int itemtype, int ammoitemtype, bool typeisammo = false):this(itemtype, typeisammo ? default(int[]) : new int[]{ ammoitemtype }, typeisammo ? new int[]{ ammoitemtype } : default(int[]))
        {
        }

        public AmmoSlotRender(int itemtype, int[] ammoitemtypes, int[] ammotypes = default(int[]))
        {
            itemType = itemtype;
            ammoItemTypes = ammoitemtypes;
            ammoTypes = ammotypes;
        }

		public virtual void Draw(SpriteBatch sb, Color color, Item item, Vector2 pos, float sc) 
        {
            if(Main.playerInventory || item.type <= ItemID.None || item.stack <= 0 || item.type != itemType) return;
            int totalItemCount = 0;
            if(ammoItemTypes != default(int[])){ totalItemCount += BasePlayer.GetItemstackSum(Main.player[Main.myPlayer], ammoItemTypes, false, true, true); }
            if(ammoTypes != default(int[])){ totalItemCount += BasePlayer.GetItemstackSum(Main.player[Main.myPlayer], ammoTypes, true, true, true); }
            string s = "" + totalItemCount;
            if (totalItemCount > 99999) { s = "A Lot!"; }
            //sb.DrawString(Main.fontItemStack, s, pos + new Vector2(10f * sc, 32f * sc), color, 0f, default, sc *= 0.8f, SpriteEffects.None, 0f);   
			ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontItemStack, s, pos + new Vector2(10f * sc, 32f * sc), color, 0f, default, new Vector2(sc *= 0.8f), -1f, 0.8f);			
        }
    }

	public class BaseArmorData : ArmorShaderData
	{
		public static Entity lastShaderDrawObject = null;		
		public static bool secondaryApply = false;
		int _uState = 0;
		public Texture2D _uExtraTex = null;
		
		public BaseArmorData(Ref<Effect> shader, string passName) : base(shader, passName)
		{
		}
		
		public BaseArmorData SetState(int state)
		{
			_uState = state;
			return this;
		}

		
		
		public override ArmorShaderData GetSecondaryShader(Entity entity)
		{
			secondaryApply = true;
			return base.GetSecondaryShader(entity);
		}//		
	}
}
