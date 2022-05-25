using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.LunarEvents
{
    public class StardustEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.StardustCellBig,
            NPCID.StardustCellSmall,
            NPCID.StardustWormHead,
            NPCID.StardustWormBody,
            NPCID.StardustWormTail,
            NPCID.StardustSpiderBig,
            NPCID.StardustSpiderSmall,
            NPCID.StardustJellyfishBig,
            NPCID.StardustJellyfishSmall,
            NPCID.StardustSoldier
        );

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool CheckDead(NPC npc)
        {
            return base.CheckDead(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Obstructed, 20);
            target.AddBuff(BuffID.Blackout, 300);
        }
    }

    public class StardustSplittingEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.StardustJellyfishBig,
            NPCID.StardustSoldier,
            NPCID.StardustSpiderBig,
            NPCID.StardustWormHead
        );

        public override bool CheckDead(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int tower = NPC.FindFirstNPC(NPCID.LunarTowerStardust);
                if (tower != -1 && NPC.CountNPCS(NPCID.StardustCellSmall) < 10 && Main.npc[tower].active && npc.Distance(Main.npc[tower].Center) < 5000) //in tower range
                {
                    for (int i = 0; i < 3; i++) //spawn stardust cells
                    {
                        int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.StardustCellSmall);
                        if (n < Main.maxNPCs)
                        {
                            Main.npc[n].velocity.X = Main.rand.Next(-10, 11);
                            Main.npc[n].velocity.Y = Main.rand.Next(-10, 11);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }
            }

            return base.CheckDead(npc);
        }
    }

    public class StardustCellSmall : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.StardustCellSmall);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[0] >= 270f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.Transform(Main.rand.Next(new int[] {
                    NPCID.StardustJellyfishBig,
                    NPCID.StardustSpiderBig,
                    NPCID.StardustWormHead,
                    NPCID.StardustCellBig
                }));
            }
        }
    }
}
