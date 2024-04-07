using FargowiltasSouls.Content.Projectiles.Minions;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class BigBrainMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Big Brain of Cthulhu");
            // Description.SetDefault("The Brain of Cthulhu will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BigBrainProj>()] > 0) modPlayer.BigBrainMinion = true;
            if (!modPlayer.BigBrainMinion)
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