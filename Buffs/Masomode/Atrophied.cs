using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Atrophied : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Atrophied");
            //Description.SetDefault("Your muscles are deteriorating");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "萎缩");
            //Description.AddTranslation(GameCulture.Chinese, "你的肌肉正在退化");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //melee silence hopefully plus damage reduced 99%, -all crit just in case
            player.GetModPlayer<FargoPlayer>().Atrophied = true;
            if (player.HeldItem.melee)
                player.GetModPlayer<FargoPlayer>().AttackSpeed -= 0.5f;
        }
    }
}