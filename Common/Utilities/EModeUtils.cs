using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Utilities
{
    public static class EModeUtils
    {
        public static void DropSummon(NPC npc, int itemType, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(npc.GetSource_Loot(), player.Hitbox, itemType);
                droppedSummonFlag = true;
            }
        }

        public static void DropSummon(NPC npc, string itemName, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                if (ModContent.TryFind("Fargowiltas", itemName, out ModItem modItem))
                    Item.NewItem(npc.GetSource_Loot(), player.Hitbox, modItem.Type);
                droppedSummonFlag = true;
            }
        }
    }
}
