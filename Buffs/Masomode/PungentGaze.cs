using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class PungentGaze : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pungent Gaze");
            Main.debuff[Type] = true;
        }

        public const int MAX_TIME = 300;

        public override void Update(NPC npc, ref int buffIndex)
        {
            FargoSoulsGlobalNPC globalNPC = npc.GetGlobalNPC<FargoSoulsGlobalNPC>();

            int cap = globalNPC.PungentGazeWasApplied ? MAX_TIME : 2;

            if (npc.buffTime[buffIndex] > cap)
                npc.buffTime[buffIndex] = cap;

            globalNPC.PungentGazeTime = npc.buffTime[buffIndex];

            globalNPC.PungentGazeWasApplied = false;
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.buffTime[buffIndex] += time;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().PungentGazeWasApplied = true;
            return base.ReApply(npc, time, buffIndex);
        }
    }
}