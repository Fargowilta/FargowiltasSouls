using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeSpike : Projectiles.BossWeapons.SlimeSpikeFriendly
    {
        public override string Texture => "Terraria/Projectile_605";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 300;
        }
    }
}