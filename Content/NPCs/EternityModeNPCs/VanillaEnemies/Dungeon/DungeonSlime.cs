using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class DungeonSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DungeonSlime);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Blackout, 300);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (NPC.downedPlantBoss && FargoSoulsUtil.HostCheck)
            {
                int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.Paladin, velocity: new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 0)));
                if (n != Main.maxNPCs)
                {
                    Main.npc[n].GetGlobalNPC<Paladin>().IsSmallPaladin = true;
                }
            }
        }
    }
}
