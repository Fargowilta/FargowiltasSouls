using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.SkyAndRain
{
    public class SkyEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Harpy,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernHead,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.AngryNimbus,
            NPCID.MartianProbe
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }
    }
}
