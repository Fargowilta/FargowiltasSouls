using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Shucks
{
    public class CrimetroidBuff : ModBuff
    {
        public override void SetDefaults()
        {
            /*DisplayName.SetDefault("Baby Crimetroid");
            Description.SetDefault("The Baby is following you");*/
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<PatreonPlayer>().Crimetroid = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Crimetroid>()] < 1)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Crimetroid>(), 0, 0f, player.whoAmI);
            }
        }
    }
}