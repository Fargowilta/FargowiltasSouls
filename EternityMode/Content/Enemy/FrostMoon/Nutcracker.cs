using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

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
                target.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Nutcracker", target.name)), 999999, 0);
            }
        }
    }
}
