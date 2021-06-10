using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;


namespace FargowiltasSouls.Patreon.ManliestDove
{
    public class DoveBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Dove");
            DisplayName.AddTranslation(GameCulture.Chinese, "和平鸽");
            Description.SetDefault("A Dove is following you");
            Description.AddTranslation(GameCulture.Chinese, "一只和平鸽正跟着你");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<PatreonPlayer>().DovePet = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<DoveProj>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<DoveProj>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
