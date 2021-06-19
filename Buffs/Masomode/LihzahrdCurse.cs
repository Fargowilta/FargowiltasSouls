using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class LihzahrdCurse : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lihzahrd Curse");
            Description.SetDefault("Wires disabled in Jungle Temple");
            DisplayName.AddTranslation(GameCulture.Chinese, "蜥蜴人的诅咒");
            Description.AddTranslation(GameCulture.Chinese, "神庙中电路被禁用");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().LihzahrdCurse = true;
        }
    }
}
