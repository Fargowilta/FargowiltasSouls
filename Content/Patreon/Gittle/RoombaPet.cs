using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Gittle
{
    public class RoombaPet : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Roomba");
            // Tooltip.SetDefault("Summons a Roomba to follow you around in hopes of cleaning the whole world");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "扫地机器人");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤一个扫地机器人跟随你,它希望清洁整个世界");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<RoombaPetProj>();
            Item.buffType = ModContent.BuffType<RoombaPetBuff>();
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