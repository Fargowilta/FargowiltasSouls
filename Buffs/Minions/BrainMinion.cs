using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class BrainMinion : ModBuff
    {
        public override void SetDefaults()
        {
            ////DisplayName.SetDefault("Brain of Cthulhu");
            ////Description.SetDefault("The mini Brain of Cthulhu will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            ////DisplayName.AddTranslation(GameCulture.Chinese, "克苏鲁之脑");
            ////Description.AddTranslation(GameCulture.Chinese, "迷你克苏鲁之脑将会为你而战");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.ownedProjectileCounts[mod.ProjectileType("BrainProj")] > 0) modPlayer.BrainMinion = true;
            if (!modPlayer.BrainMinion)
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