using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearBigDeathray : Deathrays.BaseDeathray
    {
        public HentaiSpearBigDeathray() : base(60, "PhantasmalDeathrayML", hitboxModifier: 1.5f) { }

        int dustTimer;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantasmal Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            cooldownSlot = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ranged = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;

            projectile.hide = true;
            projectile.penetrate = -1;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 2;

            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            int byUUID = FargoSoulsUtil.GetByUUIDReal(projectile.owner, (int)projectile.ai[1], ModContent.ProjectileType<HentaiSpearWand>());
            if (byUUID != -1)
            {
                projectile.timeLeft = 2;

                float itemrotate = player.direction < 0 ? MathHelper.Pi : 0;
                if (Math.Abs(player.itemRotation) > Math.PI / 2)
                    itemrotate = itemrotate == 0 ? MathHelper.Pi : 0;
                projectile.velocity = (player.itemRotation + itemrotate).ToRotationVector2();
                projectile.Center = player.Center + Main.rand.NextVector2Circular(5, 5);

                projectile.position += projectile.velocity * 164 * 1.3f / 4f; //offset by part of spear's length (wand)
                projectile.position += projectile.velocity * 164 * 1.3f * 0.75f; //part of penetrator's length (ray)

                projectile.damage = player.GetWeaponDamage(player.HeldItem);
                projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);

                /*projectile.Center = Main.projectile[player.heldProj].Center + Main.rand.NextVector2Circular(5, 5);
                projectile.timeLeft = 2;

                projectile.velocity = Vector2.Normalize(Main.projectile[player.heldProj].velocity);
                projectile.position += projectile.velocity * 164 * 1.3f * 0.75f; //part of penetrator's length

                projectile.damage = Main.projectile[player.heldProj].damage;
                projectile.knockBack = Main.projectile[player.heldProj].knockBack;*/
            }
            else if (projectile.localAI[0] > 5) //leeway for mp lag
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
                if (!Main.dedServ)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Zombie_104"), projectile.Center);
            }
            float num801 = 10f;

            if (projectile.localAI[0] == maxTime / 2)
            {
                if (projectile.owner == Main.myPlayer && !(player.controlUseTile && player.altFunctionUse == 2 && player.HeldItem.type == mod.ItemType("HentaiSpear")))
                    projectile.localAI[0] += 1f; //if stop firing, proceed to die
                else
                    projectile.localAI[0] -= 1f; //otherwise, stay (also for multiplayer!)
            }
            else
            {
                projectile.localAI[0] += 1f;
            }

            if (projectile.localAI[0] >= maxTime)
            {
                projectile.Kill();
                return;
            }
            //projectile.scale = num801;
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * 3.14159274f / maxTime) * 1.5f * num801;
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
            if (Main.rand.NextBool(5))
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

            if (++dustTimer > 30)
            {
                dustTimer = 0;
                const int max = 7;
                for (int i = 0; i < max; i++)
                {
                    const int ring = 128;
                    Vector2 offset = projectile.velocity * 3000 / max * (i - 0.5f);// (i + (float)dustTimer / 60);
                    for (int index1 = 0; index1 < ring; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -projectile.width / 2f + -Vector2.UnitY.RotatedBy(index1 * 3.14159274101257 * 2 / ring)
                            * new Vector2(8f, 16f)).RotatedBy(projectile.rotation - 1.57079637050629);
                        int index2 = Dust.NewDust(projectile.Center, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 3f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = projectile.Center + offset;
                        //Main.dust[index2].velocity = Vector2.Zero;
                        Main.dust[index2].velocity = 5f * Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[index2].position);
                        Main.dust[index2].velocity += 5f * projectile.velocity;
                        Main.dust[index2].velocity += vector2 * 1.5f;
                    }
                }
            }

            if (++projectile.ai[0] > 60)
            {
                projectile.ai[0] = 0;

                Main.PlaySound(SoundID.Item84, player.Center);

                if (projectile.owner == Main.myPlayer)
                {
                    const int ringMax = 10;
                    const float speed = 12f;
                    const float rotation = 0.5f;
                    for (int i = 0; i < ringMax; i++)
                    {
                        Vector2 vel = speed * projectile.velocity.RotatedBy(2 * Math.PI / ringMax * i);
                        Projectile.NewProjectile(player.Center, vel, ModContent.ProjectileType<HentaiSphereRing>(),
                            projectile.damage, projectile.knockBack, projectile.owner, rotation, speed);
                        Projectile.NewProjectile(player.Center, vel, ModContent.ProjectileType<HentaiSphereRing>(),
                            projectile.damage, projectile.knockBack, projectile.owner, -rotation, speed);
                    }
                }
            }

            if (++projectile.frameCounter > 3)
            {
                if (++projectile.frame > 15)
                    projectile.frame = 0;

                switch (projectile.frame)
                {
                    case 1:
                    case 3:
                    case 9:
                    case 11: projectile.frameCounter = 2; break;
                    default: projectile.frameCounter = 0; break;
                }
            }
        }

        public override void PostAI()
        {
            base.PostAI();
            projectile.hide = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 1; //balanceing
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = mod.GetTexture("Projectiles/Deathrays/Mutant/MutantDeathray_" + projectile.frame.ToString());
            Texture2D texture2D20 = mod.GetTexture("Projectiles/Deathrays/Mutant/MutantDeathray2_" + projectile.frame.ToString());
            Texture2D texture2D21 = mod.GetTexture("Projectiles/Deathrays/" + texture + "3");
            float num223 = projectile.localAI[1];
            Color color44 = new Color(255, 255, 255, 0) * 0.95f;
            SpriteBatch arg_ABD8_0 = Main.spriteBatch;
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            arg_ABD8_0.Draw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, color44, projectile.rotation, texture2D19.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * projectile.scale;
            Vector2 value20 = projectile.Center;
            value20 += projectile.velocity * projectile.scale * texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, texture2D20.Width, 30);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }

                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
                    num224 += rectangle7.Height * projectile.scale;
                    value20 += projectile.velocity * rectangle7.Height * projectile.scale;
                    rectangle7.Y += 30;
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
            arg_AE2D_0.Draw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, color44, projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}