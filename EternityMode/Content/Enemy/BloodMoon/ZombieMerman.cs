using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.BloodMoon
{
    public class ZombieMerman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ZombieMerman);

        public int JumpTimer;
        public bool Jumped;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(JumpTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            JumpTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            for (int i = 0; i < 9; i++)
            {
                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.Zombie, velocity: Main.rand.NextVector2Circular(8, 8));
            }
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            const float gravity = 0.4f;

            if (JumpTimer > 120) //initiate jump
            {
                JumpTimer = 0;
                Jumped = true;

                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const float time = 60;
                    Vector2 distance;
                    if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                        distance = Main.player[t].Center - npc.Bottom;
                    else
                        distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    npc.ai[1] = time;
                    npc.ai[2] = distance.X;
                    npc.ai[3] = distance.Y;
                    npc.netUpdate = true;
                }

                return false;
            }

            //if (JumpTimer == 150 && Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IronParry>(), 0, 0f, Main.myPlayer);

            if (npc.ai[1] > 0f) //while jumping
            {
                npc.ai[1]--;
                npc.noTileCollide = true;
                npc.velocity.X = npc.ai[2];
                npc.velocity.Y = npc.ai[3];
                npc.ai[3] += gravity;

                int num22 = 2;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RedTorch, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= npc.velocity;
                }

                JumpTimer = 0;
                JumpTimer++;
                return false;
            }
            else
            {
                if (npc.noTileCollide)
                {
                    npc.direction = System.Math.Sign(npc.velocity.X);
                    JumpTimer = 0;
                    npc.noTileCollide = Collision.SolidCollision(npc.position, npc.width, npc.height);
                    return false;
                }
            }

            if (npc.HasValidTarget && npc.life < npc.lifeMax / 2 && npc.velocity.Y == 0)
            {
                JumpTimer++;
            }

            if (npc.velocity.Y == 0f && Jumped)
            {
                Jumped = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int j = -1; j <= 1; j += 2)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            Vector2 vel = 16f * j * Vector2.UnitX.RotatedBy(MathHelper.PiOver4 / 3 * i * -j);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.SharpTears, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 0f, Main.rand.NextFloat(0.5f, 1f));
                        }
                    }
                }
            }

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Anticoagulation>(), 600);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FrogLeg, 10));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.BalloonPufferfish, 10));
        }
    }
}
