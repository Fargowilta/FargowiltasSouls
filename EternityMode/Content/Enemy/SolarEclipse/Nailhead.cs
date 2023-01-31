using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.SolarEclipse
{
    public class Nailhead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Nailhead);

        public int Counter;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(Counter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            Counter = binaryReader.Read7BitEncodedInt();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            const int timeToBeginAttack = 420;

            if (++Counter == timeToBeginAttack)
            {
                FargoSoulsUtil.DustRing(npc.Center, 48, DustID.YellowTorch, 9f, default, 3f);
                npc.netUpdate = true;
                NetSync(npc);
            }

            if (Counter > timeToBeginAttack)
            {
                npc.position -= npc.velocity;

                if (Counter > timeToBeginAttack + 45 && Counter % 4 == 0)
                    Spray();
            }

            if (Counter > timeToBeginAttack + 45 + 90)
            {
                Counter = 0;
                npc.netUpdate = true;
                NetSync(npc);
            }

            void Spray()
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    //npc entire block is fucked
                    int length = Main.rand.Next(3, 6);
                    int[] numArray = new int[length];
                    int maxValue = 0;
                    for (int index = 0; index < (int)byte.MaxValue; ++index)
                    {
                        if (Main.player[index].active && !Main.player[index].dead && Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[index].position, Main.player[index].width, Main.player[index].height))
                        {
                            numArray[maxValue] = index;
                            ++maxValue;
                            if (maxValue == length)
                                break;
                        }
                    }
                    if (maxValue > 1)
                    {
                        for (int index1 = 0; index1 < 100; ++index1)
                        {
                            int index2 = Main.rand.Next(maxValue);
                            int index3 = index2;
                            while (index3 == index2)
                                index3 = Main.rand.Next(maxValue);
                            int num1 = numArray[index2];
                            numArray[index2] = numArray[index3];
                            numArray[index3] = num1;
                        }
                    }

                    Vector2 vector2_1 = new Vector2(-1f, -1f);

                    for (int index = 0; index < maxValue; ++index)
                    {
                        Vector2 vector2_2 = Main.npc[numArray[index]].Center - npc.Center;
                        vector2_2.Normalize();
                        vector2_1 += vector2_2;
                    }

                    vector2_1.Normalize();

                    for (int index = 0; index < length; ++index)
                    {
                        float num1 = Main.rand.Next(8, 13);
                        Vector2 vector2_2 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                        vector2_2.Normalize();

                        if (maxValue > 0)
                        {
                            vector2_2 += vector2_1;
                            vector2_2.Normalize();
                        }
                        vector2_2 *= num1;

                        if (maxValue > 0)
                        {
                            --maxValue;
                            vector2_2 = Main.player[numArray[maxValue]].Center - npc.Center;
                            vector2_2.Normalize();
                            vector2_2 *= num1;
                        }

                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.position.Y + npc.width / 4f, vector2_2.X, vector2_2.Y, ProjectileID.Nail, (int)(npc.damage * 0.15), 1f);
                    }
                }
            }
        }
    }
}
