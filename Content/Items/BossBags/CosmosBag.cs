using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class CosmosBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<UniverseCore>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<CosmosChampion>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Eridanium>(), 1, 10, 10));
            itemLoot.Add(ItemDropRule.Common(ModContent.Find<ModItem>("Fargowiltas", "CrucibleCosmos").Type));
        }
    }
}