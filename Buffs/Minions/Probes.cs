using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class Probes : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Probes");
            //Description.SetDefault("The probes will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "探测器");
            //Description.AddTranslation(GameCulture.Chinese, "探测器将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Probes = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("Probe1")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("Probe1"), 0, 9f, player.whoAmI);
                if (player.ownedProjectileCounts[mod.ProjectileType("Probe2")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("Probe2"), 0, 9f, player.whoAmI, 0f, -1f);
            }
        }
    }
}