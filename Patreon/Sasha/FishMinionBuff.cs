using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Sasha
{
    public class FishMinionBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fish");
            DisplayName.AddTranslation(GameCulture.Chinese, "鱼");
            Description.SetDefault("This fish will fight for you");
            Description.AddTranslation(GameCulture.Chinese, "鱼会为你而战");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.ownedProjectileCounts[mod.ProjectileType("FishMinion")] > 0) modPlayer.FishMinion = true;
            if (!modPlayer.FishMinion)
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
