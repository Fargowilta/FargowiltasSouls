using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class FlippedHallow : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Flipped");
            //Description.SetDefault("Your gravity is reversed");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "翻转");
            //Description.AddTranslation(GameCulture.Chinese, "你的重力颠倒了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.Center.Y / 16 > Main.worldSurface) //is underground
            {
                player.GetModPlayer<FargoPlayer>().Flipped = true;
            }
            else //above ground, purge debuff immediately
            {
                if (player.buffTime[buffIndex] > 2)
                    player.buffTime[buffIndex] = 2;
            }
        }
    }
}