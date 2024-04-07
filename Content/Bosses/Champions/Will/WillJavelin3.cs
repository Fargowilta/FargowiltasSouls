using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Will
{
    public class WillJavelin3 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_508";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Javelin");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 360;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.alpha = 255;
            Projectile.scale = 1.5f;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                Projectile.rotation = Projectile.ai[1] + (float)Math.PI / 2;
                Projectile.hide = false;
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation += (float)Math.PI * 5 / 51 * Projectile.localAI[0];

                Projectile.alpha -= 6;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                if (++Projectile.localAI[1] >= 51)
                {
                    Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.ai[1]) * 30f;
                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
                target.AddBuff(ModContent.BuffType<MidasBuff>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
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

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}