using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
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


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(MPSyncSpawnTimer);
            bitWriter.WriteBit(CanNoclip);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            MPSyncSpawnTimer = binaryReader.Read7BitEncodedInt();
            CanNoclip = bitReader.ReadBit();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (MPSyncSpawnTimer > 0)
            {
                if (--MPSyncSpawnTimer == 0)
                {
                    if (FargoSoulsUtil.HostCheck && Main.rand.NextBool())
                        CanNoclip = npc.type != NPCID.EaterofSouls || NPC.downedBoss2;

                    npc.netUpdate = true;
                    NetSync(npc);
                }
            }

            npc.noTileCollide = CanNoclip && npc.HasPlayerTarget && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0);
        }
    }
}
