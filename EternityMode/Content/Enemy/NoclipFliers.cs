using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class NoclipFliers : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchTypeRange(
                NPCID.Harpy,
                NPCID.EaterofSouls,
                NPCID.BigEater,
                NPCID.LittleEater,
                NPCID.Crimera
            );

        public int MPSyncSpawnTimer = 30;

        public bool CanNoclip;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(MPSyncSpawnTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(CanNoclip), BoolStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (MPSyncSpawnTimer > 0)
            {
                if (--MPSyncSpawnTimer == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool())
                        CanNoclip = npc.type != NPCID.EaterofSouls || NPC.downedBoss2;

                    npc.netUpdate = true;
                    NetSync(npc);
                }
            }

            npc.noTileCollide = CanNoclip && npc.HasPlayerTarget && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0);
        }
    }
}
