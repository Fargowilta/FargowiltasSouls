using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Misc;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.BossBags
{
    public class MutantBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override int BossBagNPC => ModContent.NPCType<NPCs.MutantBoss.MutantBoss>();

        public override void OpenBossBag(Player player)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<EternalEnergy>(), Main.rand.Next(6) + 15);
            }

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<Masochist>());
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<MutantsFury>());
        }
    }
}