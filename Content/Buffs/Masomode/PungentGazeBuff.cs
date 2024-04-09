using FargowiltasSouls.Core.Globals;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class PungentGazeBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pungent Gaze");
            Main.debuff[Type] = true;
        }

        public const int MAX_TIME = 300;

        public override void Update(NPC npc, ref int buffIndex)
        {
            FargoSoulsGlobalNPC globalNPC = npc.FargoSouls();

            int cap = globalNPC.PungentGazeWasApplied ? MAX_TIME : 2;

            if (npc.buffTime[buffIndex] > cap)
                npc.buffTime[buffIndex] = cap;

            globalNPC.PungentGazeTime = npc.buffTime[buffIndex];

            globalNPC.PungentGazeWasApplied = false;
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.buffTime[buffIndex] += time;
            npc.FargoSouls().PungentGazeWasApplied = true;
            return base.ReApply(npc, time, buffIndex);
        }
    }
}