using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class ShadowEnchantOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_18";
        int invisTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Orb");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            Projectile.netUpdate = true;

            if (player.whoAmI == Main.myPlayer && (player.dead || !(modPlayer.ShadowEnchantActive || modPlayer.TerrariaSoul) || !player.GetToggleValue("Shadow")))
            {
                modPlayer.ShadowEnchantActive = false;
                Projectile.Kill();
                return;
            }

            // CD
            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]--;

                //dusts indicate its back
                if (Projectile.ai[0] == 0)
                {
                    const int num226 = 18;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                        vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Projectile.Center;
                        Vector2 vector7 = vector6 - Projectile.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Shadowflame, 0f, 0f, 0, default(Color), 2f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].velocity = vector7;
                    }
                }
            }

            float num395 = Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;

            if (Projectile.owner == Main.myPlayer)
            {
                //rotation mumbo jumbo
                float distanceFromPlayer = 250;

                Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.2f);

                Projectile.position = player.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(Projectile.ai[1]);
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
                float rotation = (float)Math.PI / 120;
                Projectile.ai[1] -= rotation;
                if (Projectile.ai[1] > (float)Math.PI)
                {
                    Projectile.ai[1] -= 2f * (float)Math.PI;
                    Projectile.netUpdate = true;
                }
                Projectile.rotation = Projectile.ai[1] + (float)Math.PI / 2f;


                //wait for CD
                if (Projectile.ai[0] != 0f)
                {
                    return;
                }

                //detect being hit
                foreach (Projectile proj in Main.projectile.Where(proj => proj.active && proj.friendly && !proj.hostile && proj.owner == Projectile.owner && proj.damage > 0
                && !FargoSoulsUtil.IsSummonDamage(proj, false) && proj.type != ModContent.ProjectileType<ShadowBall>() && proj.Colliding(proj.Hitbox, Projectile.Hitbox)))
                {
                    int numBalls = 5;
                    int dmg = 25;

                    if (modPlayer.AncientShadowEnchantActive)
                    {
                        numBalls = 10;
                        dmg = 50;
                    }

                    int damage = FargoSoulsUtil.HighestDamageTypeScaling(player, dmg);
                    Projectile[] balls = FargoSoulsUtil.XWay(numBalls, Projectile.GetSource_FromThis(), Projectile.Center, ModContent.ProjectileType<ShadowBall>(), 6, damage, 0);

                    foreach (Projectile ball in balls)
                    {
                        ball.originalDamage = damage;
                    }


                    if (FargoSoulsUtil.CanDeleteProjectile(proj))
                        proj.Kill();

                    Projectile.ai[0] = 300;

                    break;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > 0)
            {
                return false;
            }

            //Redraw the Projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}