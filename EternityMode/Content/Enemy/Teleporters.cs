using System.IO;
using Terraria.ModLoader.IO;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public abstract class Teleporters : EModeNPCBehaviour
    {
        public int TeleportThreshold = 180;
        public int TeleportTimer;
        public bool DoTeleport;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(TeleportTimer);
            bitWriter.WriteBit(DoTeleport);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            TeleportTimer = binaryReader.Read7BitEncodedInt();
            DoTeleport = bitReader.ReadBit();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            int teleportThreshold = npc.type == NPCID.Tim || npc.type == NPCID.RuneWizard ? 90 : 180;

            if (DoTeleport)
            {
                if (++TeleportTimer > teleportThreshold)
                {
                    TeleportTimer = 0;
                    DoTeleport = false;

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        npc.ai[0] = 1f;
                        int num1 = (int)Main.player[npc.target].position.X / 16;
                        int num2 = (int)Main.player[npc.target].position.Y / 16;
                        int num3 = (int)npc.position.X / 16;
                        int num4 = (int)npc.position.Y / 16;
                        int num5 = 20;
                        int num6 = 0;
                        bool flag1 = false;
                        if ((double)Math.Abs(npc.position.X - Main.player[npc.target].position.X) + (double)Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000.0)
                        {
                            num6 = 100;
                            flag1 = true;
                        }
                        while (!flag1 && num6 < 100)
                        {
                            ++num6;
                            int index1 = Main.rand.Next(num1 - num5, num1 + num5);
                            for (int index2 = Main.rand.Next(num2 - num5, num2 + num5); index2 < num2 + num5; ++index2)
                            {
                                if ((index2 < num2 - 4 || index2 > num2 + 4 || (index1 < num1 - 4 || index1 > num1 + 4)) && (index2 < num4 - 1 || index2 > num4 + 1 || (index1 < num3 - 1 || index1 > num3 + 1)) && Main.tile[index1, index2].HasUnactuatedTile)
                                {
                                    bool flag2 = true;
                                    if (npc.HasValidTarget && Main.player[npc.target].ZoneDungeon && (npc.type == NPCID.DarkCaster || npc.type >= NPCID.RaggedCaster && npc.type <= NPCID.DiabolistWhite) && !Main.wallDungeon[(int)Main.tile[index1, index2 - 1].WallType])
                                        flag2 = false;
                                    if (Main.tile[index1, index2 - 1].LiquidType == LiquidID.Lava && Main.tile[index1, index2 - 1].LiquidAmount > 0)
                                        flag2 = false;
                                    if (flag2 && Main.tileSolid[(int)Main.tile[index1, index2].TileType] && !Collision.SolidTiles(index1 - 1, index1 + 1, index2 - 4, index2 - 1))
                                    {
                                        npc.ai[1] = 20f;
                                        npc.ai[2] = (float)index1;
                                        npc.ai[3] = (float)index2;
                                        flag1 = true;
                                        break;
                                    }
                                }
                            }
                        }

                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }
            }

            if (npc.ai[0] == 0 && npc.ai[1] == 20 && npc.ai[2] > 0 && npc.ai[3] > 0)
            {
                TeleportTimer = 0;
                DoTeleport = false;
            }
            else if (npc.justHit && !DoTeleport)
            {
                DoTeleport = true;
                npc.netUpdate = true;
                NetSync(npc);
            }
        }
    }
}
