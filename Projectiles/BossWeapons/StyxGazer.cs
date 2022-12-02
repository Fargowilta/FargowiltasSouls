using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class StyxGazer : Deathrays.AbomSpecialDeathray
    {
        public StyxGazer() : base(120) { }
        public int counter;
        public bool spawnedHandle;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Styx Gazer");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            base.AI();

            Projectile.maxPenetrate = 1;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Zombie_104") { Volume = 0.6f }, Projectile.Center);
            }
            float num801 = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * num801 * 6f;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 1500f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            if (--counter < 0)
            {
                counter = 8;
                if (Projectile.owner == Main.myPlayer) //spawn bonus projs
                {
                    Vector2 spawnPos = Projectile.Center;
                    Vector2 vel = Projectile.velocity.RotatedBy(Math.PI / 2 * Math.Sign(Projectile.ai[0]));
                    const int max = 8;
                    for (int i = 1; i <= max; i++)
                    {
                        spawnPos += Projectile.velocity * 1500f / max;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<StyxSickle>(), Projectile.damage, Projectile.knockBack / 10, Projectile.owner);
                    }
                }
            }

            if (!spawnedHandle)
            {
                spawnedHandle = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StyxGazerHandle>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, (float)Math.PI / 2, Projectile.identity);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StyxGazerHandle>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, -(float)Math.PI / 2, Projectile.identity);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position + Projectile.velocity * Main.rand.NextFloat(1500), Projectile.width, Projectile.height, 87, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }

            int direction = Projectile.velocity.X < 0 ? -1 : 1;
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Main.player[Projectile.owner].itemTime = 17;
            Main.player[Projectile.owner].itemAnimation = 17;
            Main.player[Projectile.owner].ChangeDir(direction);
            Main.player[Projectile.owner].itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage()
        {
            Projectile.maxPenetrate = 1;
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.localNPCImmunity[target.whoAmI] >= 15)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.localNPCImmunity[target.whoAmI]++;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, ModContent.ProjectileType<AbomBoss.AbomBlast>(), 0, 0f, Projectile.owner);

            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
        }
    }
}