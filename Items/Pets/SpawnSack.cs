using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Pets
{
    public class SpawnSack : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spawn Sack");
            Tooltip.SetDefault("Summons the spawn of Mutant\n'You think you're safe?'");
            DisplayName.AddTranslation(GameCulture.Chinese, "召唤物麻袋");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤迷你突变体\n'你觉得你很安全吗?'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.value = Item.sellPrice(0, 5);
            item.rare = -13;
            item.shoot = mod.ProjectileType("MutantSpawn");
            item.buffType = mod.BuffType("MutantSpawnBuff");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = Main.DiscoColor;
                }
            }
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
