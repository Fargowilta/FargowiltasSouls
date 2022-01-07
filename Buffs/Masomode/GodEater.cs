using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class GodEater : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("God Eater");
            //Description.SetDefault("Your soul is cursed by divine wrath");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "噬神者");
            //Description.AddTranslation(GameCulture.Chinese, "你的灵魂被神明的忿怒所诅咒");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //defense removed, endurance removed, colossal DOT (45 per second)
            player.GetModPlayer<FargoPlayer>().GodEater = true;
            player.GetModPlayer<FargoPlayer>().noDodge = true;
            player.GetModPlayer<FargoPlayer>().MutantPresence = true;
            player.moonLeech = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense = 0;
            npc.defDefense = 0;
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().GodEater = true;
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().HellFire = true;
        }
    }
}