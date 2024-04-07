using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public abstract class HoverShooter : Minion
    {
        protected float ChaseAccel = 6f;
        protected float ChaseDist = 200f;
        protected float IdleAccel = 0.05f;
        protected float Inertia = 40f;
        protected int Shoot;
        protected float ShootCool = 90f;
        protected float ShootSpeed;
        protected float SpacingMult = 1f;
        protected float ViewDist = 400f;

        public virtual void CreateDust()
        {
        }

        public virtual void SelectFrame()
        {
        }

        public override void Behavior()
        {
            Player player = Main.player[Projectile.owner];
            float spacing = Projectile.width * SpacingMult;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                if (k != Projectile.whoAmI && otherProj.active && otherProj.owner == Projectile.owner && otherProj.type == Projectile.type &&
                    Math.Abs(Projectile.position.X - otherProj.position.X) + Math.Abs(Projectile.position.Y - otherProj.position.Y) < spacing)
                {
                    if (Projectile.position.X < Main.projectile[k].position.X)
                        Projectile.velocity.X -= IdleAccel;
                    else
                        Projectile.velocity.X += IdleAccel;
                    if (Projectile.position.Y < Main.projectile[k].position.Y)
                        Projectile.velocity.Y -= IdleAccel;
                    else
                        Projectile.velocity.Y += IdleAccel;
                }
            }

            Vector2 targetPos = Projectile.position;
            float targetDist = ViewDist;
            bool target = false;
            Projectile.tileCollide = true;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                {
                    targetDist = Vector2.Distance(Projectile.Center, targetPos);
                    targetPos = npc.Center;
                    target = true;
                }
            }
            else
            {
                int n = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, targetDist, true);
                if (n != -1)
                {
                    targetDist = Projectile.Distance(Main.npc[n].Center);
                    targetPos = Main.npc[n].Center;
                    target = true;
                }
            }

            if (Vector2.Distance(player.Center, Projectile.Center) > (target ? 1000f : 500f))
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] == 1f) Projectile.tileCollide = false;
            if (target && Projectile.ai[0] == 0f)
            {
                Vector2 direction = targetPos - Projectile.Center;
                if (direction.Length() > ChaseDist)
                {
                    direction.Normalize();
                    Projectile.velocity = (Projectile.velocity * Inertia + direction * ChaseAccel) / (Inertia + 1);
                }
                else
                {
                    Projectile.velocity *= (float)Math.Pow(0.97, 40.0 / Inertia);
                }
            }
            else
            {
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1)) Projectile.ai[0] = 1f;
                float speed = 6f;
                if (Projectile.ai[0] == 1f) speed = 15f;
                Vector2 center = Projectile.Center;
                Vector2 direction = player.Center - center;
                Projectile.ai[1] = 3600f;
                Projectile.netUpdate = true;
                int num = 1;
                for (int k = 0; k < Projectile.whoAmI; k++)
                    if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                        num++;
                direction.X -= (10 + num * 40) * player.direction;
                direction.Y -= 70f;
                float distanceTo = direction.Length();
                if (distanceTo > 200f && speed < 9f) speed = 9f;
                if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }

                if (distanceTo > 2000f) Projectile.Center = player.Center;
                if (distanceTo > 48f)
                {
                    direction.Normalize();
                    direction *= speed;
                    float temp = Inertia / 2f;
                    Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
                }
                else
                {
                    Projectile.direction = Main.player[Projectile.owner].direction;
                    Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / Inertia);
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            SelectFrame();
            CreateDust();
            if (Projectile.velocity.X > 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            else if (Projectile.velocity.X < 0f) Projectile.spriteDirection = Projectile.direction = 1;
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += 1f;
                if (Main.rand.NextBool(3)) Projectile.ai[1] += 1f;
            }

            if (Projectile.ai[1] > ShootCool)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] == 0f)
                if (target)
                {
                    if ((targetPos - Projectile.Center).X > 0f)
                        Projectile.spriteDirection = Projectile.direction = -1;
                    else if ((targetPos - Projectile.Center).X < 0f) Projectile.spriteDirection = Projectile.direction = 1;
                    if (Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] = 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 shootVel = targetPos - Projectile.Center;
                            if (shootVel == Vector2.Zero) shootVel = new Vector2(0f, 1f);
                            shootVel.Normalize();
                            shootVel *= ShootSpeed;
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, Shoot, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 300;
                            Main.projectile[proj].netUpdate = true;
                            Projectile.netUpdate = true;
                        }
                    }
                }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}