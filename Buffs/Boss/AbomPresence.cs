using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Boss
{
    public class AbomPresence : ModBuff
    {
        public override void SetDefaults()
        {
            /*DisplayName.SetDefault("Abominable Presence");
            Description.SetDefault("Defense, damage reduction, and life regen reduced; Moon Leech effect");
            DisplayName.AddTranslation(GameCulture.Chinese, "憎恶驾到");
            Description.AddTranslation(GameCulture.Chinese, "减少防御、伤害减免和生命恢复速度;附带月噬减益");*/
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().noDodge = true;
            player.GetModPlayer<FargoPlayer>().noSupersonic = true;
            player.moonLeech = true;
            player.bleed = true;

            player.statDefense -= 20;
            player.endurance -= 0.2f;
        }
    }
}
