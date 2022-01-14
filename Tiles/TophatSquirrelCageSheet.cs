using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasSouls.Items.Tiles
{
    public class TophatSquirrelCageSheet : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;

            // The larger cage uses Style6x3.
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.addTile(Type);

            animationFrameHeight = 54;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Top Hat Squirrel Cage");
            AddMapEntry(new Color(122, 217, 232), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, ItemType<TophatSquirrelCage>());
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frame == 0)
            {
                frameCounter++;
                if (frameCounter > Main.rand.Next(30, 900))
                {
                    if (Main.rand.Next(3) != 0)
                    {
                        int num = Main.rand.Next(7);
                        if (num == 0)
                        {
                            frame = 4;
                        }
                        else if (num <= 2)
                        {
                            frame = 2;
                        }
                        else
                        {
                            frame = 1;
                        }
                    }
                    frameCounter = 0;
                }
            }
            else if (frame == 1)
            {
                frameCounter++;
                if (frameCounter >= 10)
                {
                    frameCounter = 0;
                    frame = 0;
                }
            }
            else if (frame >= 2 && frame <= 3)
            {
                frameCounter++;
                if (frameCounter >= 5)
                {
                    frameCounter = 0;
                    frame++;
                }
                if (frame > 3)
                {
                    if (Main.rand.NextBool(5))
                    {
                        frame = 0;
                    }
                    else
                    {
                        frame = 2;
                    }
                }
            }
            else if (frame >= 4 && frame <= 8)
            {
                frameCounter++;
                if (frameCounter >= 5)
                {
                    frameCounter = 0;
                    frame++;
                }
            }
            else if (frame == 9)
            {
                frameCounter++;
                if (frameCounter > Main.rand.Next(30, 900))
                {
                    if (Main.rand.Next(3) != 0)
                    {
                        int num = Main.rand.Next(7);
                        if (num == 0)
                        {
                            frame = 13;
                        }
                        else if (num <= 2)
                        {
                            frame = 11;
                        }
                        else
                        {
                            frame = 10;
                        }
                    }
                    frameCounter = 0;
                }
            }
            else if (frame == 10)
            {
                frameCounter++;
                if (frameCounter >= 10)
                {
                    frameCounter = 0;
                    frame = 9;
                }
            }
            else if (frame == 11 || frame == 12)
            {
                frameCounter++;
                if (frameCounter >= 5)
                {
                    frame++;
                    if (frame > 12)
                    {
                        if (Main.rand.Next(5) != 0)
                        {
                            frame = 11;
                        }
                        else
                        {
                            frame = 9;
                        }
                    }
                    frameCounter = 0;
                }
            }
            else if (frame >= 13)
            {
                frameCounter++;
                if (frameCounter >= 5)
                {
                    frameCounter = 0;
                    frame++;
                }
                if (frame > 17)
                {
                    frame = 0;
                }
            }
        }
    }
}