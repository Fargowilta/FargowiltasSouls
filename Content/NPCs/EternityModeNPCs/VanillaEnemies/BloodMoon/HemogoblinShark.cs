using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
    public class HemogoblinShark : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GoblinShark);

        public int AttackTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer < 360)
            {
                if (npc.HasValidTarget && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    AttackTimer += 9; //faster when no line of sight
            }
            else if (AttackTimer == 360 + 10)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 180);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 200);
                }
            }
            else if (AttackTimer >= 360 + 10 + 45)
            {
                AttackTimer = 0;

                if (FargoSoulsUtil.HostCheck && npc.HasValidTarget)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 target = Main.player[npc.target].Center + Main.rand.NextVector2Circular(8, 8);
                        Vector2 spawnPos = FindSharpTearsSpot(Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) ? npc.Center : Main.player[npc.target].Center, target).ToWorldCoordinates(Main.rand.Next(17), Main.rand.Next(17));
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, 16f * Vector2.Normalize(target - spawnPos), ProjectileID.SharpTears, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer, 0f, Main.rand.NextFloat(0.5f, 1f));
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
        }

        private static Point FindSharpTearsSpot(Vector2 origin, Vector2 targetSpot)
        {
            targetSpot.ToTileCoordinates();
            Vector2 center = origin;
            Vector2 endPoint = targetSpot;
            int samplesToTake = 3;
            float samplingWidth = 4f;
            Collision.AimingLaserScan(center, endPoint, samplingWidth, samplesToTake, out Vector2 vectorTowardsTarget, out float[] samples);
            float num = float.PositiveInfinity;
            for (int index = 0; index < samples.Length; ++index)
            {
                if ((double)samples[index] < (double)num)
                    num = samples[index];
            }
            targetSpot = center + vectorTowardsTarget.SafeNormalize(Vector2.Zero) * num;
            Point tileCoordinates = targetSpot.ToTileCoordinates();
            Microsoft.Xna.Framework.Rectangle rectangle1 = new(tileCoordinates.X, tileCoordinates.Y, 1, 1);
            rectangle1.Inflate(6, 16);
            Microsoft.Xna.Framework.Rectangle rectangle2 = new(0, 0, Main.maxTilesX, Main.maxTilesY);
            rectangle2.Inflate(-40, -40);
            rectangle1 = Rectangle.Intersect(rectangle1, rectangle2);
            List<Point> pointList1 = [];
            List<Point> pointList2 = [];
            for (int left = rectangle1.Left; left <= rectangle1.Right; ++left)
            {
                for (int top = rectangle1.Top; top <= rectangle1.Bottom; ++top)
                {
                    if (WorldGen.SolidTile(left, top))
                    {
                        Vector2 vector2 = new((float)(left * 16 + 8), (float)(top * 16 + 8));
                        if ((double)Vector2.Distance(targetSpot, vector2) <= 200.0)
                        {
                            if (FindSharpTearsOpening(left, top, left > tileCoordinates.X, left < tileCoordinates.X, top > tileCoordinates.Y, top < tileCoordinates.Y))
                                pointList1.Add(new Point(left, top));
                            else
                                pointList2.Add(new Point(left, top));
                        }
                    }
                }
            }
            if (pointList1.Count == 0 && pointList2.Count == 0)
                pointList1.Add((origin.ToTileCoordinates().ToVector2() + Main.rand.NextVector2Square(-2f, 2f)).ToPoint());
            List<Point> pointList3 = pointList1;
            if (pointList3.Count == 0)
                pointList3 = pointList2;
            int index1 = Main.rand.Next(pointList3.Count);
            return pointList3[index1];
        }

        private static bool FindSharpTearsOpening(int x, int y, bool acceptLeft, bool acceptRight, bool acceptUp, bool acceptDown)
        {
            return acceptLeft && !WorldGen.SolidTile(x - 1, y) || acceptRight && !WorldGen.SolidTile(x + 1, y) || acceptUp && !WorldGen.SolidTile(x, y - 1) || acceptDown && !WorldGen.SolidTile(x, y + 1);
        }
    }
}
