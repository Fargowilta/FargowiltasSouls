using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Misc
{
    public class AbomBag : SoulsItem
    {
        public override int BossBagNPC => ModContent.NPCType<NPCs.AbomBoss.AbomBoss>();

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
            player.QuickSpawnItem(mod.ItemType("AbomEnergy"), Main.rand.Next(16) + 15); // 15-30

            float chance = 3f;
            for (int i = 0; i < FargoSoulsWorld.downedChampions.Length; i++)
                if (FargoSoulsWorld.downedChampions[i])
                    chance += 0.5f;

            if (SoulConfig.Instance.PatreonFishron && Main.rand.NextFloat(100) < chance)
                player.QuickSpawnItem(mod.ItemType("StaffOfUnleashedOcean"));
        }
    }
}