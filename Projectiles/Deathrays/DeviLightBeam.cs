using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public class DeviLightBeam : BaseDeathray
    {
        public DeviLightBeam() : base(30 * 2, "AbomDeathray") { }

        public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Light Ray");
		}

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.extraUpdates = 1;
        }

        public override bool CanDamage()
        {
            return projectile.localAI[0] > 6;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            /*int ai1 = (int)projectile.ai[1];
            if (Main.npc[ai1].active && Main.npc[ai1].type == mod.NPCType("DeviBoss"))
            {
                projectile.Center = Main.npc[(int)projectile.ai[1]].Center + projectile.velocity * 250 + Main.rand.NextVector2Circular(5, 5);
            }
            else
            {
                projectile.Kill();
                return;
            }*/
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item12, projectile.Center);
                //Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
            }
            float num801 = 0.3f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= maxTime)
            {
                projectile.Kill();
                return;
            }
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * 3.14159274f / maxTime) * 3f * num801;
            if (projectile.scale > num801)
                projectile.scale = num801;
            float num804 = projectile.velocity.ToRotation();
            num804 += projectile.ai[0];
            projectile.rotation = num804 - 1.57079637f;
            projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)projectile.width;
            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
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
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            projectile.position -= projectile.velocity;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Purified"), 300);
        }
    }
}