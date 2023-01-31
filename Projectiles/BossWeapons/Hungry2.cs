using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Hungry2 : Hungry
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Hungry";

        int baseWidth;
        int baseHeight;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hungry");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.minion = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            baseWidth = Projectile.width;
            baseHeight = Projectile.height;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] != 0;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0) //override tungsten
            {
                Projectile.localAI[0] = 1;
                Projectile.scale = 1f;
            }

            //dust!
            int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (Projectile.ai[0] == 0)
            {
                Player player = Main.player[Projectile.owner];
                if (player.active && !player.dead && player.channel && player.HeldItem.type == ModContent.ItemType<Items.Weapons.SwarmDrops.FleshCannon>() && player.CheckMana(player.HeldItem.mana))
                {
                    Projectile.damage = player.GetWeaponDamage(player.HeldItem);
                    Projectile.CritChance = player.GetWeaponCrit(player.HeldItem);
                    Projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                    Projectile.ai[0] = 1;
                    Projectile.penetrate = 1;
                    Projectile.maxPenetrate = 1;
                    Projectile.netUpdate = true;
                    return;
                }

                if (Projectile.scale < 5f)
                {
                    Projectile.scale *= 1.008f;

                    if (Projectile.scale >= 5f) //dust indicates full charge
                    {
                        for (int i = 0; i < 42; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 18f;
                            vector6 = vector6.RotatedBy((i - (36 / 2 - 1)) * 6.28318548f / 42) + Projectile.Center;
                            Vector2 vector7 = vector6 - Projectile.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Torch, 0f, 0f, 0, default, 5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = 5f;
                            Main.dust[d].velocity = vector7;
                        }
                    }
                }

                Projectile.rotation = player.itemRotation;
                if (player.direction < 0)
                    Projectile.rotation += (float)Math.PI;
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();
                float offset = 60f + 40f * Projectile.scale / 5f;
                Projectile.Center = player.Center + offset * player.HeldItem.scale * Vector2.UnitX.RotatedBy(Projectile.rotation);
                Projectile.position -= Projectile.velocity;

                Projectile.timeLeft = 240;
            }
            else
            {
                if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    Projectile.tileCollide = true;
                Projectile.ignoreWater = false;

                const int aislotHomingCooldown = 0;
                const int homingDelay = 5;
                const float desiredFlySpeedInPixelsPerFrame = 60;
                const float amountOfFramesToLerpBy = 10; // minimum of 1, please keep in full numbers even though it's a float!

                Projectile.ai[aislotHomingCooldown]++;
                if (Projectile.ai[aislotHomingCooldown] > homingDelay)
                {
                    Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                    NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 600, true));
                    if (n != null)
                    {
                        Vector2 desiredVelocity = Projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                    }
                    else if (Projectile.timeLeft > 120)
                    {
                        Projectile.timeLeft = 120;
                    }
                }
                /*else
                {
                    Main.NewText(amountOfFramesToLerpBy.ToString());
                    //Main.NewText(Main.player[Projectile.owner].numMinions.ToString() + " " + Main.player[Projectile.owner].maxMinions.ToString() + " " + Main.player[Projectile.owner].slotsMinions.ToString());
                }*/

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(baseWidth * Projectile.scale);
            Projectile.height = (int)(baseHeight * Projectile.scale);
            Projectile.Center = Projectile.position;

            Projectile.rotation += (float)Math.PI / 2;

            Projectile.damage = (int)(Projectile.ai[1] * Projectile.scale);
            if (Projectile.scale < 5f / 2f)
                Projectile.damage /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.scale >= 5)
            {
                damage = (int)(damage * 1.5);
                crit = true;
            }
        }

        public override void Kill(int timeleft)
        {
            if (Projectile.scale >= 5f)
            {
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, Main.rand.NextFloat(3f, 6f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f);
                    Main.dust[d].position = Projectile.Center;
                }
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default(Vector2),
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }
    }
}