using FargowiltasSouls.Items.Accessories.Expert;
using FargowiltasSouls.Items.Weapons.Challengers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.BossBags
{
    public class TrojanSquirrelBag : BossBag
    {
        protected override bool IsPreHMBag => true;

        public override int BossBagNPC => ModContent.NPCType<NPCs.Challengers.TrojanSquirrel>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<BoxofGizmos>());

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), Main.rand.Next(new int[] {
                ModContent.ItemType<TreeSword>(),
                ModContent.ItemType<MountedAcornGun>(),
                ModContent.ItemType<SnowballStaff>(),
                ModContent.ItemType<KamikazeSquirrelStaff>()
            }));

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), Main.rand.Next(new int[] {
                ItemID.Squirrel,
                ItemID.SquirrelRed,
                //ItemID.SquirrelGold,
                //ItemID.GemSquirrelAmber,
                //ItemID.GemSquirrelAmethyst,
                //ItemID.GemSquirrelDiamond,
                //ItemID.GemSquirrelEmerald,
                //ItemID.GemSquirrelRuby,
                //ItemID.GemSquirrelSapphire,
                //ItemID.GemSquirrelTopaz
            }));

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.WoodenCrate, 5);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.HerbBag, 5);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.Acorn, 100);

            if (Main.rand.NextBool(5))
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.Find<ModItem>("Fargowiltas", "LumberJaxe").Type);
        }
    }
}