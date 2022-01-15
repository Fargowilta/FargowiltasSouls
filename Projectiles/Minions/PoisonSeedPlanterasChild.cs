using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class PoisonSeedPlanterasChild : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_276";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Poison Seed");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.minion = true;
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
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("Infested"), 360);
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.Poisoned, 360);
        }
    }
}