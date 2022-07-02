using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.NPCs.Critters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Tiles
{
    public class FMMBanner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Banner");
            AddMapEntry(new Color(13, 88, 130), name);

            //name.AddTranslation((int)GameCulture.CultureName.Chinese, "旗帜");
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int style = frameX / 18;
            int item = /*style == 0 ?*/ ModContent.ItemType<TophatSquirrelBanner>();// : ModContent.ItemType<FezSquirrelBanner>();
            Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, item);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer) return;

            Player player = Main.LocalPlayer;
            int style = Main.tile[i, j].TileFrameX / 18;
            int npcType = /*style = 0 ?*/ ModContent.NPCType<TophatSquirrelCritter>();// : ModContent.NPCType<FezSquirrel>();

            Main.SceneMetrics.NPCBannerBuff[npcType] = true;
            Main.SceneMetrics.hasBanner = true;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1) spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }
}