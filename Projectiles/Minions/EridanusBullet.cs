using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EridanusBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eridanus Bullet");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 0;
            projectile.timeLeft = 600;

            projectile.penetrate = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            int ai0 = (int)projectile.ai[0];
            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].CanBeChasedBy())
            {
                if (projectile.localAI[1] < 1f)
                {
                    float rotation = projectile.velocity.ToRotation(); //homing
                    Vector2 vel = Main.npc[ai0].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, projectile.localAI[1]));

                    projectile.localAI[1] += 1f / 300f;
                }
                else
                {
                    projectile.velocity = projectile.DirectionTo(Main.npc[ai0].Center) * projectile.velocity.Length(); //fuck it homing
                }
            }
            else
            {
                if (++projectile.localAI[0] > 6f)
                {
                    projectile.localAI[0] = 1f;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500f);
                    projectile.netUpdate = true;
                }
            }

            if (++projectile.localAI[0] < 300)
            {
                projectile.velocity *= 1.005f;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2);
                Main.PlaySound(SoundID.Item92, projectile.Center);
            }

            projectile.rotation += 0.15f * Math.Sign(projectile.velocity.X);

            if (++projectile.frameCounter > 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                projectile.timeLeft = 0;
                projectile.position = projectile.Center;
                projectile.width = 600;
                projectile.height = 600;
                projectile.Center = projectile.position;
                projectile.penetrate = -1;
                projectile.Damage();
            }

            //if (!Main.dedServ && Main.LocalPlayer.active) Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

            Main.PlaySound(SoundID.NPCKilled, projectile.Center, 6);
            Main.PlaySound(SoundID.Item92, projectile.Center);
            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int index1 = 0; index1 < 50; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 21f * projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 12f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 5f));
                if (Main.rand.NextBool(3))
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= Main.rand.NextFloat(12f, 18f);
                Main.dust[d].position = projectile.Center;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.LightningRod>(), 600);

            if (projectile.timeLeft > 0)
                projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}