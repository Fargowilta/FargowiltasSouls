using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Jammed : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Jammed");
            //Description.SetDefault("Your ranged weapons are faulty");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "卡壳");
            //Description.AddTranslation(GameCulture.Chinese, "你的远程武器出故障了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //all ranged weapons shoot confetti 
            player.GetModPlayer<FargoPlayer>().Jammed = true;
        }
    }
}