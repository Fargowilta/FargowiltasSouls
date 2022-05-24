using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MothronZenith : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zenith");
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.scale = 1.5f;

            Projectile.hide = true;

            CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            if (Projectile.alpha > 0)
                return false;

            return base.CanDamage();
        }

        public override bool CanHitPlayer(Player target)
        {
            if (target.hurtCooldowns[CooldownSlot] > 0)
                return false;

            return base.CanHitPlayer(target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float length;
            switch (Projectile.frame)
            {
                case 0: length = 56; break;
                case 1: length = 40; break;
                case 2: length = 54; break;
                case 3: length = 50; break;
                case 4: length = 44; break;
                case 5: length = 56; break;
                case 6: length = 50; break;
                case 7: length = 32; break;
                case 8: length = 34; break;
                case 9: length = 46; break;
                case 10: length = 34; break;
                default: goto case 0;
            }
            length = (float)Math.Sqrt(2 * length * length);
            length *= 0.8f;

            float dummy = 0f;
            Vector2 offset = length / 2 * Projectile.scale * Projectile.rotation.ToRotationVector2();
            Vector2 end = Projectile.Center - offset;
            Vector2 tip = Projectile.Center + offset;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, 8f * Projectile.scale, ref dummy))
                return true;

            return false;
        }

        private int dustTimer;

        public override void AI()
        {
            const int startup = 100;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.localAI[1] = Projectile.velocity.Length() / startup;

                Projectile.hide = false;
                Projectile.rotation = 2f * Main.rand.NextFloat(MathHelper.TwoPi) * (Main.rand.NextBool() ? -1 : 1);
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
            }

            if (++dustTimer == 15)
            {
                MakeDust();
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            if (Projectile.ai[0] == -1) //fly forward
            {
                if (++Projectile.localAI[0] <= startup)
                {
                    Projectile.velocity = (Projectile.velocity.Length() - Projectile.localAI[1]) * Vector2.Normalize(Projectile.velocity);

                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.ai[1] > 0 ? 0 : MathHelper.Pi, 0.05f);
                    Projectile.spriteDirection = (int)Projectile.ai[1];

                    if (Projectile.localAI[0] == startup)
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                }
                else
                {
                    Projectile.velocity = 36f * Projectile.ai[1] * Vector2.UnitX;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else //hover around mothron
            {
                NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.Mothron);
                if (npc == null || (npc.ai[0] < 3f && dustTimer > 15))
                {
                    Projectile.Kill();
                    return;
                }
                else
                {
                    Projectile.timeLeft++;

                    const float rotationPerTick = MathHelper.TwoPi / 60f;

                    Projectile.spriteDirection = Math.Sign(Projectile.ai[1]);
                    Projectile.ai[1] += rotationPerTick * Projectile.spriteDirection;
                    Projectile.rotation = Projectile.ai[1];

                    float distance = 120;
                    if (Projectile.spriteDirection < 0)
                        distance *= 2;
                    Projectile.Center = npc.Center + distance * Projectile.ai[1].ToRotationVector2();

                    if (npc.ai[0] < 4f)
                        Projectile.alpha -= 4;
                }
            }

            Projectile.alpha -= 4;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            for (int i = 0; i < 30; i++)
            {
                int debuff = Main.rand.Next(FargowiltasSouls.DebuffIDs);
                if (!target.buffImmune[debuff] && !Main.buffNoTimeDisplay[debuff])
                {
                    target.AddBuff(debuff, 240);
                    if (target.HasBuff(debuff))
                        break;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override void Kill(int timeLeft)
        {
            if (dustTimer >= 15)
            {
                MakeDust();
                SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
            }
        }

        private void MakeDust()
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 0, SwordColor, 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotationOffset = MathHelper.ToRadians(45) * Projectile.spriteDirection;
            if (Projectile.spriteDirection < 0)
                rotationOffset += MathHelper.Pi;

            Color color26 = SwordColor;
            color26 *= Projectile.Opacity;
            color26.A = 20;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.25f)
            {
                Color color27 = color26 * 0.5f;
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0] + rotationOffset;
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            //float scale = Projectile.scale * (Main.mouseTextColor / 200f - 0.35f) * 0.3f + 1f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation + rotationOffset, origin2, Projectile.scale * 1.2f, effects, 0);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation + rotationOffset, origin2, Projectile.scale, effects, 0);
            return false;
        }

        private Color SwordColor
        {
            get
            {
                switch (Projectile.frame)
                {
                    case 0: return Color.MintCream;
                    case 1: return Color.Yellow;
                    case 2: return Color.Orange;
                    case 3: return Color.Cyan;
                    case 4: return Color.Green;
                    case 5: return Color.HotPink;
                    case 6: return Color.OrangeRed;
                    case 7: return Color.Orange;
                    case 8: return Color.Red;
                    case 9: return Color.LimeGreen;
                    case 10: return Color.Blue;
                    default: goto case 0;
                }
            }
        }
    }
}