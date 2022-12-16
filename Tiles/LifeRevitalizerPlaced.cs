using FargowiltasSouls.Items.Placeables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Tiles
{
    public class LifeRevitalizerPlaced : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.IsValidSpawnPoint[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Life Revitalizer");
            AddMapEntry(Color.Pink, name);

            AnimationFrameHeight = 54;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            Tile tile = Framing.GetTileSafely(i, j);
            //account for possibly clicking on any part of the multi-tile, negate it to have coords of top left corner
            i -= tile.TileFrameX / 18;
            j -= tile.TileFrameY / 18;
            //add offset to get the coord right below the middle-bottom of this multi-tile
            i += 1;
            j += 3;

            //Main.NewText($"{i} {j}");

            player.FindSpawn();
            if (player.SpawnX == i && player.SpawnY == j)
            {
                player.RemoveSpawn();
                Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
            }
            else if (WorldGen.InWorld(i, j))
            {
                player.ChangeSpawn(i, j);
                Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<LifeRevitalizer>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<LifeRevitalizer>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 20 / 255f;
            b = 147 / 255f;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 6)
            {
                frameCounter = 0;
                frame = ++frame % 3;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //drawcode method for star
            /*if (Main.tile[i - 1, j - 1].TileType == ModContent.TileType<LifeRevitalizerPlaced>() && Main.tile[i + 1, j - 1].TileType == ModContent.TileType<LifeRevitalizerPlaced>())
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                Texture2D star = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle rect = new Rectangle(0, 0, star.Width, star.Height);
                float scale = 0.175f * Main.rand.NextFloat(1f, 1.5f);
                Vector2 origin = new Vector2((star.Width / 2) + scale, (star.Height / 2) + scale);
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X + 6, j * 16 - (int)Main.screenPosition.Y - 8) + zero;
                spriteBatch.Draw(star, pos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                DrawData starDraw = new DrawData(star, pos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.HotPink).UseSecondaryColor(Color.HotPink);
                GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
                starDraw.Draw(spriteBatch);
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }*/
            return true; 
        }

    }
}