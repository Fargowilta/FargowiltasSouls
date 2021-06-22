using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class TwinsEX : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Twins EX");
            Description.SetDefault("The real Twins will fight for you");
            DisplayName.AddTranslation(GameCulture.Chinese, "双子EX");
            Description.AddTranslation(GameCulture.Chinese, "真正的双子魔眼会为你而战");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[mod.ProjectileType("OpticRetinazer")] > 0)
            {
                player.GetModPlayer<FargoPlayer>().TwinsEX = true;
                player.buffTime[buffIndex] = 2;
            }
        }
    }
}
