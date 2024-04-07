using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class Vulture : Shooters
    {
        public Vulture() : base(150, ModContent.ProjectileType<VultureFeather>(), 10, 1, DustID.Sand, 500) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Vulture);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(SwoopCount);
            binaryWriter.Write7BitEncodedInt(RandomTime);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            SwoopCount = binaryReader.Read7BitEncodedInt();
            RandomTime = binaryReader.Read7BitEncodedInt();
        }

        public int SwoopCount;
        public int RandomTime;
        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[0] == 0f) //no attack until moving
                AttackTimer = 0;

            // 1.7 ENEMY TODO: Finish vulture swoops, maybe replace the shooting part, add to harpies too, maybe give some tankiness and kb immunity while swooping
            /*
            if (RandomTime == 0)
            {
                RandomTime = Main.rand.Next(10, 200);
            }

            if (AttackTimer == AttackThreshold - Telegraph - (RandomTime + 5))
            {
                SwoopCount++;
                if (SwoopCount == 1)
                {
                    //do the swoop
                    SwoopCount = -1;
                }
            }
            */
        }
    }
}
