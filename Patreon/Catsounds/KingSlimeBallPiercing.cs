using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeBallPiercing : Projectiles.BossWeapons.SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.penetrate = 2;
        }
    }
}