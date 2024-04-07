using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SpazmaglaiveExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            //Projectile.aiStyle = 136;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 8;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.FargoSouls().CanSplit = false;
        }

        public int timer;
        public float lerp = 0.12f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cursed Inferno");
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[1]];
            if (!target.active)
                Projectile.Kill();

            Vector2 center = target.Center;
            Projectile.Center = center;
            Projectile.rotation = Projectile.velocity.ToRotation();

            DelegateMethods.v3_1 = new Vector3(1.2f, 1f, 0.3f);
            float num2 = Projectile.localAI[0] / 40f;
            if (num2 > 1f)
            {
                num2 = 1f;
            }
            float num3 = (Projectile.localAI[0] - 38f) / 40f;
            if (num3 < 0f)
            {
                num3 = 0f;
            }
            //Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2() * 100f * num3, Projectile.Center + Projectile.rotation.ToRotationVector2() * 100f * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));
            //Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583, default(Vector2)) * 100f * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583, default(Vector2)) * 100f * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));
            //Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583, default(Vector2)) * 100f * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583, default(Vector2)) * 100f * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));


            Projectile.localAI[0] += 1f;

            if (Projectile.localAI[0] >= Projectile.ai[0])
            {
                Projectile.Kill();
            }
        }

        public override bool? Colliding(Rectangle myRect, Rectangle targetRect)
        {
            float num11 = 0f;
            float num12 = Projectile.localAI[0] / 25f;
            if (num12 > 1f)
            {
                num12 = 1f;
            }
            float num13 = (Projectile.localAI[0] - 38f) / 40f;
            if (num13 < 0f)
            {
                num13 = 0f;
            }
            Vector2 lineStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 100f * num13;
            Vector2 lineEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * 200f * num12;
            if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), lineStart, lineEnd, 40f * Projectile.scale, ref num11))
            {
                return true;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(13, 219, 83);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 value10 = Projectile.Center;
            value10 -= Main.screenPosition;
            float num178 = 40f;
            float num179 = num178 * 2f;
            float num180 = Projectile.localAI[0] / num178;
            Texture2D texture2D5 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Color color33 = Color.White;
            Color color34 = new(255, 255, 255, 0);
            Color color35 = new(30, 180, 30, 0);
            Color color36 = new(0, 30, 0, 0);
            ulong num181 = 1uL;
            for (float num182 = 0f; num182 < 15f; num182 += 0.66f)
            {
                float num183 = Utils.RandomFloat(ref num181) * 0.25f - 0.125f;
                Vector2 value11 = (Projectile.rotation + num183).ToRotationVector2();
                Vector2 value12 = value10 + value11 * 200f;
                float num184 = num180 + num182 * 0.06666667f;
                int num185 = (int)(num184 / 0.06666667f);
                num184 %= 1f;
                if ((num184 <= num180 % 1f || Projectile.localAI[0] >= num178) && (num184 >= num180 % 1f || Projectile.localAI[0] < num179 - num178))
                {
                    if (num184 < 0.1f)
                    {
                        color33 = Color.Lerp(Color.Transparent, color34, Utils.GetLerpValue(0f, 0.1f, num184, true));
                    }
                    else if (num184 < 0.35f)
                    {
                        color33 = color34;
                    }
                    else if (num184 < 0.7f)
                    {
                        color33 = Color.Lerp(color34, color35, Utils.GetLerpValue(0.35f, 0.7f, num184, true));
                    }
                    else if (num184 < 0.9f)
                    {
                        color33 = Color.Lerp(color35, color36, Utils.GetLerpValue(0.7f, 0.9f, num184, true));
                    }
                    else if (num184 < 1f)
                    {
                        color33 = Color.Lerp(color36, Color.Transparent, Utils.GetLerpValue(0.9f, 1f, num184, true));
                    }
                    else
                    {
                        color33 = Color.Transparent;
                    }
                    color33.A = (byte)(255 - num184 * 255);
                    float num186 = 0.9f + num184 * 0.8f;
                    num186 *= num186;
                    num186 *= 0.8f;
                    Vector2 position = Vector2.SmoothStep(value10, value12, num184);
                    Rectangle rectangle2 = texture2D5.Frame(1, 7, 0, (int)(num184 * 7f));
                    Main.EntitySpriteDraw(texture2D5, position, new Microsoft.Xna.Framework.Rectangle?(rectangle2), color33, Projectile.rotation + 6.28318548f * (num184 + Main.GlobalTimeWrappedHourly * 1.2f) * 0.2f + num185 * 1.2566371f, rectangle2.Size() / 2f, num186 / 2, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 180, false);
        }
    }
}