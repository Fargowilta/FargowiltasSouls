using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Purified : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purified");
            Description.SetDefault("You are cleansed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "净化");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你被净化了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //purges all other buffs. does NOT play nice with luiafk?
            player.GetModPlayer<FargoSoulsPlayer>().Purified = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 3;
        }
    }
}