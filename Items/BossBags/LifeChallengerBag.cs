using FargowiltasSouls.Items.Accessories.Expert;
using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.Items.Weapons.Challengers;
using System.Data;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.BossBags
{
    public class LifeChallengerBag : BossBag
    {
        protected override bool IsPreHMBag => true;

        public override int BossBagNPC => ModContent.NPCType<NPCs.Challengers.LifeChallenger>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<LifeRevitalizer>());

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), Main.rand.Next(new int[] {
                ModContent.ItemType<EnchantedLifeblade>(),
                ModContent.ItemType<Lightslinger>(),
                ModContent.ItemType<CrystallineCongregation>(),
                ModContent.ItemType<KamikazePixieStaff>()
            }));

            /*player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), Main.rand.Next(new int[] {
            }));*/

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), 3207, 5);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.SoulofLight, 3);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.PixieDust, 25);

        }
    }
}