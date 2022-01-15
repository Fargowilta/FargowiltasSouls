using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class FargoArrowProj : ModProjectile
    {
        private int _bounce = 6;
        private int[] dusts = new int[] { 130, 55, 133, 131, 132 };
        private int currentDust = 0;
        private int timer = 0;
        private Vector2 velocity;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fargo Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = -1; //same as luminite
            projectile.timeLeft = 200;
            projectile.light = 1f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.WoodenArrowFriendly;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
        }


        public override void AI()
        {
            //dust
            Dust.NewDust(projectile.position, projectile.width, projectile.height, dusts[currentDust], projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
            currentDust++;
            if (currentDust > 4)
            {
                currentDust = 0;
            }

            //luminite
            if (projectile.localAI[0] == 0f && projectile.localAI[1] == 0f)
            {
                projectile.localAI[0] = projectile.Center.X;
                projectile.localAI[1] = projectile.Center.Y;
                velocity = new Vector2(projectile.velocity.X, projectile.velocity.Y);
            }

            timer++;
            if (timer >= 60)
            {
                Player player = Main.player[projectile.owner];

                int num271 = Main.rand.Next(5, 10);
                for (int num272 = 0; num272 < num271; num272++)
                {
                    int num273 = Dust.NewDust(projectile.Center, 0, 0, 220, 0f, 0f, 100, default(Color), 0.5f);
                    Main.dust[num273].velocity *= 1.6f;
                    Dust expr_9845_cp_0 = Main.dust[num273];
                    expr_9845_cp_0.velocity.Y = expr_9845_cp_0.velocity.Y - 1f;
                    Main.dust[num273].position = Vector2.Lerp(Main.dust[num273].position, projectile.Center, 0.5f);
                    Main.dust[num273].noGravity = true;
                }
                int num274 = 1;
                int nextSlot = Projectile.GetNextSlot();
                if (Main.ProjectileUpdateLoopIndex < nextSlot && Main.ProjectileUpdateLoopIndex != -1)
                {
                    num274++;
                }
                int luminiteArrow = Projectile.NewProjectile(projectile.localAI[0], projectile.localAI[1], velocity.X, velocity.Y, ProjectileID.MoonlordArrowTrail, projectile.damage, projectile.knockBack, projectile.owner, 0f, (float)num274);
                timer = 0;

                Main.projectile[luminiteArrow].localNPCHitCooldown = 5;
                Main.projectile[luminiteArrow].usesLocalNPCImmunity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHit();

            //chloro
            if (_bounce > 1)
            {
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                _bounce--;
                if (projectile.velocity.X != oldVelocity.X) projectile.velocity.X = -oldVelocity.X;
                if (projectile.velocity.Y != oldVelocity.Y) projectile.velocity.Y = -oldVelocity.Y;
            }
            else
            {
                projectile.Kill();
            }

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHit();

            //flame
            target.AddBuff(BuffID.OnFire, 600);

            //frostburn
            target.AddBuff(BuffID.Frostburn, 600);

            //cursed
            target.AddBuff(BuffID.CursedInferno, 600);

            //ichor
            target.AddBuff(BuffID.Ichor, 600);

            //venom
            target.AddBuff(BuffID.Venom, 600);
        }

        public void OnHit()
        {
            //holy stars
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int num479 = 0; num479 < 10; num479++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
            }
            for (int num480 = 0; num480 < 3; num480++)
            {
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
            float x = projectile.position.X + (float)Main.rand.Next(-400, 400);
            float y = projectile.position.Y - (float)Main.rand.Next(600, 900);
            Vector2 vector12 = new Vector2(x, y);
            float num483 = projectile.position.X + (float)(projectile.width / 2) - vector12.X;
            float num484 = projectile.position.Y + (float)(projectile.height / 2) - vector12.Y;
            int num485 = 22;
            float num486 = (float)Math.Sqrt((double)(num483 * num483 + num484 * num484));
            num486 = (float)num485 / num486;
            num483 *= num486;
            num484 *= num486;
            int num487 = projectile.damage;
            int num488 = Projectile.NewProjectile(x, y, num483, num484, ProjectileID.HallowStar, num487, projectile.knockBack, projectile.owner, 0f, 0f);
            Main.projectile[num488].ai[1] = projectile.position.Y;
            Main.projectile[num488].ai[0] = 1f;

            Main.projectile[num488].localNPCHitCooldown = 2;
            Main.projectile[num488].usesLocalNPCImmunity = true;

            //hellfire explode
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int num613 = 0; num613 < 10; num613++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
            }
            for (int num614 = 0; num614 < 5; num614++)
            {
                int num615 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 2.5f);
                Main.dust[num615].noGravity = true;
                Main.dust[num615].velocity *= 3f;
                num615 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num615].velocity *= 2f;
            }
            int num616 = Gore.NewGore(projectile.position, default(Vector2), Main.rand.Next(61, 64), 1f);
            Main.gore[num616].velocity *= 0.4f;
            Gore expr_14419_cp_0 = Main.gore[num616];
            expr_14419_cp_0.velocity.X = expr_14419_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.1f;
            Gore expr_14449_cp_0 = Main.gore[num616];
            expr_14449_cp_0.velocity.Y = expr_14449_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.1f;
            num616 = Gore.NewGore(projectile.position, default(Vector2), Main.rand.Next(61, 64), 1f);
            Main.gore[num616].velocity *= 0.4f;
            Gore expr_144DD_cp_0 = Main.gore[num616];
            expr_144DD_cp_0.velocity.X = expr_144DD_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.1f;
            Gore expr_1450D_cp_0 = Main.gore[num616];
            expr_1450D_cp_0.velocity.Y = expr_1450D_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.1f;
            if (projectile.owner == Main.myPlayer)
            {
                projectile.penetrate = -1;
                projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
                projectile.width = 64;
                projectile.height = 64;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                projectile.Damage();
            }
        }

        public override void Kill(int timeleft)
        {
            OnHit();
        }
    }
}