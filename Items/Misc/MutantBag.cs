using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Misc
{
    public class MutantBag : SoulsItem
    {
        public override int BossBagNPC => ModContent.NPCType<NPCs.MutantBoss.MutantBoss>();

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("Right click to open");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体的摸彩袋");
            Tooltip.AddTranslation(GameCulture.Chinese, "右键打开");*/
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.Purple;
        }

        public override void OpenBossBag(Player player)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                player.QuickSpawnItem(mod.ItemType("Sadism"), Main.rand.Next(6) + 15);
            }

            player.QuickSpawnItem(mod.ItemType("MutantsFury"));
        }
    }
}