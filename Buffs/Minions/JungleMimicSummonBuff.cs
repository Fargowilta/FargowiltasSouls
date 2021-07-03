using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Minions
{
    public class JungleMimicSummonBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Jungle Mimic");
            Description.SetDefault("The Jungle Mimic will fight for you");
            DisplayName.AddTranslation(GameCulture.Chinese, "丛林宝箱怪");
            Description.AddTranslation(GameCulture.Chinese, "丛林大宝箱怪会为你而战");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[mod.ProjectileType("JungleMimicSummon")] > 0)
            {
                player.buffTime[buffIndex] = 2;
            }
        }
    }
}
