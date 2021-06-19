using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class ObsidianLavaWetBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lava Wet");
            Description.SetDefault("You are dripping lava");
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "浸入岩浆");
            Description.AddTranslation(GameCulture.Chinese, "岩浆从你身上滴落");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().LavaWet = true;
        }
    }
}
