using FargowiltasSouls.Projectiles.Minions;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class EaterMinion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater of Worlds");
            Description.SetDefault("The mini Eater of Worlds will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "世界吞噬者");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "迷你世界吞噬者将会为你而战");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<EaterHead>()] > 0) modPlayer.EaterMinion = true;
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