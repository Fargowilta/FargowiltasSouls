using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyDash : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_686";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dash");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = Player.defaultHeight;
            projectile.height = Player.defaultHeight;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.alpha = 60;
            projectile.timeLeft = 15;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead)
            {
                projectile.Kill();
                return;
            }

            if (player.HasBuff(ModContent.BuffType<Buffs.Souls.TimeFrozen>()))
            {
                projectile.Kill();
                return;
            }

            player.GetModPlayer<FargoPlayer>().BetsyDashing = true;
            player.GetModPlayer<FargoPlayer>().dashCD = 5;

            player.Center = projectile.Center;
            if (projectile.timeLeft > 1) //trying to avoid wallclipping
                player.position += projectile.velocity;
            player.velocity = projectile.velocity * .5f;
            player.direction = projectile.velocity.X > 0 ? 1 : -1;

            /*player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;*/
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.controlMount = false;

            if (player.mount.Active)
                player.mount.Dismount(player);

            player.immune = true;
            player.immuneTime = Math.Max(player.immuneTime, 2);
            player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], 2);
            player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], 2);
            
            if (projectile.velocity != Vector2.Zero)
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0, 0, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.BetsysCurse, 600);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0, 0, 0, default, 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //dont kill proj when hits tiles
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.localAI[0] != 0)
            {
                Texture2D texture2D13 = Main.projectileTexture[projectile.type];
                int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;

                Color color26 = Color.White;
                color26 = projectile.GetAlpha(color26);
                color26.A = (byte)projectile.alpha;

                SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    float lerpamount = 0;
                    if (i > 3 && i < 5)
                        lerpamount = 0.6f;
                    if (i >= 5)
                        lerpamount = 0.8f;

                    Color color27 = Color.Lerp(Color.White, Color.Purple, lerpamount) * 0.75f * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    float scale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0f);
                }

                Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            }
            return false;
        }
    }
}