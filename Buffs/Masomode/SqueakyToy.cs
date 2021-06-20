using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class SqueakyToy : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Squeaky Toy");
            Description.SetDefault("Your attacks are squeaky toys!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "吱吱响的玩具");
            Description.AddTranslation(GameCulture.Chinese, "你的攻击如同玩具一般作响！");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //all attacks do one damage and make squeaky noises
            player.GetModPlayer<FargoPlayer>().SqueakyToy = true;
        }
    }
}
