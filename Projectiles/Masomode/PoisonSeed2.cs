using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PoisonSeed2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_276";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Seed");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 1)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 1)
                    projectile.frame = 0;
            }

            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(mod.BuffType("Infested"), 180);
            target.AddBuff(mod.BuffType("IvyVenom"), 240);
        }
    }
}