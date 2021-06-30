using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class SpiritSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
            projectile.hostile = true;
            projectile.scale = 0.8f;
        }

        public override void AI()
        {
            if (--projectile.ai[1] < 0 && projectile.ai[1] > -300)
            {
                int ai0 = (int)projectile.ai[0];
                if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs
                    && Main.npc[ai0].active && Main.npc[ai0].type == ModContent.NPCType<NPCs.Champions.SpiritChampion>())
                {
                    Player p = Main.player[Main.npc[ai0].target];
                    if (projectile.Distance(p.Center) > 200 && Main.npc[ai0].ai[0] == 3)
                    {
                        for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                        {
                            Vector2 change = projectile.DirectionTo(p.Center) * 2.2f;
                            projectile.velocity = (projectile.velocity * 29f + change) / 30f;
                        }
                    }
                    else //stop homing when in certain range of player, or npc leaves this mode
                    {
                        projectile.ai[1] = -300;
                    }
                }
                else
                {
                    projectile.ai[0] = Player.FindClosest(projectile.Center, 0, 0);
                }
            }
            else if (projectile.ai[1] < -300 && projectile.velocity.Length() < 2.2f)
            {
                projectile.velocity *= 1.022f;
            }

            for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
            {
                projectile.position += projectile.velocity;
                
                /*for (int j = 0; j < 5; ++j)
                {
                    Vector2 vel = projectile.velocity * 0.2f * j;
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 175, 0f, 0f, 100, default, 1.3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = Vector2.Zero;
                    Main.dust[d].position -= vel;
                }*/
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(ModContent.BuffType<Infested>(), 360);
                target.AddBuff(ModContent.BuffType<ClippedWings>(), 180);
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

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.2f)
            {
                Player player = Main.player[projectile.owner];
                Texture2D glow = texture2D13; //mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                Color color27 = color26; //Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                float smoothtrail = i % 1 * (float)Math.PI / 6.85f;

                center += projectile.Size / 2;
                
                Main.spriteBatch.Draw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
                    null,
                    color27,
                    projectile.rotation,
                    glow.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0f);
            }
            return false;
        }
    }
}