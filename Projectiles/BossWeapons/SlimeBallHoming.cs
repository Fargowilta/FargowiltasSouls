using Terraria;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SlimeBallHoming : SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        int bounce;

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.penetrate = 2;
        }

        public override void AI()
        {
            base.AI();

            if (bounce == 0)
            {
                bounce = Main.rand.Next(90);
            }

            if (++projectile.localAI[0] == bounce)
            {
                //projectile.localAI[0] = 0;
                
                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC n = Main.npc[foundTarget];
                    projectile.velocity = projectile.DirectionTo(n.Center) * projectile.velocity.Length();
                }
            }
        }

        private int HomeOnTarget()
        {
            const bool homingCanAimAtWetEnemies = true;
            const float homingMaximumRangeInPixels = 1000;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(projectile) && (!n.wet || homingCanAimAtWetEnemies) && Collision.CanHitLine(projectile.Center, 0, 0, n.Center, 0, 0))
                {
                    float distance = projectile.Distance(n.Center);
                    if (distance <= homingMaximumRangeInPixels &&
                        (
                            selectedTarget == -1 || //there is no selected target
                            projectile.Distance(Main.npc[selectedTarget].Center) > distance) //or we are closer to this target than the already selected target
                    )
                        selectedTarget = i;
                }
            }

            return selectedTarget;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.immune[projectile.owner] = 9;
        }
    }
}