using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DeerclopsHand : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_965";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Hand");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.InsanityShadowHostile];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = ProjectileID.Sets.TrailCacheLength[ProjectileID.InsanityShadowHostile];
            ProjectileID.Sets.TrailingMode[Projectile.type] = ProjectileID.Sets.TrailingMode[ProjectileID.InsanityShadowHostile];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.InsanityShadowHostile);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (player != null && player.GetModPlayer<FargoSoulsPlayer>().MashCounter < 20 && Projectile.timeLeft > 60)
                {
                    if (player.active && !player.dead && !player.ghost && Projectile.Colliding(Projectile.Hitbox, player.Hitbox))
                    {
                        player.frozen = true;
                        player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 2);
                        player.AddBuff(ModContent.BuffType<Grabbed>(), 2);
                    }

                    Projectile.velocity = (player.Center - Projectile.Center) / 10f;
                    Projectile.position += player.position - player.oldPosition;

                    Projectile.direction = Projectile.spriteDirection = Math.Sign(player.Center.X - Projectile.Center.X);
                    Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation();

                    const float IdleAccel = 2f;
                    foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == Projectile.type && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
                    {
                        p.position.X += IdleAccel * (p.position.X < Projectile.position.X ? -1 : 1);
                        p.position.Y += IdleAccel * (p.position.Y < Projectile.position.Y ? -1 : 1);
                    }
                }
                else
                {
                    if (Projectile.velocity == Vector2.Zero)
                        Projectile.velocity = Main.rand.NextVector2CircularEdge(1, 1) * 16f;

                    Projectile.velocity *= -1f;

                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);

                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                }

                Projectile.alpha -= 25;
                if (Projectile.alpha < 50)
                    Projectile.alpha = 50;
            }
            else
            {
                Projectile.velocity *= 0.98f;

                Projectile.alpha += 25;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }

                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.spriteDirection < 0)
                Projectile.rotation += MathHelper.Pi;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 900);
            target.AddBuff(ModContent.BuffType<Hypothermia>(), 1200);
        }

        //public override Color? GetAlpha(Color lightColor)
        //{
        //    return Color.White * Projectile.Opacity;
        //}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Color.Black * Projectile.Opacity;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.LightBlue * Projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale * 1.1f, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}