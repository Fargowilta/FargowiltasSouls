using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.FrostMoon
{
    public class ElfArcher : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ElfArcher);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //num3 = 110, damage = 36/45, tsunami
            if (npc.ai[2] > 0f && npc.ai[1] <= 60f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Y -= Math.Abs(speed.X) * 0.1f; //account for gravity
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();
                    Vector2 spinningpoint = speed;
                    speed *= 7f;

                    int damage = Main.expertMode ? 36 : 45;

                    //tsunami code lol
                    float num3 = 0.3141593f;
                    int num4 = 5;
                    spinningpoint *= 40f;
                    bool flag4 = Collision.CanHit(npc.Center, 0, 0, npc.Center + spinningpoint, 0, 0);
                    for (int index1 = 0; index1 < num4; ++index1)
                    {
                        float num8 = index1 - (num4 - 1f) / 2f;
                        Vector2 vector2_5 = spinningpoint.RotatedBy(num3 * num8);
                        if (!flag4)
                            vector2_5 -= spinningpoint;
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + vector2_5, speed, ModContent.ProjectileType<ElfArcherArrow>(), damage, 0f, Main.myPlayer);
                        Main.projectile[p].noDropItem = true;
                    }
                }
                SoundEngine.PlaySound(SoundID.Item5, npc.Center);
                npc.ai[2] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
        }
    }
}
