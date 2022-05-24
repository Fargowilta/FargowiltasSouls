using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Dungeon
{
    public class DungeonTeleporters : Teleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DiabolistRed,
            NPCID.DiabolistWhite,
            NPCID.Necromancer,
            NPCID.NecromancerArmored,
            NPCID.RaggedCaster,
            NPCID.RaggedCasterOpenCoat
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            switch (npc.type)
            {
                case NPCID.DiabolistRed:
                    if (Main.rand.NextBool(4))
                        npc.Transform(Main.rand.NextBool() ? NPCID.Necromancer : NPCID.RaggedCaster);
                    break;

                case NPCID.DiabolistWhite:
                    if (Main.rand.NextBool(4))
                        npc.Transform(Main.rand.NextBool() ? NPCID.NecromancerArmored : NPCID.RaggedCasterOpenCoat);
                    break;

                case NPCID.Necromancer:
                    if (Main.rand.NextBool(4))
                        npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistRed : NPCID.RaggedCaster);
                    break;

                case NPCID.NecromancerArmored:
                    if (Main.rand.NextBool(4))
                        npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistWhite : NPCID.RaggedCasterOpenCoat);
                    break;

                case NPCID.RaggedCaster:
                    if (Main.rand.NextBool(4))
                        npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistRed : NPCID.Necromancer);
                    break;

                case NPCID.RaggedCasterOpenCoat:
                    if (Main.rand.NextBool(4))
                        npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistWhite : NPCID.NecromancerArmored);
                    break;

                default:
                    break;
            }
        }

        public override void AI(NPC npc)
        {
            if (npc.HasValidTarget && !Main.player[npc.target].ZoneDungeon && !DoTeleport)
            {
                DoTeleport = true;
                TeleportTimer = TeleportThreshold - 420; //occasionally teleport outside dungeon
            }

            base.AI(npc);
        }
    }
}
