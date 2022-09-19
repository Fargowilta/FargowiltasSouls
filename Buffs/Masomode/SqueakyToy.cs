using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class SqueakyToy : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squeaky Toy");
            Description.SetDefault("Your attacks are squeaky toys!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "吱吱响的玩具");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的攻击如同玩具一般作响!");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //all attacks do one damage and make squeaky noises
            player.GetModPlayer<FargoSoulsPlayer>().SqueakyToy = true;
        }
    }
}