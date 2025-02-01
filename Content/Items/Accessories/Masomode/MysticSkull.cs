﻿using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class MysticSkull : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        static void Effects(Player player)
        {
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaMagnet = true;
            player.manaFlower = true;
        }

        public override void UpdateInventory(Player player) => Effects(player);

        public override void UpdateVanity(Player player) => Effects(player);

        public override void UpdateAccessory(Player player, bool hideVisual) => Effects(player);

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<MysticSkullInactive>());
            return false;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<MysticSkullInactive>());
        }
    }
    public class MysticSkullInactive : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 0;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        static void Effects(Player player)
        {
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaMagnet = true;
            player.manaFlower = true;
        }

        //public override void UpdateInventory(Player player) => Effects(player);

        public override void UpdateVanity(Player player) => Effects(player);

        public override void UpdateAccessory(Player player, bool hideVisual) => Effects(player);

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<MysticSkull>());
            return false;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<MysticSkull>());
        }
    }
}