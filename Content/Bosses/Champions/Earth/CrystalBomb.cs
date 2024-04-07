using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class CrystalBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            //Projectile.tileCollide = false;
            //Projectile.ignoreWater = true;

            Projectile.alpha = 255;
            Projectile.hide = true;
            CooldownSlot = 1;

            Projectile.scale = 2.5f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Main.rand.NextBool(2) ? 1f : -1f;
                Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2);
                Projectile.hide = false;
            }

            if (--Projectile.localAI[1] < 0)
            {
                Projectile.localAI[1] = 60;
                SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            }

            Projectile.alpha -= 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            Projectile.rotation += (float)Math.PI / 40f * Projectile.localAI[0];

            Lighting.AddLight(Projectile.Center, 0.3f, 0.75f, 0.9f);

            int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.NorthPole, 0.0f, 0.0f, 100, Color.Transparent, 1f);
            Main.dust[index3].noGravity = true;

            Projectile.velocity *= 1.03f;

            if (Projectile.Center.Y > Projectile.ai[0])
                Projectile.tileCollide = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, 0f, 0, default, 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (FargoSoulsUtil.HostCheck)
            {
                for (int index = 0; index < 24; ++index)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Main.rand.NextVector2Circular(12f, 12f),
                        ModContent.ProjectileType<CrystalBombShard>(), Projectile.damage, 0f, Projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}