using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class InvisibleScytheHitbox : ModProjectile
    {
        bool init = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Ray");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;

        }

        public override void AI()
        {
            if (Main.LocalPlayer.active && Main.LocalPlayer.statLife <= 0)
            {
                Main.LocalPlayer.KillMe(PlayerDeathReason.ByCustomReason(Main.LocalPlayer.name + " was executed."), 999999, 0); //kills you 1 frame after it sets your health down
            }
            if (Main.LocalPlayer.active && Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.Hitbox))
            {
                Main.LocalPlayer.statLife = -100;
            }
            if (Projectile.ai[1] > 0)
            {
                Projectile.Kill();
            }
            Projectile.ai[1]++;
        }
    }
}

