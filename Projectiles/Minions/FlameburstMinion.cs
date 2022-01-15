using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class FlameburstMinion : ModProjectile
    {
        Vector2 destination;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Flameburst Minion");
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 44;
            projectile.height = 30;
            projectile.timeLeft = 900;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.GetModPlayer<FargoPlayer>().DarkEnchant || !player.GetToggleValue("DarkArt")))
            {
                projectile.Kill();
                return;
            }

            projectile.netUpdate = true; // Please sync ech

            //pulsation mumbo jumbo
            projectile.position.X = (float)((int)projectile.position.X);
            projectile.position.Y = (float)((int)projectile.position.Y);
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;

            //charging above the player
            if (projectile.ai[0] == 0)
            {
                //float above player
                projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                projectile.position.Y = player.Center.Y - (float)(projectile.height / 2) + player.gfxOffY - 50f;

                //rotate towards and face mouse
                const float rotationModifier = 0.08f;

                if (player.whoAmI == Main.myPlayer)
                {
                    if (Main.MouseWorld.X > projectile.Center.X)
                    {
                        projectile.spriteDirection = 1;

                        projectile.rotation = projectile.rotation.AngleLerp(
                            (new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y) - projectile.Center).ToRotation(), rotationModifier);
                    }
                    else
                    {
                        projectile.spriteDirection = -1;

                        //absolute fuckery so it faces the right direction
                        Vector2 target = new Vector2(Main.MouseWorld.X - (Main.MouseWorld.X - projectile.Center.X) * 2, Main.MouseWorld.Y - (Main.MouseWorld.Y - projectile.Center.Y) * 2) - projectile.Center;

                        projectile.rotation = projectile.rotation.AngleLerp(target.ToRotation(), rotationModifier);
                    }
                }

                //2 seconds
                const float chargeTime = 120;

                if (player.controlUseItem)
                {
                    //charge up while attacking
                    projectile.localAI[0]++;

                    //charge level 1
                    if (projectile.localAI[0] == chargeTime)
                    {
                        if (projectile.owner == Main.myPlayer)
                            projectile.netUpdate = true;

                        double spread = 2 * Math.PI / 36;
                        for (int i = 0; i < 36; i++)
                        {
                            Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                            int index2 = Dust.NewDust(projectile.Center, 0, 0, DustID.FlameBurst, velocity.X, velocity.Y, 100);
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].noLight = true;
                        }
                    }
                    //charging further
                    if (projectile.localAI[0] > chargeTime)
                    {
                        int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.FlameBurst, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f);
                        Main.dust[d].noGravity = true;
                    }
                }
                else
                {
                    //let go and fire
                    if (projectile.localAI[0] > chargeTime)
                    {
                        if (projectile.owner == Main.myPlayer)
                            projectile.netUpdate = true;

                        Vector2 mouse = Main.MouseWorld;
                        destination = mouse;

                        //switch to travel mode
                        projectile.ai[0] = 1;
                        projectile.localAI[0] = 0;

                        player.GetModPlayer<FargoPlayer>().DarkSpawn = true;
                        //player.GetModPlayer<FargoPlayer>().DarkSpawnCD = 5;
                    }
                }
            }
            else
            {
                //travelling to destination
                if (Vector2.Distance(projectile.Center, destination) > 10 && projectile.localAI[0] == 0)
                {
                    Vector2 velocity = Vector2.Normalize(destination - projectile.Center) * 10;
                    projectile.velocity = velocity;

                    //dust
                    int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, DustID.FlameBurst, projectile.velocity.X * 0.2f,
                        projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Main.dust[dustId].noGravity = true;
                    int dustId3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, DustID.FlameBurst, projectile.velocity.X * 0.2f,
                        projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Main.dust[dustId3].noGravity = true;
                }
                //attack as a sentry
                else
                {
                    projectile.localAI[0] = 1;
                    projectile.velocity = Vector2.Zero;

                    int attackRate = 20;
                    projectile.ai[1] += 1f;

                    if (projectile.ai[1] >= attackRate)
                    {
                        float num = 2000f;
                        int npcIndex = -1;
                        for (int i = 0; i < 200; i++)
                        {
                            float dist = Vector2.Distance(projectile.Center, Main.npc[i].Center);

                            if (dist < num && dist < 600 && Main.npc[i].CanBeChasedBy(projectile, false))
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

                                int p = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<MegaFlameburst>(), player.GetModPlayer<FargoPlayer>().HighestDamageTypeScaling(85), 4, projectile.owner, projectile.whoAmI);
                                Main.PlayTrackedSound(SoundID.DD2_FlameburstTowerShot, projectile.Center);

                                const float rotationModifier = 0.08f;

                                for (int i = 0; i < 20; i++)
                                {
                                    if (target.Center.X > projectile.Center.X)
                                    {
                                        projectile.spriteDirection = 1;

                                        projectile.rotation = projectile.rotation.AngleLerp(
                                        (new Vector2(target.Center.X, target.Center.Y) - projectile.Center).ToRotation(), rotationModifier);
                                    }
                                    else
                                    {
                                        projectile.spriteDirection = -1;

                                        //absolute fuckery so it faces the right direction
                                        Vector2 rotation = new Vector2(target.Center.X - (target.Center.X - projectile.Center.X) * 2, target.Center.Y - (target.Center.Y - projectile.Center.Y) * 2) - projectile.Center;

                                        projectile.rotation = projectile.rotation.AngleLerp(rotation.ToRotation(), rotationModifier);
                                    }
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
            }
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
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
    }
}