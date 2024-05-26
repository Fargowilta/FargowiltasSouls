using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class AngryBones : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.AngryBones,
            NPCID.AngryBonesBig,
            NPCID.AngryBonesBigHelmet,
            NPCID.AngryBonesBigMuscle
        );

        //public int BoneSprayTimer;
        public int BabyTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //if (--BoneSprayTimer > 0 && BoneSprayTimer % 6 == 0) //spray bones
            //{
            //    Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
            //    speed.Normalize();
            //    speed *= 5f;
            //    speed.Y -= Math.Abs(speed.X) * 0.2f;
            //    speed.Y -= 3f;
            //    if (FargoSoulsUtil.HostCheck)
            //        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.SkeletonBone, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
            //}

            if (npc.justHit)
            {
                //BoneSprayTimer = 120;
                BabyTimer += 20;
            }

            if (++BabyTimer > 300) //shoot baby guardians
            {
                BabyTimer = 0;
                if (FargoSoulsUtil.HostCheck && npc.HasValidTarget && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.SafeDirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<SkeletronGuardian2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
            {
                if (Main.rand.NextBool(5))
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.CursedSkull);

                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = new(Main.rand.Next(-50, 51), Main.rand.Next(-100, 1));
                    speed.Normalize();
                    speed *= Main.rand.NextFloat(3f, 6f);
                    speed.Y -= Math.Abs(speed.X) * 0.2f;
                    speed.Y -= 3f;
                    speed.Y *= Main.rand.NextFloat(1.5f);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.SkeletonBone, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                }
            }
        }
    }
}
