using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeMinion : ModProjectile
    {
        public bool goingDown = false;
        public int spikeAttackCounter;
        public int slimeAttackCounter;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("King Slime");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.alpha = 75;
            projectile.width = 38;
            projectile.height = 40;
            projectile.timeLeft *= 5;
            projectile.aiStyle = 26;
            aiType = ProjectileID.BabySlime;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<PatreonPlayer>().KingSlimeMinion)
                projectile.timeLeft = 2;
            else
                projectile.Kill();

            if (projectile.damage == 0)
            {
                projectile.damage = (int)(15 * player.minionDamage);
                if (player.GetModPlayer<FargoPlayer>().MasochistSoul)
                    projectile.damage *= 2;
            }

            //no attacks while flying 
            if (projectile.frame >= 2) return;

            //spike attack
            if (goingDown)
            {
                if (projectile.velocity.Y <= 0f) //start attack
                {
                    goingDown = false;
                    spikeAttackCounter++;

                    if (spikeAttackCounter >= 10)
                    {
                        spikeAttackCounter = 0;
                        if (projectile.owner == Main.myPlayer && FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 400, true) != -1)
                        {
                            for (int i = 0; i < 25; i++)
                            {
                                Projectile.NewProjectile(new Vector2(projectile.Center.X + Main.rand.Next(-5, 5), projectile.Center.Y - 15),
                                    new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)),
                                    ModContent.ProjectileType<KingSlimeSpike>(), projectile.damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                    
                }
            }
            else if (projectile.velocity.Y > 0)
            {
                goingDown = true;
            }

            //slime rain attack
            if (++slimeAttackCounter > 150)
            {
                slimeAttackCounter = 0;

                int target = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 800, true);
                if (target != -1 && projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 spawn = Main.npc[target].Center + Main.npc[target].velocity * Main.rand.NextFloat(15f);
                        spawn.X += Main.rand.Next(-50, 51);
                        spawn.Y -= Main.rand.Next(600, 701);
                        Vector2 speed = Main.npc[target].Center - spawn;
                        speed.Normalize();
                        speed *= Main.rand.NextFloat(10f, 20f);
                        Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<KingSlimeBallPiercing>(), projectile.damage, 0f, Main.myPlayer);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = Main.player[projectile.owner].Center.Y > projectile.position.Y + projectile.height;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }
    }
}