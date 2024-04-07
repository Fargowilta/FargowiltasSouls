﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class SandsofTime : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sands of Time");
            /* Tooltip.SetDefault(@"Works in your inventory
Grants immunity to Mighty Wind
You respawn twice as fast when no boss is alive
Use to teleport to your last death point
'Whatever you do, don't drop it'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "时之沙");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'无论你做什么,都不要丢下它'
            // 放在物品栏中即可生效
            // 免疫强风和仙人掌伤害
            // 当没有Boss存活时,重生速度加倍
            // 按下快捷键传送到上次死亡地点");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 4);

            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item6;
        }

        static void PassiveEffect(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.FargoSouls().SandsofTime = true;
            player.FargoSouls().CactusImmune = true;
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual) => PassiveEffect(player);

        public static void Use(Player player)
        {
            if (player.itemTime == player.itemTimeMax / 2 && player.lastDeathPostion != Vector2.Zero)
            {
                for (int index = 0; index < 70; ++index)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemTopaz, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, new Color(), 1.5f);
                    Main.dust[d].velocity *= 4f;
                    Main.dust[d].noGravity = true;
                }

                player.grappling[0] = -1;
                player.grapCount = 0;
                for (int index = 0; index < Main.maxProjectiles; ++index)
                {
                    if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                        Main.projectile[index].Kill();
                }

                if (player.whoAmI == Main.myPlayer)
                {
                    player.Teleport(player.lastDeathPostion, 1);
                    player.velocity = Vector2.Zero;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, player.lastDeathPostion.X, player.lastDeathPostion.Y, 1);
                }

                for (int index = 0; index < 70; ++index)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemTopaz, 0.0f, 0.0f, 150, new Color(), 1.5f);
                    Main.dust[d].velocity *= 4f;
                    Main.dust[d].noGravity = true;
                }
            }
        }

        public override void UseItemFrame(Player player) => Use(player);
        public override bool? UseItem(Player player) => true;
    }
}