using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class Skeletons : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Skeleton,
            NPCID.HeadacheSkeleton,
            NPCID.MisassembledSkeleton,
            NPCID.PantlessSkeleton,
            NPCID.SkeletonTopHat,
            NPCID.SkeletonAstonaut,
            NPCID.SkeletonAlien,
            NPCID.BoneThrowingSkeleton,
            NPCID.BoneThrowingSkeleton2,
            NPCID.BoneThrowingSkeleton3,
            NPCID.BoneThrowingSkeleton4
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            switch (npc.type)
            {
                case NPCID.Skeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton); break;
                case NPCID.HeadacheSkeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton2); break;
                case NPCID.MisassembledSkeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton3); break;
                case NPCID.PantlessSkeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton4); break;
                default: break;
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.skeleBoss, NPCID.SkeletronHead))
            {
                npc.life = 0;
                npc.HitEffect();
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-100, 1));
                    speed.Normalize();
                    speed *= Main.rand.NextFloat(3f, 6f);
                    speed.Y -= Math.Abs(speed.X) * 0.2f;
                    speed.Y -= 3f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.SkeletonBone, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                }
            }
        }
    }
}
