using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class CurseoftheMoon : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Curse of the Moon");
            //Description.SetDefault("The moon's wrath consumes you");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "月之诅咒");
            //Description.AddTranslation(GameCulture.Chinese, "月亮的愤怒吞噬了你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= 20;
            player.endurance -= 0.20f;
            player.GetModPlayer<FargoPlayer>().AllDamageUp(-0.20f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(-20);
            player.GetModPlayer<FargoPlayer>().CurseoftheMoon = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().CurseoftheMoon = true;
        }
    }
}