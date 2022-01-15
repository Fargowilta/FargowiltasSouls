﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public class DeviBigDeathray : BaseDeathray
    {
        public DeviBigDeathray() : base(180, "DeviDeathray") { }

        public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Love Ray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 2;

            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc != null)
            {
                projectile.Center = npc.Center + projectile.velocity * 300 + Main.rand.NextVector2Circular(20, 20);
            }
            else
            {
                projectile.Kill();
                return;
            }
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
            }
            float num801 = 17f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= maxTime)
            {
                projectile.Kill();
                return;
            }
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * 3.14159274f / maxTime) * 5f * num801;
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
            Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            const int increment = 100;
            for (int i = 0; i < array3[0]; i += increment)
            {
                if (Main.rand.Next(3) != 0)
                    continue;
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                int d = Dust.NewDust(projectile.position + projectile.velocity * offset,
                    projectile.width, projectile.height, 86, 0f, 0f, 0, default, Main.rand.NextFloat(4f, 8f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity += projectile.velocity * 0.5f;
                Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 2); //lots of defense down stack to make damage calc consistent
            target.AddBuff(BuffID.WitheredArmor, 2);
            //target.AddBuff(mod.BuffType("Rotting"), 2);
            //target.AddBuff(mod.BuffType("MutantNibble"), 2);
            target.AddBuff(mod.BuffType("Stunned"), 2);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 2);
            target.AddBuff(mod.BuffType("Lovestruck"), 360);
            target.AddBuff(mod.BuffType("Defenseless"), 1800);
            
            target.velocity.X = 0;
            target.velocity.Y = -0.4f;
        }
    }
}