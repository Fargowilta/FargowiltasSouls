using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SandsofTime : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sands of Time");
            Tooltip.SetDefault(@"Works in your inventory
Grants immunity to Mighty Wind and cactus damage
You respawn twice as fast when no boss is alive
Use to teleport to your last death point
'Whatever you do, don't drop it'");
            DisplayName.AddTranslation(GameCulture.Chinese, "时之沙");
            Tooltip.AddTranslation(GameCulture.Chinese, @"放置在背包中即可生效
使你免疫强风减益和仙人掌刺伤
非Boss战期间会使你的重生速度翻倍
使用此饰品后会将你传送至上一次死亡时的地点
'无论你做什么都不要丢下这个东西'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);

            item.useTime = 90;
            item.useAnimation = 90;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTurn = true;
            item.UseSound = SoundID.Item6;
        }

        public override void UpdateInventory(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;

            //respawn faster ech
            player.GetModPlayer<FargoPlayer>().SandsofTime = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.lastDeathPostion != Vector2.Zero;
        }

        public override bool UseItem(Player player)
        {
            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int index = 0; index < 1000; ++index)
            {
                if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                    Main.projectile[index].Kill();
            }

            if (player.whoAmI == Main.myPlayer)
            {
                player.Teleport(player.lastDeathPostion, 1);
                player.velocity = Vector2.Zero;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, player.lastDeathPostion.X, player.lastDeathPostion.Y, 1);
            }

            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, 0.0f, 0.0f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            return true;
        }
    }
}
