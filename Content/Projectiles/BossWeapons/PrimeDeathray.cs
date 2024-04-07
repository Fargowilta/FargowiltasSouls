using FargowiltasSouls.Content.Projectiles.Deathrays;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class PrimeDeathray : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/PhantasmalDeathray";
        public PrimeDeathray() : base(90) { }

        public float Spinup { get { return Projectile.localAI[0] / 2.25f; } }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Blazing Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;

            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            int byIdentity = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.ai[1], ModContent.ProjectileType<RefractorBlaster2Held>());
            if (byIdentity != -1)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.damage = player.GetWeaponDamage(player.HeldItem);
                Projectile.CritChance = player.GetWeaponCrit(player.HeldItem);
                Projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);

                float rotation = player.itemRotation + (player.direction < 0 ? MathHelper.Pi : 0);
                Projectile.velocity = rotation.ToRotationVector2();
                Projectile.Center = player.MountedCenter + 87f * Projectile.velocity;

                Projectile.timeLeft++;
                float rotdir = Projectile.ai[0] > 0 ? 1 : -1;
                Vector2 vel = Projectile.velocity.RotatedBy(rotdir * MathHelper.Pi / 6);
                float windup = Math.Min(1f, Spinup);
                float rotspeed = windup * 1.5f * 6f;

                Projectile.velocity = vel.RotatedBy(Math.Sin(Projectile.localAI[0] * rotspeed + Math.Abs(Projectile.ai[0]) / 6 * MathHelper.TwoPi)
                    * rotdir * MathHelper.Pi / 14 * windup);
            }
            else if (Projectile.localAI[0] > 0.05f) //leeway for mp lag
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            float num801 = .2f;
            Projectile.localAI[0] += 0.01f;
            Projectile.scale = Math.Min(Projectile.localAI[0], num801);
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            //Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2000f, array3);
            //for (int i = 0; i < array3.Length; i++) array3[i] = Projectile.localAI[0] * Projectile.ai[1];
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
            /*Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
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
            }*/
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;

            DelegateMethods.v3_1 = new Vector3(0.8f, 0f, 0);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 10, DelegateMethods.CastLight);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;
            target.AddBuff(BuffID.OnFire, 600);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Math.Min(1f, 0.1f + 0.9f * Spinup);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= Math.Min(1f, 0.1f + 0.9f * Spinup);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(Color.Transparent, new Color(255, 0, 0, 0) * 0.95f, Math.Min(1f, Spinup));
        }
    }
}