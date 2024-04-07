using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class GiantShelly : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.GiantShelly,
            NPCID.GiantShelly2
        );

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slow, 120);
        }

        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);

            if (npc.ai[0] == 3f)
            {
                Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 4;
                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileID.Stinger, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1, Main.myPlayer);
                FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], 12, MathHelper.Pi / 12, 1);
            }
        }
    }
}
