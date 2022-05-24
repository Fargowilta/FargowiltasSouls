using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class GiantShelly : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.GiantShelly,
            NPCID.GiantShelly2
        );

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Slow, 120);
        }

        public override void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit)
        {
            base.OnHitByAnything(npc, player, damage, knockback, crit);

            if (npc.ai[0] == 3f)
            {
                Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 4;
                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileID.Stinger, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1, Main.myPlayer);
                FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], 12, MathHelper.Pi / 12, 1);
            }
        }
    }
}
