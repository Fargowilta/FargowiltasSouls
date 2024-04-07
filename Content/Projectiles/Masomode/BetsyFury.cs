using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BetsyFury : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_709";

        Vector2 spawn;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sky Dragon's Fury");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.MonkStaffT3_AltShot];
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.hostile = true;
            Projectile.scale = 1.2f;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);
                spawn = Projectile.Center;
            }

            if (Projectile.Distance(spawn) > 1200)
                Projectile.Kill();

            if (++Projectile.localAI[0] < 120)
                Projectile.velocity *= 1.03f;

            Projectile.alpha = Projectile.alpha - 30;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.4f, 0.85f, 0.9f);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, 300);
            target.AddBuff(BuffID.WitheredWeapon, 300);
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override void OnKill(int timeLeft)
        {
            int num1 = 3;
            int num2 = 10;

            for (int index1 = 0; index1 < num1; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, default, 1.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + Projectile.Center;
            }
            for (int index1 = 0; index1 < num2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0.0f, 0.0f, 0, default, 1.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[index2].noGravity = true;
            }

            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<BetsyElectrosphere>(),
                    Projectile.damage, 0f, Main.myPlayer, spawn.X, spawn.Y);
            }

            SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryCircle, Projectile.Center);
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

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}