using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class SkeletonArcher : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletonArcher);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //damage = 28/35, ID.VenomArrow
            if (npc.ai[2] > 0f && npc.ai[1] <= 40f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Y -= Math.Abs(speed.X) * 0.075f; //account for gravity (default *0.1f)
                    speed.X += Main.rand.Next(-24, 25);
                    speed.Y += Main.rand.Next(-24, 25);
                    speed.Normalize();
                    speed *= 11f;

                    int damage = Main.expertMode ? 28 : 35;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<SkeletonArcherArrow>(), damage, 0f, Main.myPlayer);
                }
                SoundEngine.PlaySound(SoundID.Item5, npc.Center);
                npc.ai[2] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
        }
    }
}
