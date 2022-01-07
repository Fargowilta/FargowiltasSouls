using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class OceanicMaul : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Oceanic Maul");
            //Description.SetDefault("Defensive stats and max life are savaged");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "海洋重击");
            //Description.AddTranslation(GameCulture.Chinese, "降低防御力和最大生命值");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().OceanicMaul = true;
            player.bleed = true;
            player.statDefense -= 15;
            player.endurance -= 0.15f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().OceanicMaul = true;
        }
    }
}