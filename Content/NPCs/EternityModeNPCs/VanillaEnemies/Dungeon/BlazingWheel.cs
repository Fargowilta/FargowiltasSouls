using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class BlazingWheel : SpikeBall
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BlazingWheel);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale *= 2f;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (OutsideDungeon)
                target.AddBuff(BuffID.Burning, 300);
        }
    }
}
