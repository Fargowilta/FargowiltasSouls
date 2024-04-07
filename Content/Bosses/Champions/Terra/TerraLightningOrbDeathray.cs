using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Terra
{
    public class TerraLightningOrbDeathray : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/TerraLightningOrbDeathray";
        public TerraLightningOrbDeathray() : base(1000, 0.8f) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Lightning Deathray");
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            Projectile orb = FargoSoulsUtil.ProjectileExists(Projectile.ai[1], ModContent.ProjectileType<TerraLightningOrb2>());
            if (orb == null)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = orb.Center;
                Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.ai[0] + orb.rotation);
            }
            /*if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
            }*/
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = 0.4f + (float)Math.Sin(Projectile.localAI[0] / 4) * 0.15f;
            float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f + Projectile.ai[0];
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float[] array3 = new float[(int)num805];
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2400f, array3);
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
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.Frostburn, 300);
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
            }
        }
    }
}