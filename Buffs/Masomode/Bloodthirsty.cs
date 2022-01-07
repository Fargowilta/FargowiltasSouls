using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Bloodthirsty : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Bloodthirsty");
            //Description.SetDefault("Hugely increased enemy spawn rate");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            //longerExpertDebuff = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "嗜血");
            //Description.AddTranslation(GameCulture.Chinese, "极大提高刷怪速率");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //crazy spawn rate
            player.GetModPlayer<FargoPlayer>().Bloodthirsty = true;
        }
    }
}