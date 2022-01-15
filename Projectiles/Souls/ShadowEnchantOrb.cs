using System;
using System.Linq;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class ShadowEnchantOrb : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_18";
        int invisTimer = 0;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Shadow Orb");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            Main.projPet[projectile.type] = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            projectile.netUpdate = true;

            if (player.whoAmI == Main.myPlayer && (player.dead || !(modPlayer.ShadowEnchant || modPlayer.TerrariaSoul) || !player.GetToggleValue("Shadow")))
            {
                modPlayer.ShadowEnchant = false;
                projectile.Kill();
                return;
            }

            // CD
            if (projectile.ai[0] > 0)
            {
                projectile.ai[0]--;

                //dusts indicate its back
                if (projectile.ai[0] == 0)
                {
                    const int num226 = 18;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.UnitX.RotatedBy(projectile.rotation) * 6f;
                        vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                        Vector2 vector7 = vector6 - projectile.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Shadowflame, 0f, 0f, 0, default(Color), 2f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].velocity = vector7;
                    }
                }
            }

            float num395 = Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;

            if (projectile.owner == Main.myPlayer)
            {
                //rotation mumbo jumbo
                float distanceFromPlayer = 250;

                Lighting.AddLight(projectile.Center, 0.1f, 0.4f, 0.2f);

                projectile.position = player.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(projectile.ai[1]);
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;
                float rotation = (float)Math.PI / 120;
                projectile.ai[1] -= rotation;
                if (projectile.ai[1] > (float)Math.PI)
                {
                    projectile.ai[1] -= 2f * (float)Math.PI;
                    projectile.netUpdate = true;
                }
                projectile.rotation = projectile.ai[1] + (float)Math.PI / 2f;


                //wait for CD
                if (projectile.ai[0] != 0f)
                {
                    return;
                }

                //detect being hit
                foreach (Projectile proj in Main.projectile.Where(proj => proj.active && proj.friendly && !proj.hostile && proj.owner == projectile.owner && proj.damage > 0
                && !FargoSoulsUtil.IsMinionDamage(proj, false) && proj.type != ModContent.ProjectileType<ShadowBall>() && proj.Colliding(proj.Hitbox, projectile.Hitbox)))
                {
                    int numBalls = 5;
                    int dmg = 25;

                    if (modPlayer.AncientShadowEnchant)
                    {
                        numBalls = 10;
                        dmg = 50;
                    }

                    FargoSoulsUtil.XWay(numBalls, projectile.Center, ModContent.ProjectileType<ShadowBall>(), 6, modPlayer.HighestDamageTypeScaling(dmg), 0);

                    if (FargoSoulsUtil.CanDeleteProjectile(proj))
                        proj.Kill();

                    projectile.ai[0] = 300;

                    break;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] > 0)
            {
                return false;
            }

            //Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}