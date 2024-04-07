using FargowiltasSouls.Content.Projectiles.JungleMimic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class JungleMimicSummonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jungle Mimic");
            // Description.SetDefault("The Jungle Mimic will fight for you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<JungleMimicSummon>()] > 0)
            {
                player.buffTime[buffIndex] = 2;
            }
        }
    }
}