﻿using FargowiltasSouls.Buffs.Pets;
using FargowiltasSouls.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Pets
{
    public class LifelightMasterPet : SoulsItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gold-Tinged Feather");
            Tooltip.SetDefault("Summons a lesser angel that provides light");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DukeFishronPetItem);
            Item.width = 20;
            Item.height = 40;
            Item.shoot = ModContent.ProjectileType<BabyLifelight>();
            Item.buffType = ModContent.BuffType<BabyLifelightBuff>();
        }

        public override void UseStyle(Player player, Rectangle 
            heldItemFrame)
        {
            base.UseStyle(player, heldItemFrame);

            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}