using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public class PhantasmalDeathrayML : BaseDeathray
    {
        public PhantasmalDeathrayML() : base(90, "PhantasmalDeathrayML") { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantasmal Deathray II");
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.MoonLordCore, NPCID.MoonLordHand, NPCID.MoonLordHead);
            if (npc == null)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = npc.Center;
            }
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            if (projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Zombie_104"), projectile.Center);
            }
            float num801 = 1f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 20 && projectile.localAI[0] < maxTime - 20)
            {
                //no longer in stardust phase
                bool skip = (npc.type == NPCID.MoonLordCore ? npc.GetEModeNPCMod<MoonLordCore>().GetVulnerabilityState(npc) : npc.GetEModeNPCMod<MoonLordBodyPart>().GetVulnerabilityState(npc)) != 3;
                for (int i = 0; i < Main.maxNPCs; i++) //if any eye firing a deathray
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.MoonLordFreeEye
                        && Main.npc[i].ai[0] == 4 && Main.npc[i].ai[1] > 970)
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                    projectile.localAI[0] = maxTime - 20;
            }
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
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 360);
        }
    }
}