using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class PalmTreeSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Palm Tree");
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 82;
            projectile.aiStyle = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.timeLeft = 7200;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (player.active && !player.dead && modPlayer.PalmEnchant)
                projectile.timeLeft = 2;

            projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            projectile.ai[1] += 1f;

            int attackRate = 45;

            if (modPlayer.WoodForce || modPlayer.WizardEnchant)
            {
                attackRate = 30;
            }

            if (projectile.ai[1] >= attackRate)
            {
                float num = 2000f;
                int npcIndex = -1;
                for (int i = 0; i < 200; i++)
                {
                    float dist = Vector2.Distance(projectile.Center, Main.npc[i].Center);

                    if (dist < num && dist < 300 && Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        npcIndex = i;
                        num = dist;
                    }
                }

                if (npcIndex != -1)
                {
                    NPC target = Main.npc[npcIndex];

                    if (Collision.CanHit(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height))
                    {
                        Vector2 velocity = Vector2.Normalize(target.Center - projectile.Center) * 10;

                        int p = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.SeedlerNut, projectile.damage, 2, projectile.owner);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].minion = true;
                            Main.projectile[p].melee = false;
                        }

                    }
                }
                projectile.ai[1] = 0f;

                //kill if too far away
                if (Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center) > 2000)
                {
                    projectile.Kill();
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.position += projectile.velocity;
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}
