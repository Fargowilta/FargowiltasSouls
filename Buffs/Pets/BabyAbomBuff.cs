using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Pets
{
    public class BabyAbomBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Baby Abom");
            Description.SetDefault("Kickflipping on a scythe");
            DisplayName.AddTranslation(GameCulture.Chinese, "憎恶之镰");
            Description.AddTranslation(GameCulture.Chinese, "跑来跑去的小镰刀");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<FargoPlayer>().BabyAbom = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.BabyAbom>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Pets.BabyAbom>(), 0, 0f, player.whoAmI);
            }
        }
    }
}
