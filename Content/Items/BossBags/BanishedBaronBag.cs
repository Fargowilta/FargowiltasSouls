using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class BanishedBaronBag : BossBag
    {
        protected override bool IsPreHMBag => true; //so it doesn't drop dev sets
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedOxygenTank>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<LifeChallenger>()));

            itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<TheBaronsTusk>(), ModContent.ItemType<RoseTintedVisor>(), ModContent.ItemType<NavalRustrifle>(), ModContent.ItemType<DecrepitAirstrikeRemote>()));
            itemLoot.Add(ItemDropRule.Common(5003, 1, 1, 5)); //seaside crate
            itemLoot.Add(ItemDropRule.OneFromOptions(3, ItemID.Sextant, ItemID.WeatherRadio, ItemID.FishermansGuide));
            itemLoot.Add(ItemDropRule.Common(ItemID.FishingBobber, 4, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ItemID.FishingPotion, 3, 2, 5));
            itemLoot.Add(ItemDropRule.Common(ItemID.SonarPotion, 2, 2, 5));
            itemLoot.Add(ItemDropRule.Common(ItemID.CratePotion, 5, 2, 5));
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldenBugNet, 50, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ItemID.FishHook, 50, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldenFishingRod, 150, 1, 1));
        }
    }
}