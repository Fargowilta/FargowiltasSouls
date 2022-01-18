using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeMinionBuff : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Mini Prime");
            //Description.SetDefault("Skeletron Prime will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.ownedProjectileCounts[mod.ProjectileType("PrimeMinionProj")] > 0) patronPlayer.PrimeMinion = true;
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
