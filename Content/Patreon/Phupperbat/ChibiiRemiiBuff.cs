using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Phupperbat
{
    public class ChibiiRemiiBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chibii Remii");
            // Description.SetDefault("Devil 'Remilia Stretch'");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<PatreonPlayer>().ChibiiRemii = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<ChibiiRemii>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Top, Vector2.Zero, ModContent.ProjectileType<ChibiiRemii>(), 0, 0f, player.whoAmI);
            }
        }
    }
}