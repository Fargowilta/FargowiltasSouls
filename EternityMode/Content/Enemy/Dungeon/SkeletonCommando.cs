using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Dungeon
{
    public class SkeletonCommando : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletonCommando);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //num3 = 90, num5 = 4f, damage = 48/60, ID.RocketSkeleton
            if (npc.ai[2] > 0f && npc.ai[1] <= 50f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();

                    int damage = Main.expertMode ? 48 : 60;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 4f * speed, ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(10f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(-10f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                }
                SoundEngine.PlaySound(SoundID.Item11, npc.Center);
                npc.ai[2] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
        }
    }
}
