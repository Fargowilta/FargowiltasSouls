using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class MegaFlameburst : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_668";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mega Flameburst");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 28;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            projectile.scale = 2f;

            if (projectile.ai[1] == 0)
            {
                projectile.ai[1] = 1;

                const int num226 = 12;
                for (int i = 0; i < num226; i++)
                {
                    Vector2 vector6 = Vector2.UnitX.RotatedBy(projectile.rotation) * 6f;
                    vector6 = vector6.RotatedBy(((i - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.FlameBurst, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
            }

            const int aislotHomingCooldown = 0;
            const int homingDelay = 0;
            const float desiredFlySpeedInPixelsPerFrame = 8;
            const float amountOfFramesToLerpBy = 25; // minimum of 1, please keep in full numbers even though it's a float!

            projectile.ai[aislotHomingCooldown]++;
            if (projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 
                
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000));
                if (n != null)
                {
                    Vector2 desiredVelocity = projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void Kill(int timeleft)
        {
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, projectile.damage, 0, projectile.owner);
            if (p != Main.maxProjectiles)
                Main.projectile[p].timeLeft = 15;
        }
    }
}