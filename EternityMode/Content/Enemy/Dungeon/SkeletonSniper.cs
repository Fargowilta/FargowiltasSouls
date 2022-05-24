using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Dungeon
{
    public class SkeletonSniper : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletonSniper);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //num3 = 200, num8 = 0
            if (npc.ai[2] > 0f && npc.ai[1] <= 105f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.X += Main.rand.Next(-40, 41) * 0.2f;
                    speed.Y += Main.rand.Next(-40, 41) * 0.2f;
                    speed.Normalize();
                    speed *= 11f;

                    int damage = Main.expertMode ? 80 : 100;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<SniperBullet>(), damage, 0f, Main.myPlayer);
                }
                SoundEngine.PlaySound(SoundID.Item40, npc.Center);
                npc.ai[2] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
        }
    }
}
