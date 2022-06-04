using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.LunarEvents
{
    public class SolarEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.SolarCrawltipedeHead,
            NPCID.SolarCrawltipedeBody,
            NPCID.SolarCrawltipedeTail,
            NPCID.SolarCorite,
            NPCID.SolarSolenian,
            NPCID.SolarDrakomire,
            NPCID.SolarDrakomireRider,
            NPCID.SolarSpearman,
            NPCID.SolarSroller,
            NPCID.SolarFlare,
            NPCID.SolarGoop
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.OnFire3] = true;
            npc.buffImmune[BuffID.Ichor] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.Burning, 300);
        }
    }

    public class SolarCorite : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarCorite);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 250, BuffID.Burning, false, DustID.Torch);
        }
    }

    public class SolarCrawltipedeTail : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarCrawltipedeTail);

        public int Timer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (Timer >= 4)
            {
                Timer = 0;
                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1)
                {
                    Vector2 distance = Main.player[t].Center - npc.Center;
                    if (distance.Length() < 400f)
                    {
                        distance.Normalize();
                        distance *= 6f;
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance, ProjectileID.FlamesTrap, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        Main.projectile[p].friendly = false;
                        SoundEngine.PlaySound(SoundID.Item34, npc.Center);
                    }
                }
            }
        }
    }

    public class SolarFlare : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarFlare);

        public bool IsCultistProjectile;
        public int Timer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.noTileCollide = true;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss) && npc.Distance(Main.npc[EModeGlobalNPC.cultBoss].Center) < 3000)
            {
                IsCultistProjectile = true;
                if (!FargoSoulsWorld.MasochistModeReal)
                    npc.damage = (int)(npc.damage * .6);
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (IsCultistProjectile && !FargoSoulsWorld.SwarmActive && !FargoSoulsWorld.MasochistModeReal)
                npc.position += npc.velocity * Math.Min(0.5f, ++Timer / 60f - 1f);
        }
    }

    public class SolarGoop : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarGoop);

        public int Timer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.noTileCollide = true;
            npc.lavaImmune = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            Timer++;
            if (Timer >= 300)
            {
                npc.life = 0;
                npc.HitEffect();
                npc.checkDead();
                npc.active = false;
            }

            if (npc.HasPlayerTarget)
            {
                Vector2 speed = Main.player[npc.target].Center - npc.Center;
                speed.Normalize();
                speed *= 12f;

                npc.velocity.X += speed.X / 100f;

                if (npc.velocity.Length() > 16f)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= 16f;
                }
            }
            else if (Timer % 10 == 0)
            {
                npc.TargetClosest(false);
            }

            npc.dontTakeDamage = true;
        }
    }

    public class SolarSolenian : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarSolenian);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.knockBackResist = 0f;
        }
    }

    public class SolarSpearman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarSpearman);

        public override bool CheckDead(NPC npc)
        {
            int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
            if (t != -1 && Main.player[t].active && !Main.player[t].dead && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 velocity = Main.player[t].Center - npc.Center;
                velocity.Normalize();
                velocity *= 14f;
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ModContent.ProjectileType<DrakanianDaybreak>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f, Main.myPlayer);
            }
            SoundEngine.PlaySound(SoundID.Item1, npc.Center);
            if (Main.rand.NextBool())
            {
                npc.Transform(NPCID.SolarSolenian);
                return false;
            }

            return base.CheckDead(npc);
        }
    }

    public class SolarSroller : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SolarSroller);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.scale += 0.5f;
        }
    }
}
