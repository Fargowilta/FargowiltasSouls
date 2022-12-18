using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Dungeon
{
    public class DungeonSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DungeonSlime);

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Blackout, 300);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (NPC.downedPlantBoss && Main.netMode != NetmodeID.MultiplayerClient)
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
