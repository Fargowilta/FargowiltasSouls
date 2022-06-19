using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Berserked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Berserked");
            Description.SetDefault("10% increased damage and speed but you cannot control yourself");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "狂暴");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你控几不住你记几");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //causes player to constantly use weapon
            //seemed to have strange interactions with stunning debuffs like frozen or stoned...
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetModPlayer<FargoSoulsPlayer>().Berserked = true;
            player.moveSpeed += 0.1f;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 3;
        }
    }
}