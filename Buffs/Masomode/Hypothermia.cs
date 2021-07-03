using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Hypothermia : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hypothermia");
            Description.SetDefault("Increased damage taken from cold attacks");
            DisplayName.AddTranslation(GameCulture.Chinese, "失温");
            Description.AddTranslation(GameCulture.Chinese, "冷系攻击对你造成更多伤害");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Hypothermia = true;
        }
    }
}
