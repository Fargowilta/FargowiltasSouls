using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class FlameburstMinion : ModProjectile
    {
        Vector2 destination;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flameburst Minion");
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 44;
            Projectile.height = 30;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.GetModPlayer<FargoSoulsPlayer>().DarkArtistEnchantActive || !player.GetToggleValue("DarkArt")))
            {
                Projectile.Kill();
                return;
            }

            Projectile.netUpdate = true; // Please sync ech

            //pulsation mumbo jumbo
            Projectile.position.X = (float)((int)Projectile.position.X);
            Projectile.position.Y = (float)((int)Projectile.position.Y);
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;

            //charging above the player
            if (Projectile.ai[0] == 0)
            {
                //float above player
                Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2) + player.gfxOffY - 50f;

                //rotate towards and face mouse
                const float rotationModifier = 0.08f;

                if (player.whoAmI == Main.myPlayer)
                {
                    if (Main.MouseWorld.X > Projectile.Center.X)
                    {
                        Projectile.spriteDirection = 1;

                        Projectile.rotation = Projectile.rotation.AngleLerp(
                            (new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y) - Projectile.Center).ToRotation(), rotationModifier);
                    }
                    else
                    {
                        Projectile.spriteDirection = -1;

                        //absolute fuckery so it faces the right direction
                        Vector2 target = new Vector2(Main.MouseWorld.X - (Main.MouseWorld.X - Projectile.Center.X) * 2, Main.MouseWorld.Y - (Main.MouseWorld.Y - Projectile.Center.Y) * 2) - Projectile.Center;

                        Projectile.rotation = Projectile.rotation.AngleLerp(target.ToRotation(), rotationModifier);
                    }
                }

                //2 seconds
                const float chargeTime = 120;

                if (player.controlUseItem)
                {
                    //charge up while attacking
                    Projectile.localAI[0]++;

                    //charge level 1
                    if (Projectile.localAI[0] == chargeTime)
                    {
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.netUpdate = true;

                        double spread = 2 * Math.PI / 36;
                        for (int i = 0; i < 36; i++)
                        {
                            Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                            int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.FlameBurst, velocity.X, velocity.Y, 100);
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].noLight = true;
                        }
                    }
                    //charging further
                    if (Projectile.localAI[0] > chargeTime)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f);
                        Main.dust[d].noGravity = true;
                    }
                }
                else
                {
                    //let go and fire
                    if (Projectile.localAI[0] > chargeTime)
                    {
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.netUpdate = true;

                        Vector2 mouse = Main.MouseWorld;
                        destination = mouse;

                        //switch to travel mode
                        Projectile.ai[0] = 1;
                        Projectile.localAI[0] = 0;

                        player.GetModPlayer<FargoSoulsPlayer>().DarkArtistSpawn = true;
                        //player.GetModPlayer<FargoSoulsPlayer>().DarkSpawnCD = 5;
                    }
                }
            }
            else
            {
                //travelling to destination
                if (Vector2.Distance(Projectile.Center, destination) > 10 && Projectile.localAI[0] == 0)
                {
                    Vector2 velocity = Vector2.Normalize(destination - Projectile.Center) * 10;
                    Projectile.velocity = velocity;

                    //dust
                    int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.FlameBurst, Projectile.velocity.X * 0.2f,
                        Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Main.dust[dustId].noGravity = true;
                    int dustId3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.FlameBurst, Projectile.velocity.X * 0.2f,
                        Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Main.dust[dustId3].noGravity = true;
                }
                //attack as a sentry
                else
                {
                    Projectile.localAI[0] = 1;
                    Projectile.velocity = Vector2.Zero;

                    int attackRate = 20;
                    Projectile.ai[1] += 1f;

                    if (Projectile.ai[1] >= attackRate)
                    {
                        float num = 2000f;
                        int npcIndex = -1;
                        for (int i = 0; i < 200; i++)
                        {
                            float dist = Vector2.Distance(Projectile.Center, Main.npc[i].Center);

                            if (dist < num && dist < 600 && Main.npc[i].CanBeChasedBy(Projectile, false))
                            {
                                npcIndex = i;
                                num = dist;
                            }
                        }

                        if (npcIndex != -1)
                        {
                            NPC target = Main.npc[npcIndex];

                            if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                            {
                                Vector2 velocity = Vector2.Normalize(target.Center - Projectile.Center) * 10;

                                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MegaFlameburst>(), FargoSoulsUtil.HighestDamageTypeScaling(player, 85), 4, Projectile.owner, Projectile.whoAmI);
                                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Projectile.Center);

                                const float rotationModifier = 0.08f;

                                for (int i = 0; i < 20; i++)
                                {
                                    if (target.Center.X > Projectile.Center.X)
                                    {
                                        Projectile.spriteDirection = 1;

                                        Projectile.rotation = Projectile.rotation.AngleLerp(
                                        (new Vector2(target.Center.X, target.Center.Y) - Projectile.Center).ToRotation(), rotationModifier);
                                    }
                                    else
                                    {
                                        Projectile.spriteDirection = -1;

                                        //absolute fuckery so it faces the right direction
                                        Vector2 rotation = new Vector2(target.Center.X - (target.Center.X - Projectile.Center.X) * 2, target.Center.Y - (target.Center.Y - Projectile.Center.Y) * 2) - Projectile.Center;

                                        Projectile.rotation = Projectile.rotation.AngleLerp(rotation.ToRotation(), rotationModifier);
                                    }
                                }
                            }
                        }
                        Projectile.ai[1] = 0f;

                        //kill if too far away
                        if (Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center) > 2000)
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            const int num226 = 12;
            for (int i = 0; i < num226; i++)
            {
                Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                vector6 = vector6.RotatedBy(((i - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.FlameBurst, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}