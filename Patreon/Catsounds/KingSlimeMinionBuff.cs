using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeMinionBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mini King Slime");
            Description.SetDefault("This Mini King Slime will protect you");
            DisplayName.AddTranslation(GameCulture.Chinese, "迷你史莱姆王");
            Description.AddTranslation(GameCulture.Chinese, "迷你史莱姆王会保护你");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PatreonPlayer>().KingSlimeMinion = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<KingSlimeMinion>()] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<KingSlimeMinion>(), 0, 3f, player.whoAmI);
            }
        }
    }
}
