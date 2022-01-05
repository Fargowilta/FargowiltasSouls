using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Boss
{
    public class DeviPresence : ModBuff
    {
        public override void SetDefaults()
        {
            /*DisplayName.SetDefault("Deviant Presence");
            Description.SetDefault("Friendly NPCs take massively increased damage");
            DisplayName.AddTranslation(GameCulture.Chinese, "戴维安驾到");
            Description.AddTranslation(GameCulture.Chinese, "大幅增加友方NPC受到的伤害");*/
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().DevianttPresence = true;
        }
    }
}
