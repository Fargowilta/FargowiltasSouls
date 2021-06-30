using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingLoveBig : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/SparklingLove";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 90;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 65;
            projectile.aiStyle = -1;
            projectile.scale = 4f;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            //the important part
            int ai1 = (int)projectile.ai[1];
            if (ai1 > -1 && ai1 < Main.maxProjectiles && Main.projectile[ai1].active && Main.projectile[ai1].type == ModContent.ProjectileType<SparklingDevi>())
            {
                if (projectile.timeLeft > 15)
                {
                    Vector2 offset = new Vector2(0, -275).RotatedBy(Math.PI / 4 * Main.projectile[ai1].spriteDirection);
                    projectile.Center = Main.projectile[ai1].Center + offset;
                    projectile.rotation = (float)Math.PI / 4 * Main.projectile[ai1].spriteDirection - (float)Math.PI / 4;
                }
                else //swinging down
                {
                    if (projectile.timeLeft == 15) //confirm facing the right direction with right offset
                        projectile.rotation = (float)Math.PI / 4 * Main.projectile[ai1].spriteDirection - (float)Math.PI / 4;

                    projectile.rotation -= (float)Math.PI / 15 * Main.projectile[ai1].spriteDirection * 0.75f;
                    Vector2 offset = new Vector2(0, -275).RotatedBy(projectile.rotation + (float)Math.PI / 4);
                    projectile.Center = Main.projectile[ai1].Center + offset;
                }

                projectile.spriteDirection = -Main.projectile[ai1].spriteDirection;

                projectile.localAI[1] = Main.projectile[ai1].velocity.ToRotation();

                if (projectile.localAI[0] == 0)
                {
                    projectile.localAI[0] = 1;
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -14);
                    Main.PlaySound(SoundID.Item92, projectile.Center);

                    MakeDust();
                }
            }
            else
            {
                projectile.Kill();
                return;
            }
        }

        private void MakeDust()
        {
            const int scaleCounter = 3;

            Vector2 start = projectile.width * Vector2.UnitX.RotatedBy(projectile.rotation - (float)Math.PI / 4);
            if (Math.Abs(start.X) > projectile.width / 2) //bound it so its always inside projectile's hitbox
                start.X = projectile.width / 2 * Math.Sign(start.X);
            if (Math.Abs(start.Y) > projectile.height / 2)
                start.Y = projectile.height / 2 * Math.Sign(start.Y);
            int length = (int)start.Length();
            start = Vector2.Normalize(start);
            float scaleModifier = scaleCounter / 3f + 0.5f;
            for (int j = -length; j <= length; j += 80)
            {
                Vector2 dustPoint = projectile.Center + start * j;
                dustPoint.X -= 23;
                dustPoint.Y -= 23;

                /*for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2f);
                    Main.dust[dust].velocity *= 1.4f * scaleModifier;
                }*/

                for (int index1 = 0; index1 < 15; ++index1)
                {
                    int index2 = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2.5f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 16f * scaleModifier;
                    int index3 = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier);
                    Main.dust[index3].velocity *= 8f * scaleModifier;
                    Main.dust[index3].noGravity = true;
                }

                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), Main.rand.NextFloat(1f, 2f) * scaleModifier);
                    Main.dust[d].velocity *= Main.rand.NextFloat(1f, 4f) * scaleModifier;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[projectile.owner] = 1;
            target.AddBuff(BuffID.Lovestruck, 300);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.timeLeft < 15)
            {
                crit = true;
            }
        }

        public override void Kill(int timeleft)
        {
            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

            MakeDust();

            Main.PlaySound(SoundID.NPCKilled, projectile.Center, 6);
            Main.PlaySound(SoundID.Item92, projectile.Center);

            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -14);

            if (projectile.owner == Main.myPlayer)
            {
                /*float minionSlotsUsed = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && !Main.projectile[i].hostile && Main.projectile[i].owner == projectile.owner && Main.projectile[i].minion)
                        minionSlotsUsed += Main.projectile[i].minionSlots;
                }*/

                float modifier = Main.player[projectile.owner].maxMinions - Main.player[projectile.owner].slotsMinions;
                if (modifier < 0)
                    modifier = 0;
                if (modifier > 12)
                    modifier = 12;

                int max = (int)modifier + 4;
                for (int i = 0; i < max; i++)
                {
                    Vector2 target = 600 * -Vector2.UnitY.RotatedBy(2 * Math.PI / max * i + projectile.localAI[1]);
                    Vector2 speed = 2 * target / 90;
                    float acceleration = -speed.Length() / 90;
                    float rotation = speed.ToRotation() + (float)Math.PI / 2;
                    Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<SparklingLoveEnergyHeart>(),
                        projectile.damage, projectile.knockBack, projectile.owner, rotation, acceleration);

                    Projectile.NewProjectile(projectile.Center, 14f * Vector2.UnitY.RotatedBy(2 * Math.PI / max * (i + 0.5) + projectile.localAI[1]),
                        ModContent.ProjectileType<SparklingLoveHeart2>(), projectile.damage, projectile.knockBack,
                        projectile.owner, -1, 45);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            float rotationOffset = projectile.spriteDirection > 0 ? 0 : (float)Math.PI / 2;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165 + rotationOffset, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation + rotationOffset, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}