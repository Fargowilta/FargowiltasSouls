using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class AbomFangBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominable Fang");
            // Description.SetDefault("The power of Eternity Mode compels you");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "憎恶毒牙");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "永恒模式的力量压迫着你");
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            player.ichor = true;
            player.onFire2 = true;
            player.electrified = true;
            player.moonLeech = true;
        }
    }
}
