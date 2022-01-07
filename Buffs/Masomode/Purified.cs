using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Purified : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Purified");
            //Description.SetDefault("You are cleansed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "净化");
            //Description.AddTranslation(GameCulture.Chinese, "你被净化了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //purges all other buffs. does NOT play nice with luiafk?
            player.GetModPlayer<FargoPlayer>().Purified = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 2;
        }
    }
}