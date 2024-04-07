using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Core.Globals;
using System.Data;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class TrojanSquirrelBag : BossBag
    {
        protected override bool IsPreHMBag => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoxofGizmos>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<TrojanSquirrel>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.Find<ModItem>("Fargowiltas", "LumberJaxe").Type, 5));
            itemLoot.Add(ItemDropRule.Common(ItemID.SquirrelHook));
            itemLoot.Add(ItemDropRule.Common(ItemID.WoodenCrate, 1, 5, 5));
            itemLoot.Add(ItemDropRule.Common(ItemID.HerbBag, 1, 5, 5));
            itemLoot.Add(ItemDropRule.Common(ItemID.Acorn, 1, 100, 100));
            itemLoot.Add(ItemDropRule.OneFromOptions(1, new int[]
            {
                ItemID.Squirrel,
                ItemID.SquirrelRed
            }));
            itemLoot.Add(ItemDropRule.OneFromOptions(1, new int[]
            {
                ModContent.ItemType<TreeSword>(),
                ModContent.ItemType<MountedAcornGun>(),
                ModContent.ItemType<SnowballStaff>(),
                ModContent.ItemType<KamikazeSquirrelStaff>()
            }));
        }
    }
}