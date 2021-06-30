using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PrimeDeathray : Deathrays.BaseDeathray
    {
        public PrimeDeathray() : base(90, "PhantasmalDeathray") { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            cooldownSlot = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
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
            Player player = Main.player[projectile.owner];
            //if (player.active && !player.dead && player.heldProj > -1 && player.heldProj < Main.maxProjectiles && Main.projectile[player.heldProj].active && Main.projectile[player.heldProj].type == ModContent.ProjectileType<RefractorBlaster2Held>())
            if (player.active && !player.dead
                && player.HeldItem.type == ModContent.ItemType<Items.Weapons.SwarmDrops.RefractorBlaster2>()
                && player.ownedProjectileCounts[ModContent.ProjectileType<RefractorBlaster2Held>()] > 0)
            {
                /*projectile.damage = Main.projectile[player.heldProj].damage;
                projectile.knockBack = Main.projectile[player.heldProj].knockBack;

                Vector2 Offset = new Vector2(Main.projectile[player.heldProj].width * 0.4f, 0).RotatedBy(Main.projectile[player.heldProj].velocity.ToRotation());
                projectile.Center = Main.projectile[player.heldProj].Center + Offset;*/
                
                projectile.damage = Main.player[projectile.owner].GetWeaponDamage(Main.player[projectile.owner].HeldItem);
                projectile.knockBack = Main.player[projectile.owner].GetWeaponKnockback(Main.player[projectile.owner].HeldItem, Main.player[projectile.owner].HeldItem.knockBack);

                float rotation = player.itemRotation + (player.direction < 0 ? MathHelper.Pi : 0);
                projectile.velocity = rotation.ToRotationVector2();
                projectile.Center = player.MountedCenter + 87f * projectile.velocity;

                projectile.timeLeft++;
                float rotdir = (projectile.ai[0] > 0) ? 1 : -1;
                Vector2 vel = projectile.velocity.RotatedBy(rotdir * MathHelper.Pi/6);
                float windup = Math.Min(1f, projectile.localAI[0] / 2.25f);
                float rotspeed = windup * 1.5f * 6f;
                
                projectile.velocity = vel.RotatedBy(Math.Sin(projectile.localAI[0] * rotspeed + (Math.Abs(projectile.ai[0]) / 6 * MathHelper.TwoPi))
                    * rotdir * MathHelper.Pi / 14 * windup);
            }
            else if (projectile.localAI[0] > 0.05f) //leeway for mp lag
            {
                projectile.Kill();
                return;
            }
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            float num801 = .2f;
            projectile.localAI[0] += 0.01f;
            projectile.scale = Math.Min(projectile.localAI[0], num801);
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
            Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2000f, array3);
            //for (int i = 0; i < array3.Length; i++) array3[i] = projectile.localAI[0] * projectile.ai[1];
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

            DelegateMethods.v3_1 = new Vector3(0.8f, 0f, 0f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (projectile.localAI[1]), 10, DelegateMethods.CastLight);
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            target.AddBuff(BuffID.OnFire, 600);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * Math.Min(1f, 0.1f + 0.9f * projectile.localAI[0] / 2.25f));
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = (int)(damage * Math.Min(1f, 0.1f + 0.9f * projectile.localAI[0] / 2.25f));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 0, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = Main.projectileTexture[projectile.type];
            Texture2D texture2D20 = mod.GetTexture("Projectiles/Deathrays/" + texture + "2");
            Texture2D texture2D21 = mod.GetTexture("Projectiles/Deathrays/" + texture + "3");
            float num223 = projectile.localAI[1];
            Microsoft.Xna.Framework.Color color44 = new Microsoft.Xna.Framework.Color(255, 0, 0, 0);
            color44 = Color.Lerp(Color.Transparent, color44, Math.Min(1f, projectile.localAI[0] / 2.25f));
            SpriteBatch arg_ABD8_0 = Main.spriteBatch;
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = projectile.Center - Main.screenPosition;
            Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = null;
            arg_ABD8_0.Draw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, color44, projectile.rotation, texture2D19.Size() / 2f, projectile.scale, SpriteEffects.None, 1f);
            num223 -= (float)(texture2D19.Height / 2 + texture2D21.Height) * projectile.scale;
            Vector2 value20 = projectile.Center;
            value20 += projectile.velocity * projectile.scale * (float)texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Microsoft.Xna.Framework.Rectangle rectangle7 = new Microsoft.Xna.Framework.Rectangle(0, 16 * (projectile.timeLeft / 3 % 5), texture2D20.Width, 16);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < (float)rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, projectile.rotation, new Vector2((float)(rectangle7.Width / 2), 0f), projectile.scale, SpriteEffects.None, 1f);
                    num224 += (float)rectangle7.Height * projectile.scale;
                    value20 += projectile.velocity * (float)rectangle7.Height * projectile.scale;
                    rectangle7.Y += 16;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            SpriteBatch arg_AE2D_0 = Main.spriteBatch;
            Texture2D arg_AE2D_1 = texture2D21;
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            arg_AE2D_0.Draw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, color44, projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 1f);
            return false;
        }
    }
}