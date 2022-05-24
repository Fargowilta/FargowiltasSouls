using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Boss
{
    public class Grabbed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grabbed");
            Description.SetDefault("Mash movement keys to escape!");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "抓住你了！");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "狂点你的移动键来逃离这个！");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            fargoPlayer.Mash = true;
        }
    }
}
