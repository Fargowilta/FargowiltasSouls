using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Crimson
{
    public class IchorSticker : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IchorSticker);

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Ichor, 600);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                FargoSoulsUtil.XWay(5, npc.GetSource_FromThis(), npc.Center, ProjectileID.GoldenShowerHostile, 4, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 2);
        }
    }
}
