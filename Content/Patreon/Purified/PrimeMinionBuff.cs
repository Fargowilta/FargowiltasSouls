using Terraria;
using Terraria.ModLoader;
namespace FargowiltasSouls.Content.Patreon.Purified
{
    public class PrimeMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Prime");
            // Description.SetDefault("Skeletron Prime will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PrimeMinionProj>()] > 0) patronPlayer.PrimeMinion = true;
            if (!patronPlayer.PrimeMinion)
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
