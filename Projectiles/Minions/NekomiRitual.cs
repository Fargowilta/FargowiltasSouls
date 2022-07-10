using FargowiltasSouls.Items.Armor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class NekomiRitual : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        private const float threshold = 150 / 2f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Nekomi Ritual");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.alpha = 255;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
        }

        int oldHeartCount;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.active && !player.dead && !player.ghost && player.GetModPlayer<FargoSoulsPlayer>().NekomiSet)
            {
                Projectile.alpha = 0;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = player.Center;

            Projectile.timeLeft = 2;
            Projectile.scale = (1f - Projectile.alpha / 255f) * 1.5f + (Main.mouseTextColor / 200f - 0.35f) * 0.5f; //throbbing
            Projectile.scale /= 2f;
            if (Projectile.scale < 0.1f)
                Projectile.scale = 0.1f;

            Projectile.localAI[0] = (int)((float)fargoPlayer.NekomiMeter / NekomiHood.MAX_METER * NekomiHood.MAX_HEARTS);

            if (oldHeartCount != Projectile.localAI[0])
            {
                int max = Projectile.localAI[0] > oldHeartCount ? (int)Projectile.localAI[0] : oldHeartCount;
                for (int i = 0; i < max; i++)
                {
                    float rotation = MathHelper.TwoPi / max * i + Projectile.rotation;
                    Vector2 pos = Projectile.Center + new Vector2(0f, -threshold * Projectile.scale).RotatedBy(rotation);
                    FargoSoulsUtil.HeartDust(pos, rotation + MathHelper.PiOver2, spreadModifier: 0.5f, scaleModifier: 0.75f);
                }
                oldHeartCount = (int)Projectile.localAI[0];

                if (Projectile.localAI[0] >= NekomiHood.MAX_HEARTS)
                    SoundEngine.PlaySound(SoundID.Item43, Projectile.Center);
            }

            if (Projectile.localAI[0] >= NekomiHood.MAX_HEARTS)
            {
                Projectile.rotation += MathHelper.Pi / 57f;
                if (Projectile.rotation > MathHelper.TwoPi)
                    Projectile.rotation -= MathHelper.TwoPi;
            }
            else
            {
                Projectile.rotation = 0;
            }
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Projectile.GetAlpha(lightColor);

            for (int i = 0; i < Projectile.localAI[0]; i++)
            {
                float rotation = MathHelper.TwoPi / Projectile.localAI[0] * i + Projectile.rotation;
                Vector2 drawOffset = new Vector2(0f, -threshold * Projectile.scale).RotatedBy(rotation);
                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity * 0.9f;
        }
    }
}