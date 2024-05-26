using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse
{
    public class Reaper : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Reaper);

        public int DashTimer;
        public int AttackTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 40, false, 199, default, ModContent.BuffType<MarkedforDeathBuff>(), BuffID.Obstructed);

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
                    ProjectileID.DeathSickle, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 3), 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<LivingWastelandBuff>(), 900);
            target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 600);
            target.AddBuff(ModContent.BuffType<UnluckyBuff>(), 60 * 30);
        }
    }
}
