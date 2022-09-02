using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.SolarEclipse
{
    public class Reaper : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Reaper);

        public int DashTimer;
        public int AttackTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 40, false, 199, default, ModContent.BuffType<MarkedforDeath>(), BuffID.Obstructed);

            if (++DashTimer >= 420)
            {
                DashTimer = 0;
                npc.TargetClosest();
                npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 12f;
                AttackTimer = 90;
            }

            if (AttackTimer >= 0 && --AttackTimer % 10 == 0)
            {
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ProjectileID.DeathSickle, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 3), 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
            target.AddBuff(ModContent.BuffType<Unlucky>(), 60 * 30);
        }
    }
}
