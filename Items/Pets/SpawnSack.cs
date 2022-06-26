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
            Item.CloneDefaults(ItemID.DukeFishronPetItem);
            Item.shoot = ModContent.ProjectileType<MutantSpawn>();
            Item.buffType = ModContent.BuffType<MutantSpawnBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}