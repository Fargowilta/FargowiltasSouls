using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class GoldenShowerWOF : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_288";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Golden Shower");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.hostile = true;
            Projectile.extraUpdates = 2;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            }

            /*for (int i = 0; i < 2; i++) //vanilla dusts
            {
                for (int j = 0; j < 2; ++j)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 170, 0.0f, 0.0f, 100, default, 0.75f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].velocity += Projectile.velocity * 0.5f;
                    Main.dust[d].position -= Projectile.velocity / 3 * j;
                }
                if (Main.rand.NextBool(8))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 170, 0.0f, 0.0f, 100, default, 0.325f);
                    Main.dust[d].velocity *= 0.25f;
                    Main.dust[d].velocity += Projectile.velocity * 0.5f;
                }
            }*/

            if (--Projectile.ai[0] < 0)
                Projectile.tileCollide = true;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity.Y += 0.5f;
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 900);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //when hit a tile, linger there until most of the trail catches up with the head
            if (Projectile.position != Projectile.oldPos[(int)(ProjectileID.Sets.TrailCacheLength[Projectile.type] * 0.75)])
            {
                Projectile.localAI[0] = 1;
                Projectile.position += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                return false;
            }

            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.2f)
            {
                //don't draw if this pos overlaps the one before it
                if (i >= 1 && Projectile.oldPos[(int)i] == Projectile.oldPos[(int)i - 1])
                    continue;

                Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpearSpinGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Color color27 = Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                /*bool withinangle = Projectile.rotation > -Math.PI / 2 && Projectile.rotation < Math.PI / 2;
                if (withinangle && player.direction == 1)
                    smoothtrail *= -1;
                else if (!withinangle && player.direction == -1)
                    smoothtrail *= -1;*/

                center += Projectile.Size / 2;

                //Vector2 offset = (Projectile.Size / 4).RotatedBy(Projectile.oldRot[(int)i] - smoothtrail * (-Projectile.direction));
                Main.EntitySpriteDraw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    null,
                    color27,
                    Projectile.rotation,
                    glow.Size() / 2,
                    scale * 0.15f,
                    SpriteEffects.None,
                    0);
            }
            return false;
        }
    }
}