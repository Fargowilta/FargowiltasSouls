using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.FrostMoon
{
    public class Nutcracker : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.Nutcracker, NPCID.NutcrackerSpinning);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (WorldSavingSystem.MasochistModeReal && target.Male)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Nutcracker", target.name)), 999999, 0);
            }
        }
    }
}
