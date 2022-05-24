using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public class DeerclopsDeathray : BaseDeathray
    {
        public DeerclopsDeathray() : base(300, "PhantasmalDeathrayWOF") { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ice Beam");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.MaxUpdates = 5;
            if (FargoSoulsWorld.MasochistModeReal)
                Projectile.MaxUpdates *= 2;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            maxTime = (int)Projectile.ai[1];

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.DeerclopsScream, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, Projectile.Center);
            }
            float num801 = 0.25f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 5f * num801;
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
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            //for (int i = 0; i < array3.Length; i++)
            //    array3[i] = 3000f;
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

            //dont make dust when firing in the 90 degree cone downwards
            if (Math.Abs(MathHelper.WrapAngle(MathHelper.PiOver2 - Projectile.velocity.ToRotation())) > MathHelper.PiOver4)
            {
                Vector2 beamEnd = Projectile.Center + Projectile.velocity * Projectile.localAI[1];
                for (int i = 0; i < 2; i = num3 + 1)
                {
                    int num812 = Dust.NewDust(beamEnd, 0, 0, DustID.GemSapphire);
                    Main.dust[num812].scale = 1f;
                    Main.dust[num812].velocity *= 3;
                    Main.dust[num812].velocity.Y -= 6f;
                    num3 = i;
                }
            }

            Projectile.position -= Projectile.velocity;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frozen, 30);

            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
            target.AddBuff(ModContent.BuffType<Hypothermia>(), 1200);
        }
    }
}