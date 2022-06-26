using FargowiltasSouls.Buffs.Pets;
using FargowiltasSouls.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Pets
{
    public class ChibiHat : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibi Hat");
            Tooltip.SetDefault("Summons Chibi Devi\nShe follows your mouse\n'Cute! Cute! Cute!'");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DukeFishronPetItem);
            Item.shoot = ModContent.ProjectileType<ChibiDevi>();
            Item.buffType = ModContent.BuffType<ChibiDeviBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            base.UseStyle(player, heldItemFrame);

            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}