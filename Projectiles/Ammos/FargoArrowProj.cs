using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Ammos
{
    public class FargoArrowProj : ModProjectile
    {
        private int[] dusts = new int[] { 130, 55, 133, 131, 132 };
        private int currentDust = 0;
        private int timer = 0;
        private Vector2 velocity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fargo Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = -1; //same as luminite
            Projectile.timeLeft = 200;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.WoodenArrowFriendly;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }


        public override void AI()
        {
            //dust
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dusts[currentDust], Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
            currentDust++;
            if (currentDust > 4)
            {
                currentDust = 0;
            }

            //luminite
            if (Projectile.localAI[0] == 0f && Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[0] = Projectile.Center.X;
                Projectile.localAI[1] = Projectile.Center.Y;
                velocity = new Vector2(Projectile.velocity.X, Projectile.velocity.Y);
            }

            timer++;
            if (timer >= 60)
            {
                Player player = Main.player[Projectile.owner];

                int num271 = Main.rand.Next(5, 10);
                for (int num272 = 0; num272 < num271; num272++)
                {
                    int num273 = Dust.NewDust(Projectile.Center, 0, 0, 220, 0f, 0f, 100, default(Color), 0.5f);
                    Main.dust[num273].velocity *= 1.6f;
                    Dust expr_9845_cp_0 = Main.dust[num273];
                    expr_9845_cp_0.velocity.Y = expr_9845_cp_0.velocity.Y - 1f;
                    Main.dust[num273].position = Vector2.Lerp(Main.dust[num273].position, Projectile.Center, 0.5f);
                    Main.dust[num273].noGravity = true;
                }
                int num274 = 1;
                int nextSlot = Projectile.GetNextSlot();
                if (Main.ProjectileUpdateLoopIndex < nextSlot && Main.ProjectileUpdateLoopIndex != -1)
                {
                    num274++;
                }
                int luminiteArrow = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.localAI[0], Projectile.localAI[1], velocity.X, velocity.Y, ProjectileID.MoonlordArrowTrail, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, (float)num274);
                timer = 0;

                Main.projectile[luminiteArrow].localNPCHitCooldown = 5;
                Main.projectile[luminiteArrow].usesLocalNPCImmunity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHit();

            //chloro
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] == 1f)
            {
                Projectile.damage = (int)((float)Projectile.damage * 0.66f);
            }
            if (Projectile.ai[1] >= 10f)
            {
                Projectile.Kill();
            }
            Projectile.velocity.X = -velocity.X;
            Projectile.velocity.Y = -velocity.Y;
            int num22 = Projectile.FindTargetWithLineOfSight(800f);
            if (num22 != -1)
            {
                NPC npc = Main.npc[num22];
                float t = Projectile.Distance(npc.Center);
                Vector2 value3 = -Vector2.UnitY * MathHelper.Lerp((float)npc.height * 0.1f, (float)npc.height * 0.5f, Utils.GetLerpValue(0f, 300f, t, false));
                Projectile.velocity = Projectile.DirectionTo(npc.Center + value3).SafeNormalize(-Vector2.UnitY) * Projectile.velocity.Length();
                Projectile.netUpdate = true;
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
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int num479 = 0; num479 < 10; num479++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
            }
            for (int num480 = 0; num480 < 3; num480++)
            {
                if (!Main.dedServ)
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
            float x = Projectile.position.X + (float)Main.rand.Next(-400, 400);
            float y = Projectile.position.Y - (float)Main.rand.Next(600, 900);
            Vector2 vector12 = new Vector2(x, y);
            float num483 = Projectile.position.X + (float)(Projectile.width / 2) - vector12.X;
            float num484 = Projectile.position.Y + (float)(Projectile.height / 2) - vector12.Y;
            int num485 = 22;
            float num486 = (float)Math.Sqrt((double)(num483 * num483 + num484 * num484));
            num486 = (float)num485 / num486;
            num483 *= num486;
            num484 *= num486;
            int num487 = Projectile.damage;
            int num488 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, num483, num484, ProjectileID.HallowStar, num487, Projectile.knockBack, Projectile.owner, 0f, 0);
            Main.projectile[num488].ai[1] = Projectile.position.Y;
            Main.projectile[num488].ai[0] = 1f;

            Main.projectile[num488].localNPCHitCooldown = 2;
            Main.projectile[num488].usesLocalNPCImmunity = true;

            //hellfire explode
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int num613 = 0; num613 < 10; num613++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
            }
            for (int num614 = 0; num614 < 5; num614++)
            {
                int num615 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 2.5f);
                Main.dust[num615].noGravity = true;
                Main.dust[num615].velocity *= 3f;
                num615 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num615].velocity *= 2f;
            }
            int num616 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, default(Vector2), Main.rand.Next(61, 64), 1f);
            Main.gore[num616].velocity *= 0.4f;
            Gore expr_14419_cp_0 = Main.gore[num616];
            expr_14419_cp_0.velocity.X = expr_14419_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.1f;
            Gore expr_14449_cp_0 = Main.gore[num616];
            expr_14449_cp_0.velocity.Y = expr_14449_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.1f;
            num616 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, default(Vector2), Main.rand.Next(61, 64), 1f);
            Main.gore[num616].velocity *= 0.4f;
            Gore expr_144DD_cp_0 = Main.gore[num616];
            expr_144DD_cp_0.velocity.X = expr_144DD_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.1f;
            Gore expr_1450D_cp_0 = Main.gore[num616];
            expr_1450D_cp_0.velocity.Y = expr_1450D_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.1f;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.penetrate = -1;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 64;
                Projectile.height = 64;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.Damage();
            }
        }

        public override void Kill(int timeleft)
        {
            OnHit();
        }
    }
}