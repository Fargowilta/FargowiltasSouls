using System;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TerraLightningOrbDeathray : Deathrays.BaseDeathray
    {
        public TerraLightningOrbDeathray() : base(1000, "TerraLightningOrbDeathray", 0.8f) { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lightning Deathray");
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            Projectile orb = FargoSoulsUtil.ProjectileExists(projectile.ai[1], ModContent.ProjectileType<TerraLightningOrb2>());
            if (orb == null)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = orb.Center;
                projectile.velocity = Vector2.UnitX.RotatedBy(projectile.ai[0] + orb.rotation);
            }
            /*if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item12, projectile.Center);
            }*/
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= maxTime)
            {
                projectile.Kill();
                return;
            }
            projectile.scale = 0.4f + (float)Math.Sin(projectile.localAI[0]/4) * 0.15f;
            float num804 = projectile.velocity.ToRotation();
            //num804 += projectile.ai[0];
            projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)projectile.ai[1]].ai[3] - 1.57079637f + projectile.ai[0];
            //if (projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //projectile.rotation = num804;
            //num804 += 1.57079637f;
            projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float[] array3 = new float[(int)num805];
            float num806 = (float)projectile.width;
            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);
            Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Frostburn, 300);
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(ModContent.BuffType<Infested>(), 300);
            }
        }
    }
}