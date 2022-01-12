using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class EaterMinion : ModBuff
    {
        public override void SetDefaults()
        {
            ////DisplayName.SetDefault("Eater of Worlds");
            ////Description.SetDefault("The mini Eater of Worlds will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            ////DisplayName.AddTranslation(GameCulture.Chinese, "世界吞噬者");
            ////Description.AddTranslation(GameCulture.Chinese, "迷你世界吞噬者将会为你而战");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.ownedProjectileCounts[mod.ProjectileType("EaterHead")] > 0) modPlayer.EaterMinion = true;
            if (!modPlayer.EaterMinion)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}