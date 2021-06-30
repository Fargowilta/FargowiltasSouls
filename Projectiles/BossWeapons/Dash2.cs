using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Dash2 : Dash
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Dash";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 15 * 60 * (projectile.extraUpdates + 1);
        }
    }
}