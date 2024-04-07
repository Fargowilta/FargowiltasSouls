using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse
{
    public class Vampire : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Vampire,
            NPCID.VampireBat
        );

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Weak, 600);

            npc.life += hurtInfo.Damage * 2;
            if (npc.life > npc.lifeMax)
                npc.life = npc.lifeMax;
            CombatText.NewText(npc.Hitbox, CombatText.HealLife, hurtInfo.Damage * 2);

            npc.netUpdate = true;
        }
    }
}
