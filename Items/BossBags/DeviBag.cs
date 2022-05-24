using FargowiltasSouls.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.BossBags
{
    public class DeviBag : BossBag
    {
        protected override bool IsPreHMBag => true;

        public override int BossBagNPC => ModContent.NPCType<NPCs.DeviBoss.DeviBoss>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<DeviatingEnergy>(), Main.rand.Next(16) + 15);
        }
    }
}