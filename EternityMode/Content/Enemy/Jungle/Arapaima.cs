using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class Arapaima : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Arapaima);

        public int JumpTimer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            JumpTimer = Main.rand.Next(120);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++JumpTimer > 420) //initiate jump
            {
                JumpTimer = 0;

                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (npc.life < npc.lifeMax && t != -1 && Main.player[t].wet && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const float gravity = 0.3f;
                    const float time = 120f;
                    Vector2 distance;
                    if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                        distance = Main.player[t].Center - npc.Center;
                    else
                        distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    npc.ai[1] = 120f;
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
    }
}
