using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Infested : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Infested");
            //Description.SetDefault("This can only get worse");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "感染");
            //Description.AddTranslation(GameCulture.Chinese, "这只会变得更糟");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoPlayer p = player.GetModPlayer<FargoPlayer>();

            //weak DOT that grows exponentially stronger
            if (p.FirstInfection)
            {
                p.MaxInfestTime = player.buffTime[buffIndex];
                p.FirstInfection = false;
            }

            p.Infested = true;
        }

        /*public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }*/

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().Infested = true;
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
}