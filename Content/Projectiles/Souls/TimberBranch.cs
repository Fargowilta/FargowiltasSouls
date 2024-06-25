using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using FargowiltasSouls.Content.Buffs.Souls;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class TimberBranch : ModProjectile
    {
        public const int Duration = 90;
        public const int FadeTime = 5;
        public const int GrowthSpeedOffset = 3;
        public const int FadeinTimeLeft = Duration - FadeTime;

        public const int Length = 84;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = Duration;
            Projectile.Opacity = 0f;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 120;
        }
        public ref float DepthRemaining => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[2];
        public override void AI()
        {
            Projectile.Opacity = 1f;
            if (Projectile.timeLeft > FadeinTimeLeft)
            {
                float progress = 1f - ((Projectile.timeLeft - FadeinTimeLeft) / (float)FadeTime);
                Projectile.scale = 1f * progress;
            }
            if (Projectile.timeLeft == FadeinTimeLeft + GrowthSpeedOffset)
            {
                if (DepthRemaining > 0)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = -1; i <= 1; i += 2)
                        {
                            float rotation = i * Main.rand.NextFloat(MathHelper.PiOver2 * 0.1f, MathHelper.PiOver2 * 0.3f);
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + Projectile.velocity * (Length - Projectile.width), Projectile.velocity.RotatedBy(rotation), Type, Projectile.damage, Projectile.knockBack, Projectile.owner, DepthRemaining - 1);
                        }
                    }
                }
                   
            }
            if (Projectile.timeLeft <= FadeTime)
            {
                float progress = Projectile.timeLeft / (float)FadeTime;
                Projectile.scale = 1f * progress;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = Vector2.Zero;

            Vector2 branchCenter = Projectile.Center + (Vector2.Normalize(Projectile.velocity) * Projectile.scale * Length);
            Main.EntitySpriteDraw(texture, branchCenter - Main.screenPosition, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            
            if (DepthRemaining == 0)
            {
                /*
                Texture2D texture2 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Souls/TimberBranchTip").Value;
                Rectangle rectangle2 = new(0, 0, texture2.Width, texture2.Height);
                Vector2 origin2 = Vector2.Zero;

                Vector2 crownCenter = Projectile.Center + (Vector2.Normalize(Projectile.velocity) * Projectile.scale * Length);

                Main.EntitySpriteDraw(texture2, crownCenter - Main.screenPosition, rectangle2, Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
                */
            }
            
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * Length, Projectile.width, ref point);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();
            target.AddBuff(ModContent.BuffType<WitheredForceBuff>(), 60 * 4);
            target.AddBuff(ModContent.BuffType<TimberBleedBuff>(), 60 * 4);
        }
    }
}
