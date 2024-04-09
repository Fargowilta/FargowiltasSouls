using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PumpkinMoon
{
    public class PumpkinMoonBosses : EModeNPCBehaviour
    {
        public const int WAVELOCK = 15;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.MourningWood,
            NPCID.Pumpking
        );

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);


        }

        public override bool PreKill(NPC npc)
        {
            if (Main.pumpkinMoon && NPC.waveNumber < WAVELOCK)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.Heart);
                }
                return false;
            }

            return base.PreKill(npc);
        }
    }
}
