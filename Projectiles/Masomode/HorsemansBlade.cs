using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class HorsemansBlade : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_1826";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Horseman's Blade");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1.15f;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] > 60f)
            {
                Projectile.velocity.X *= 0.97f;
                Projectile.velocity.Y += 0.45f;
            }
            else if (Projectile.ai[1] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int max = 4;
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Normalize(Projectile.velocity).RotatedBy(2 * Math.PI / max * i) * 8f,
                        ModContent.ProjectileType<FlamingJack>(), Projectile.damage, 0f, Main.myPlayer, Projectile.ai[0], 30f);
            }

            Projectile.rotation += Projectile.velocity.Length() / (Projectile.velocity.X > 0 ? 30f : -30f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}