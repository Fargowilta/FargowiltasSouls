using FargowiltasSouls.Buffs.Pets;
using FargowiltasSouls.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Pets
{
    public class SpawnSack : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spawn Sack");
            Tooltip.SetDefault("Summons the spawn of Mutant\n'You think you're safe?'");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.value = Item.sellPrice(0, 5);
            Item.rare = -13;
            Item.shoot = ModContent.ProjectileType<MutantSpawn>();
            Item.buffType = ModContent.BuffType<MutantSpawnBuff>();
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;
                }
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}