using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class DragonBreathProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public int timer;
        public const float lerp = 0.18f;
        public const float halfRange = 500f;
        public const float halfRangeReduced = halfRange / 10f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon's Breath");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            //Projectile.aiStyle = 136;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 8;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer != player.whoAmI)
                return;

            if (player.dead || !player.active)
                Projectile.Kill();

            Vector2 center = player.MountedCenter;
            Projectile.Center = center;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (++timer > 24)
                timer = 0;

            if (player.channel)
            {
                Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity),
                    Vector2.Normalize(Main.MouseWorld - player.MountedCenter), lerp); //slowly move towards direction of cursor
                Projectile.velocity.Normalize();

                if (timer == 0 || timer == 12)
                {
                    player.PickAmmo(player.inventory[player.selectedItem], out int _, out float _, out int _, out float _, out int _);
                }
                Projectile.timeLeft++;
            }

            float extrarotate = (Projectile.direction < 0) ? MathHelper.Pi : 0;
            player.itemRotation = Projectile.velocity.ToRotation() + extrarotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Vector2 HoldOffset = new Vector2(60, 0).RotatedBy(Projectile.velocity.ToRotation());
            Projectile.Center += HoldOffset;

            DelegateMethods.v3_1 = new Vector3(1.2f, 1f, 0.3f);
            float num2 = Math.Min(Projectile.ai[0] / halfRangeReduced, 0.75f) * 2f;
            float num3 = Math.Max((Projectile.ai[0] - halfRangeReduced * 0.95f) / halfRangeReduced, 0f) * 2;
            if (num3 > num2)
                num3 = num2;
            //Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2() * halfRange * num3, Projectile.Center + Projectile.rotation.ToRotationVector2() * halfRange * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));
            //Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(10))) * halfRange * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(10))) * halfRange * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));
            //Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.ToRadians(Main.rand.NextFloat(10))) * halfRange * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.ToRadians(5)) * halfRange * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));

            /*if (Main.rand.NextBool(4) && Projectile.ai[0] >= 25f)
			{
				Vector2 vector = Projectile.Center + Projectile.rotation.ToRotationVector2() * 600f;
				vector -= Utils.RandomVector2(Main.rand, -halfRangeReduced, halfRangeReduced);
				Gore gore = Gore.NewGoreDirect(vector, Vector2.Zero, 61 + Main.rand.Next(3), 1f);
				gore.velocity *= 0.6f;
				gore.velocity += Projectile.rotation.ToRotationVector2() * 4f;
			}*/
            //Projectile.frameCounter++;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > halfRangeReduced * 1.1f)
            {
                if (player.channel && player.HasAmmo(player.inventory[player.selectedItem]))
                    Projectile.ai[0] = halfRangeReduced * 0.9f;
            }

            if (Projectile.ai[0] <= halfRangeReduced * 1.1f && timer == 0)
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Projectile.Center + (Projectile.velocity * 600)); //dd2 sound effects are weird so this is temporary(?) fix to sound effect being too loud

            if (Projectile.ai[0] >= halfRangeReduced * 1.95f)
            {
                Projectile.Kill();
            }
        }

        public override bool? Colliding(Rectangle myRect, Rectangle targetRect)
        {
            float num11 = 0f;
            float num12 = Projectile.ai[0] / (halfRangeReduced * 0.9f);
            if (num12 > 1f)
            {
                num12 = 1f;
            }
            float num13 = (Projectile.ai[0] - halfRangeReduced * 0.95f) / halfRangeReduced;
            if (num13 < 0f)
            {
                num13 = 0f;
            }
            Vector2 lineStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * halfRange * num13;
            Vector2 lineEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * halfRange * 2 * num12;
            for (int i = -1; i <= 1; i++) //cone hitbox
            {
                Vector2 rotatedLineEnd = lineStart + (lineEnd - lineStart).RotatedBy(MathHelper.ToRadians(5 * i));
                if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), lineStart, rotatedLineEnd, 40f * Projectile.scale, ref num11))
                    return true;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(102, 224, 255);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 value10 = Projectile.Center;
            value10 -= Main.screenPosition;
            float fullRangeReduced = halfRangeReduced * 2f;
            float num180 = (float)Projectile.ai[0] / halfRangeReduced;
            Texture2D texture2D5 = TextureAssets.Projectile[Projectile.type].Value;
            Color color33 = Color.Transparent;
            Color color34 = new Color(255, 255, 255, 0);
            Color color35 = new Color(180, 30, 30, 200);
            Color color36 = new Color(30, 0, 00, 30);
            ulong num181 = 1; //(ulong)(Projectile.ai[0] / halfRangeReduced * 2);
            for (float num182 = 0f; num182 < 30f; num182 += 0.66f)
            {
                float num183 = Utils.RandomFloat(ref num181) * 0.25f - 0.125f;
                Vector2 value11 = (Projectile.rotation + num183).ToRotationVector2();
                Vector2 value12 = value10 + value11 * halfRange * 2;
                float num184 = num180 + num182 * 0.06666667f;
                int num185 = (int)(num184 / 0.06666667f);
                num184 %= 1f;
                if ((num184 <= num180 % 1f || (float)Projectile.ai[0] >= halfRangeReduced) && (num184 >= num180 % 1f || (float)Projectile.ai[0] < fullRangeReduced - halfRangeReduced))
                {
                    if (num184 < 0.1f)
                    {
                        color33 = Color.Lerp(Color.Transparent, color34, Utils.GetLerpValue(0f, 0.1f, num184, true));
                    }
                    else if (num184 < 0.3f)
                    {
                        color33 = color34;
                    }
                    else if (num184 < 0.7f)
                    {
                        color33 = Color.Lerp(color34, color35, Utils.GetLerpValue(0.3f, 0.7f, num184, true));
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
                    float num186 = 0.9f + num184 * 0.8f;
                    num186 *= num186;
                    num186 *= 0.8f;
                    Vector2 position = Vector2.SmoothStep(value10, value12, num184);
                    Rectangle rectangle2 = texture2D5.Frame(1, 7, 0, (int)(num184 * 7f));
                    Main.EntitySpriteDraw(texture2D5, position, new Microsoft.Xna.Framework.Rectangle?(rectangle2), color33, Projectile.rotation + 6.28318548f * (num184 + Main.GlobalTimeWrappedHourly * 1.2f) * 0.2f + (float)num185 * 1.2566371f, rectangle2.Size() / 2f, num186, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;

            target.AddBuff(BuffID.OnFire, 180, false);
            target.AddBuff(BuffID.Oiled, 180, false);
            target.AddBuff(BuffID.BetsysCurse, 180, false);

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, target.Center);

            Vector2 vel = Projectile.rotation.ToRotationVector2() * 2.5f;

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(target.position, target.width, target.height, Main.rand.Next(new int[] { 6, 55, 158 }), vel.X, vel.Y);
                Main.dust[d].alpha = 200;
                Main.dust[d].velocity *= 2.4f;
                Main.dust[d].scale += Main.rand.NextFloat(0.5f);
                if (Main.dust[d].type == 55)
                    Main.dust[d].color = Color.Lerp(Color.Red, Color.Gold, Main.rand.NextFloat());
                Main.dust[d].noLight = true;

                d = Dust.NewDust(target.position, target.width, target.height, 55, vel.X, vel.Y);
                Main.dust[d].alpha = 120;
                Main.dust[d].velocity *= 2.4f;
                Main.dust[d].scale += Main.rand.NextFloat(0.2f);
                Main.dust[d].color = Color.Lerp(Color.Purple, Color.Black, Main.rand.NextFloat());
                Main.dust[d].noLight = true;

                d = Dust.NewDust(target.position, target.width, target.height, 55, vel.X, vel.Y);
                Main.dust[d].alpha = 80;
                Main.dust[d].velocity *= 0.45f;
                Main.dust[d].scale += Main.rand.NextFloat(0.2f);
                Main.dust[d].color = Color.Lerp(Color.Purple, Color.Black, Main.rand.NextFloat());
                Main.dust[d].noLight = true;
            }

            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(target.position, target.width, target.height, 228, vel.X, vel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 1.25f + Main.rand.NextFloat();
                Main.dust[d].fadeIn = 1.5f;
                Main.dust[d].velocity *= 0.5f;
                Main.dust[d].velocity += vel;
                Main.dust[d].velocity *= Main.rand.NextFloat(6f);
                Main.dust[d].noLight = true;
            }
        }
    }
}