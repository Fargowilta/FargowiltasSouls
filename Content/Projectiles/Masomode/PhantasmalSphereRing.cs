using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PhantasmalSphereRing : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantasmal Sphere");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
            Projectile.alpha = 200;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 1;

            Projectile.extraUpdates = 1;

            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            //float ratio = Projectile.timeLeft / 600f;
            //Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] * ratio + Projectile.ai[1] * (1 - ratio));
            /*Projectile.localAI[0] += Projectile.ai[0] * Projectile.timeLeft / 300f;
            Projectile.velocity.X = (float)(Math.Cos(Projectile.localAI[0] + Projectile.ai[1]) - Projectile.localAI[0] * Math.Sin(Projectile.localAI[0] + Projectile.ai[1]));
            Projectile.velocity.Y = (float)(Math.Sin(Projectile.localAI[0] + Projectile.ai[1]) + Projectile.localAI[0] * Math.Cos(Projectile.localAI[0] + Projectile.ai[1]));*/
            //Projectile.velocity *= (Projectile.timeLeft > 300 ? Projectile.timeLeft / 300f : 1f);
            //Main.NewText(Projectile.velocity.Length().ToString());
            //Projectile.velocity *= 1f + Projectile.ai[0];
            //Projectile.velocity += Projectile.velocity.RotatedBy(Math.PI / 2) * Projectile.ai[1];
            /*if (spawn == Vector2.Zero)
                spawn = Projectile.position;
            Projectile.localAI[0] += Projectile.ai[0] * (Projectile.timeLeft > 300 ? Projectile.timeLeft / 300f : 1f);
            Vector2 vel = new Vector2(Projectile.localAI[0] * (float)Math.Cos(Projectile.localAI[0] + Projectile.ai[1]) * 120f,
                Projectile.localAI[0] * (float)Math.Sin(Projectile.localAI[0] + Projectile.ai[1]) * 120f);
            Projectile.position = spawn + vel;
            vel = Projectile.position - Projectile.oldPosition;*/
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1] / (2 * Math.PI * Projectile.ai[0] * ++Projectile.localAI[0]));

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Projectile.scale = 1f - Projectile.alpha / 255f;

            if (++Projectile.frameCounter >= 6 * Projectile.MaxUpdates)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MutantNibbleBuff>(), 180);
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 360);
        }

        public override void OnKill(int timeleft)
        {
            //SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 208;
            Projectile.Center = Projectile.position;
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 1f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index3].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 1f;
                Main.dust[index3].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantSphereGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int rect1 = glow.Height;
            int rect2 = 0;
            Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(196, 247, 255, 0), Color.Transparent, 0.9f);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++) //reused betsy fireball scaling trail thing
            {

                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i] - Vector2.Normalize(Projectile.velocity) * i * 2;
                Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.5f, SpriteEffects.None, 0);
            }
            glowcolor = Color.Lerp(new Color(255, 255, 255, 0), Color.Transparent, 0.85f);
            Main.EntitySpriteDraw(glow, Projectile.position + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), glowcolor,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

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