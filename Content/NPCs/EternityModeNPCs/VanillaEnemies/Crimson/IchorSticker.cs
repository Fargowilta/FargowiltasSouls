using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Crimson
{
    public class IchorSticker : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IchorSticker);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Ichor, 600);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
                FargoSoulsUtil.XWay(5, npc.GetSource_FromThis(), npc.Center, ProjectileID.GoldenShowerHostile, 4, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 2);
        }
    }
}
