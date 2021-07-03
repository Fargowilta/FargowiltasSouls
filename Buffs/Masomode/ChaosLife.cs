using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class ChaosLife : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Chaos Life");
            Description.SetDefault("Max life reduced");
            DisplayName.AddTranslation(GameCulture.Chinese, "生命混沌");
            Description.AddTranslation(GameCulture.Chinese, "减少最大生命值");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false; ;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().OceanicMaul = true;
            if (player.buffTime[buffIndex] < 30 && NPCs.EModeGlobalNPC.AnyBossAlive())
                player.buffTime[buffIndex] = 30;
        }
    }
}
