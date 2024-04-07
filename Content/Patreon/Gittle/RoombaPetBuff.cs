using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Gittle
{
    public class RoombaPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Roomba");
            // Description.SetDefault("This Roomba is following you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "扫地机器人");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "这个扫地机器人在跟着你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<PatreonPlayer>().RoombaPet = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<RoombaPetProj>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<RoombaPetProj>(), 0, 0f, player.whoAmI, 0f, 0);
            }
        }
    }
}