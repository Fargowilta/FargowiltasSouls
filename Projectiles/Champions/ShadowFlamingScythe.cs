using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class ShadowFlamingScythe : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_329";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Flaming Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 180;
            
            cooldownSlot = 1;
            projectile.light = 0.25f;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.hide = false;
                projectile.rotation = Main.rand.NextFloat((float)Math.PI / 2);
                projectile.direction = projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }

            if (++projectile.localAI[0] < 160)
            {
                projectile.velocity *= 1.025f;
            }

            if (projectile.ai[0] == 0)
            {
                if (projectile.localAI[0] == 140)
                    projectile.Kill();
            }
            else
            {
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.ShadowChampion>())
                    && Main.npc[EModeGlobalNPC.championBoss].HasValidTarget) //home
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.035f));
                }
            }

            projectile.rotation += projectile.velocity.Length() * 0.015f * Math.Sign(projectile.velocity.X);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = -1; i <= 1; i++)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(MathHelper.ToRadians(45) * i),
                        projectile.type, (int)(projectile.damage / 3.0 * 4.0), 0f, projectile.owner, 1);
                }
            }

            const int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 10f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("Shadowflame"), 300);
                target.AddBuff(BuffID.Blackout, 300);
                target.AddBuff(BuffID.OnFire, 900);
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (projectile.ai[0] != 0)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}