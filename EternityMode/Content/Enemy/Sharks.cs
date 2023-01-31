using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class Sharks : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Shark,
            NPCID.SandShark,
            NPCID.SandsharkCorrupt,
            NPCID.SandsharkCrimson,
            NPCID.SandsharkHallow
        );

        public int JumpTimer;
        public int BleedCheckTimer;
        public int BleedCounter;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(JumpTimer);
            binaryWriter.Write7BitEncodedInt(BleedCheckTimer);
            binaryWriter.Write7BitEncodedInt(BleedCounter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            JumpTimer = binaryReader.Read7BitEncodedInt();
            BleedCheckTimer = binaryReader.Read7BitEncodedInt();
            BleedCounter = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            JumpTimer = Main.rand.Next(60);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (npc.type == NPCID.Shark && Main.rand.NextBool(3))
                EModeGlobalNPC.Horde(npc, Main.rand.Next(1, 5));
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.Shark)
            {
                if (npc.life < npc.lifeMax / 2 && --JumpTimer < 0) //initiate jump
                {
                    JumpTimer = 360;
                    int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                    if (t != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        const float gravity = 0.3f;
                        const float time = 90;
                        Vector2 distance;
                        if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                            distance = Main.player[t].Center - npc.Center;
                        else
                            distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        npc.ai[1] = time;
                        npc.ai[2] = distance.X;
                        npc.ai[3] = distance.Y;
                        npc.netUpdate = true;
                    }
                }
                if (npc.ai[1] > 0f) //while jumping
                {
                    npc.ai[1]--;
                    npc.noTileCollide = true;
                    npc.velocity.X = npc.ai[2];
                    npc.velocity.Y = npc.ai[3];
                    npc.ai[3] += 0.3f;

                    int num22 = 5;
                    for (int index1 = 0; index1 < num22; ++index1)
                    {
                        Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                        int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                        Main.dust[index2].velocity /= 4f;
                        Main.dust[index2].velocity -= npc.velocity;
                    }
                }
                else
                {
                    if (npc.noTileCollide) //compensate for long body
                        npc.noTileCollide = Collision.SolidCollision(npc.position + Vector2.UnitX * npc.width / 4, npc.width / 2, npc.height);
                }
            }

            if (++BleedCheckTimer >= 240)
            {
                BleedCheckTimer = 0;

                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1 && BleedCounter < 5)
                {
                    Player player = Main.player[t];
                    if (player.bleed && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        BleedCounter++;
                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }
            }

            if (BleedCounter > 0)
                npc.damage = (int)(npc.defDamage * (1f + BleedCounter / 2f));
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Bleeding, 240);

            target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 600);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (npc.type == NPCID.Shark)
            {
                if (Main.hardMode && Main.rand.NextBool(4) && Collision.CanHitLine(npc.Top, 0, 0, npc.Top - 30 * 16f * Vector2.UnitY, 0, 0))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.Sharknado, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 0.5f), 0f, Main.myPlayer, 15, 15);
                }

                if (!Main.dedServ && Main.rand.NextBool(1000))
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/a"), npc.Center);

                    CombatText.NewText(npc.Hitbox, Color.Blue, "a", true);

                    for (int i = 0; i < 100; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.Next(new int[]
                        {
                            DustID.Confetti,
                            DustID.Confetti_Blue,
                            DustID.Confetti_Green,
                            DustID.Confetti_Pink,
                            DustID.Confetti_Yellow
                        }), 0f, 0f, 0, default(Color), 2.5f);
                        Main.dust[d].noGravity = Main.rand.NextBool(3);
                        Main.dust[d].velocity *= 6f;
                    }
                }
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (BleedCounter > 0)
            {
                drawColor.R = (byte)(BleedCounter * 20 + 155);
                drawColor.G /= (byte)(BleedCounter + 1);
                drawColor.B /= (byte)(BleedCounter + 1);
                return drawColor;
            }

            return base.GetAlpha(npc, drawColor);
        }
    }

    public class SharkTransformables : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Goldfish,
            NPCID.GoldfishWalker,
            NPCID.BlueJellyfish,
            NPCID.GreenJellyfish,
            NPCID.PinkJellyfish
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(6)) //random sharks
            {
                npc.position = npc.Bottom;
                npc.Transform(NPCID.Shark);
                npc.Bottom = npc.position;
            }
        }
    }
}
