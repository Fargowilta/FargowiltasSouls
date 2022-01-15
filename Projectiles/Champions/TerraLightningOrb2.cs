using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using System.IO;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TerraLightningOrb2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_465";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 360;
            projectile.penetrate = -1;
            projectile.scale = 0.5f;
            cooldownSlot = 1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= projectile.width / 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.position = projectile.Center;

            projectile.width = (int)(projectile.width / projectile.scale);
            projectile.height = (int)(projectile.height / projectile.scale);

            projectile.scale = reader.ReadSingle();

            projectile.width = (int)(projectile.width * projectile.scale);
            projectile.height = (int)(projectile.height * projectile.scale);

            projectile.Center = projectile.position;
        }
        bool firsttick = false;
        public override void AI()
        {
            projectile.velocity = Vector2.Zero;

            if(!firsttick)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy(2 * (float)Math.PI / 8 * i);
                    Vector2 vel = Vector2.Normalize(dir);
                    Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("TerraLightningOrbDeathray"),
                        projectile.damage, 0, Main.myPlayer, dir.ToRotation(), projectile.whoAmI);
                }
                projectile.rotation = projectile.localAI[0];
                firsttick = true;
            }

            if(projectile.localAI[0] > 0) //rotate fast, then slow down over time
            {
                projectile.rotation += projectile.localAI[1] * (6 - projectile.scale) * 0.012f;
            }

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.TerraChampion>());
            if (npc != null)
            {
                projectile.alpha -= 10;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
                
                projectile.velocity = 4f * projectile.DirectionTo(Main.player[npc.target].Center);

                if (++projectile.ai[1] > 60) //grow
                {
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;

                    projectile.position = projectile.Center;

                    projectile.width = (int)(projectile.width / projectile.scale);
                    projectile.height = (int)(projectile.height / projectile.scale);

                    projectile.scale++;

                    projectile.width = (int)(projectile.width * projectile.scale);
                    projectile.height = (int)(projectile.height * projectile.scale);

                    projectile.Center = projectile.position;

                    MakeDust();
                    Main.PlaySound(SoundID.Item92, projectile.Center);
                }
            }
            else
            {
                if (projectile.timeLeft < 2)
                    projectile.timeLeft = 2;

                projectile.alpha += 10;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }

            Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                    projectile.frame = 0;
            }

            if (Main.rand.NextBool(3))
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-projectile.width * 0.2f * projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = projectile.Center + vector21 * projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-projectile.width * 0.6f * projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = projectile.Center + vector2 * projectile.scale;
                Main.dust[index2].noGravity = true;
            }
        }

        private void MakeDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 80; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 10f * projectile.scale;
                vector6 = vector6.RotatedBy((i - (80 / 2 - 1)) * 6.28318548f / 80) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 92, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item92, projectile.Center);

            MakeDust();

            if (projectile.alpha == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Main.dedServ)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Thunder").WithVolume(0.8f).WithPitchVariance(-0.5f), projectile.Center);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy((2 * (float)Math.PI / 8 * i) + projectile.rotation);
                    float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                    Vector2 vel = Vector2.Normalize(dir) * 54f;
                    Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("HostileLightning"),
                        projectile.damage, 0, Main.myPlayer, dir.ToRotation(), ai1New/2);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}