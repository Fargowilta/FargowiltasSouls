using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Boss
{
    public class AbomPresence : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominable Presence");
            Description.SetDefault("Defense, damage reduction, and life regen reduced; Moon Leech effect");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "憎恶驾到");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "减少防御、伤害减免和生命恢复速度;附带月噬减益");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().noDodge = true;
            player.GetModPlayer<FargoSoulsPlayer>().noSupersonic = true;
            player.moonLeech = true;
            player.bleed = true;

            player.statDefense -= 20;
            player.endurance -= 0.2f;
        }
    }
}
