using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosInvader : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_539";

        protected bool spawned;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Invader");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override bool PreAI()
        {
            if (!spawned)
            {
                spawned = true;
                Projectile.frame = Main.rand.Next(4);

                Projectile.timeLeft = (int)Projectile.ai[0];
            }
            return true;
        }

        public override void AI()
        {
            Projectile.velocity *= 1 + Projectile.ai[1];

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            Projectile.localAI[0]++;
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 80;
            Projectile.Center = Projectile.position;
            SoundEngine.PlaySound(SoundID.Item25 with { Volume = 0.5f, Pitch = 0 }, Projectile.Center);
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_Blue, 0.0f, 0.0f, 200, new Color(), 3.7f);
                Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity *= 3f;
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0.0f, 0.0f, 0, new Color(), 2.7f);
                Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity *= 3f;
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 0, new Color(), 1.5f);
                Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity *= 3f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Obstructed, 20);
            target.AddBuff(BuffID.Blackout, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantEye_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle glowrectangle = glow.Bounds;
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(29, 171, 239, 0), Color.Transparent, 0.3f);

            float transparency = (Projectile.localAI[0] - FargoSoulsGlobalProjectile.TimeFreezeMoveDuration) / FargoSoulsGlobalProjectile.TimeFreezeMoveDuration;
            if (transparency < 0) //clamp and delay the rampup until timestop is over
                transparency = 0;
            transparency /= 6; //so it takes longer to reach full brightness
            glowcolor *= Math.Min(1f, transparency);

            Vector2 drawCenter = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 18;

            float scale = Projectile.scale;
            scale *= (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;

            Main.EntitySpriteDraw(glow, drawCenter - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                glowcolor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}