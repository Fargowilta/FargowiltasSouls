using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class NanoInjection : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Nano Injection");
            Description.SetDefault("Life regeneration and stats reduced");
            DisplayName.AddTranslation(GameCulture.Chinese, "纳米注射");
            Description.AddTranslation(GameCulture.Chinese, "生命恢复速度减少,各项数值减少");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().NanoInjection = true;
            player.GetModPlayer<FargoPlayer>().AllDamageUp(-0.1f);
            player.moveSpeed -= 0.1f;
            player.statDefense -= 10;
        }
    }
}
