using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class TargetCrosshair : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Target Crosshair");
        }
        public override void SetDefaults()
        {
            Projectile.width = 184;
            Projectile.height = 184;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 120;
        }

        public override void AI()
        {
            NPC lifelight = Main.npc[(int)Projectile.ai[1]];
            Player Player = Main.player[lifelight.target];
            if (Player.active && !Player.dead)
            {
                Projectile.position.X = Player.Center.X - Projectile.width / 2;
                Projectile.position.Y = Player.Center.Y - Projectile.height / 2;
            }
                if (Projectile.ai[0] > 60f)
            {
                Projectile.Kill();
            }
            Projectile.ai[0] += 1f;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
