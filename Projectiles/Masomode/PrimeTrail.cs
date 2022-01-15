using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PrimeTrail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Trail");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.aiStyle = -1;
            projectile.scale = 0.8f;
        }

        public override void AI()
        {
            bool fade = false;

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0]);
            if (npc != null)
            {
                projectile.Center = npc.Center;
                if (projectile.ai[1] == 0) //swipe limb
                {
                    if (!npc.GetEModeNPCMod<PrimeLimb>().IsSwipeLimb || npc.ai[2] < 140)
                        fade = true;
                }
                else if (projectile.ai[1] == 1)
                {
                    if (npc.GetEModeNPCMod<PrimeLimb>().IsSwipeLimb || (Main.npc[(int)npc.ai[1]].ai[1] != 1 && Main.npc[(int)npc.ai[1]].ai[1] != 2))
                        fade = true;
                }
            }
            else
            {
                fade = true;
            }

            if (fade)
            {
                projectile.alpha += 8;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }
            else
            {
                projectile.alpha -= projectile.ai[1] == 0 ? 16 : 8;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float increment = 0.25f;
            if (projectile.ai[1] == 1f)
                increment = 0.1f;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += increment)
            {
                int max0 = (int)i - 1;
                if (max0 < 0)
                    continue;
                Player player = Main.player[projectile.owner];
                Texture2D glow = Main.projectileTexture[projectile.type];
                Color color27 = (projectile.ai[1] == 0f ? new Color(255, 0, 0, 210) : new Color(191, 51, 255, 210)) * 0.25f * projectile.Opacity;
                if (projectile.ai[1] == 0f)
                    color27 *= 0.5f;
                color27 *= ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;
                scale *= ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], (1 - i % 1));
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