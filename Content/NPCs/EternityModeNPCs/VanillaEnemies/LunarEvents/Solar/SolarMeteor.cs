using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Solar
{
    public class SolarMeteor : ModProjectile
    {
        bool spawned = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Meteor");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Cosmos/CosmosMeteor";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Meteor1);
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60 * 30;
            Projectile.extraUpdates = 0;
            Projectile.scale = 1;
            AIType = 0;
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                Projectile.frame = Main.rand.Next(3);
            }

        }
        public override void PostAI()
        {
            Projectile.velocity = (Vector2.UnitX * Projectile.ai[1]) + (Vector2.UnitY * Projectile.ai[2]);
            Projectile.rotation += (float)Math.PI / 15;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
            else
            {
                Projectile.alpha = 0;
            }
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(64 * (double)Projectile.scale);
            Projectile.height = (int)(64 * (double)Projectile.scale);
            Projectile.Center = Projectile.position;

            for (int index = 0; index < 4; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 16; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                Main.dust[index3].noGravity = true;
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[index2];
                gore.velocity *= 0.3f;
                Main.gore[index2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[index2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
            }

            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom(Main.rand, new int[3] { 6, 259, 158 }), 2.5f * Projectile.direction, -2.5f, 0, new Color(), 1f);
                Main.dust[index2].alpha = 200;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 2.4f;
                Dust dust2 = Main.dust[index2];
                dust2.scale += Main.rand.NextFloat();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.BrokenArmor, 300);
            target.AddBuff(BuffID.OnFire, 300);
            Projectile.timeLeft = 0;
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
            {
                Color color27 = Color.HotPink * Projectile.Opacity * 0.5f;
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