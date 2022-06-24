using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class PhantasmalSphereTrueEye : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Sphere");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            Projectile eye = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, Projectile.ai[0], ModContent.ProjectileType<TrueEyeR>()));
            if (eye != null)
            {
                if (Projectile.localAI[0] == 0 && Projectile.localAI[1] == 0)
                {
                    Vector2 offset = Projectile.Center - eye.Center;
                    Projectile.localAI[0] = offset.X;
                    Projectile.localAI[1] = offset.Y;
                }

                if (eye.ai[1] == 0f) //stop following true eye if true eye lost target & isn't preparing to charge
                {
                    Projectile.ai[0] = -1f;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity = eye.velocity;
                }

                if (eye.ai[1] == 2f && eye.localAI[0] > 1)
                {
                    Projectile.ai[0] = -1f;
                }
                else
                {
                    Projectile.Center = eye.Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                    Projectile.position -= Projectile.velocity;
                }
            }
            else if (Projectile.timeLeft > 60)
            {
                Projectile.timeLeft = 60;
            }

            Projectile.velocity *= 1.01f;

            if (Projectile.alpha > 200)
                Projectile.alpha = 200;
            Projectile.alpha -= 5;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            Projectile.scale = 0.65f - (0.65f * Projectile.alpha / 255f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255) * (1f - Projectile.alpha / 255f) * 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/MutantBoss/MutantSphereGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int rect1 = glow.Height;
            int rect2 = 0;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(255, 255, 255, 0), Color.Transparent, 0.92f);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++) //reused betsy fireball scaling trail thing
            {

                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.5f, SpriteEffects.None, 0);
            }
            glowcolor = Color.Lerp(new Color(196, 247, 255, 0), Color.Transparent, 0.8f);
            Main.EntitySpriteDraw(glow, Projectile.position + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), glowcolor,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void Kill(int timeleft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = 0.5f, Pitch = 0 }, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 208;
            Projectile.position.X -= (float)(Projectile.width / 2);
            Projectile.position.Y -= (float)(Projectile.height / 2);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
            }
            for (int index1 = 0; index1 < 30; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 1f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index3].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Dust dust2 = Main.dust[index3];
                dust2.velocity = dust2.velocity * 1f;
                Main.dust[index3].noGravity = true;
            }
        }
    }
}