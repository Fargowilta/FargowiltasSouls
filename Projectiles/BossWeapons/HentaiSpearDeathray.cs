﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearDeathray : Deathrays.BaseDeathray
    {
        public HentaiSpearDeathray() : base(90, "PhantasmalDeathrayML") { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            cooldownSlot = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;

            projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            /*if (Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == mod.NPCType("MutantBoss"))
            {
                projectile.Center = Main.npc[(int)projectile.ai[1]].Center;
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
                //Main.PlaySound(SoundID.NPCKilled, (int)projectile.Center.X, (int)projectile.Center.Y, 6, 1f, 0.0f);
            }
            float num801 = .5f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= maxTime)
            {
                projectile.Kill();
                return;
            }
            //projectile.scale = num801;
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * 3.14159274f / maxTime) * 2.5f * num801;
            if (projectile.scale > num801)
            {
                projectile.scale = num801;
            }
            //float num804 = projectile.velocity.ToRotation();
            //num804 += projectile.ai[0];
            //projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)projectile.ai[1]].ai[3] - 1.57079637f;
            //if (projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //projectile.rotation = num804;
            //num804 += 1.57079637f;
            //projectile.velocity = num804.ToRotationVector2();
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
                array3[i] = projectile.localAI[0] * projectile.ai[1];
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
            /*Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
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
            if (Main.rand.Next(5) == 0)
            {
                Vector2 value29 = projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }*/
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            projectile.position -= projectile.velocity;
            projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;

            const int increment = 100;
            for (int i = 0; i < array3[0]; i += increment)
            {
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                int d = Dust.NewDust(projectile.position + projectile.velocity * offset,
                    projectile.width, projectile.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4.5f;
            }
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
            target.immune[projectile.owner] = 1;
        }
    }
}