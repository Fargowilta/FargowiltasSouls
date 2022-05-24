using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Boss
{
    public class MutantPresence : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Presence");
            Description.SetDefault("Defense, damage reduction, and life regen reduced; almost all soul toggles disabled; reduced graze radius");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变驾到");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "减少防御、伤害减免和生命恢复速度; 关闭近乎所有魂的效果; 附带混沌状态减益");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //also halves defense, DR, and cripples life regen
            player.GetModPlayer<FargoSoulsPlayer>().noDodge = true;
            player.GetModPlayer<FargoSoulsPlayer>().noSupersonic = true;
            player.GetModPlayer<FargoSoulsPlayer>().MutantPresence = true;
            player.GetModPlayer<FargoSoulsPlayer>().GrazeRadius *= 0.5f;
            player.moonLeech = true;
        }
    }
}
