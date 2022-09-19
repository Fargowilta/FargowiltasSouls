using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.FrostMoon
{
    public class Nutcracker : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.Nutcracker, NPCID.NutcrackerSpinning);

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (FargoSoulsWorld.MasochistModeReal && target.Male)
            {
                if (FargoSoulsUtil.IsChinese())
                {
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + "的“坚果”裂开了。"), 999999, 0);
                }
                else
                {
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + " had his nuts cracked."), 999999, 0);
                }
            }
        }
    }
}
