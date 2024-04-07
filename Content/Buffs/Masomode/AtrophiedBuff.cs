using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class AtrophiedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Atrophied");
            // Description.SetDefault("Your muscles are deteriorating");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "萎缩");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的肌肉正在退化");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //melee silence hopefully plus damage reduced 99%, -all crit just in case
            player.FargoSouls().Atrophied = true;
            if (player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee) || player.HeldItem.DamageType.CountsAsClass(DamageClass.Throwing))
                player.FargoSouls().AttackSpeed -= 0.5f;
        }
    }
}