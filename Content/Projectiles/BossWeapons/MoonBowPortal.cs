using FargowiltasSouls.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class MoonBowPortal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_578";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 60;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.position += (player.position - player.oldPosition);

            float maxScale = 0.4f;

            Projectile.ai[0]++;
            Projectile.scale = Math.Min(1f, Projectile.ai[0] / 20) * maxScale;
            Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
            Projectile.rotation = Projectile.rotation - 0.1570796f;

            /*if (Main.rand.NextBool())
            {
                Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.noGravity = true;
                dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                dust.scale = Main.rand.NextFloat();
                dust.fadeIn = 0.5f;
                dust.customData = Projectile.Center;
                dust.velocity += player.velocity;
            }

            if (Main.rand.NextBool())
            {
                Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.noGravity = true;
                dust.position = Projectile.Center - spinningpoint * 30f;
                dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                dust.scale = Main.rand.NextFloat();
                dust.fadeIn = 0.5f;
                dust.customData = Projectile.Center;
                dust.velocity += player.velocity;
            }*/
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].active)
            {
                Vector2 target = Main.MouseWorld;
                if (Main.player[Projectile.owner].HasBuff<MoonBowBuff>())
                    target.Y += 16f; //aim correction for no grav
                Vector2 vel = 32f * Projectile.SafeDirectionTo(target);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel, ProjectileID.MoonlordArrowTrail, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}