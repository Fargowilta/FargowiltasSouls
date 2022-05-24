using FargowiltasSouls.Items.Accessories.Expert;
using FargowiltasSouls.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.BossBags
{
    public class CosmosBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override int BossBagNPC => ModContent.NPCType<NPCs.Champions.CosmosChampion>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<Eridanium>(), 10);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<UniverseCore>());
        }
    }
}