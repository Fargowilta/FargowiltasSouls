using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class SpiritTornado : ModProjectile
    {
        private Vector2 mousePos;
        private int syncTimer;
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Souls/ForbiddenTornado";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forbidden Tornado");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 1200;
            Projectile.FargoSouls().DeletionImmuneRank = 2;

            Projectile.DamageType = DamageClass.Magic;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }
        int Timer = 0;
        public override void AI()
        {
            Timer++;
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.HasEffect<SpiritTornadoEffect>())
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.friendly && !p.hostile && p.owner == Projectile.owner && p.type != Projectile.type && p.Colliding(p.Hitbox, Projectile.Hitbox)))
                {
                    p.FargoSouls().stormTimer = 240;
                }
            };

            Projectile.damage = (int)(125f * (1f + player.GetDamage(DamageClass.Magic).Additive + player.GetDamage(DamageClass.Summon).Additive - 2f));

            // MOVEMENT CODE, HOME ON MOUSE
            SyncMouse(player);
            Movement(player);

            if (Timer % 13 == 0)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 vel = Vector2.UnitX * (Main.rand.NextBool() ? 1 : -1);
                    vel *= Main.rand.NextFloat(5f, 8f);
                    vel = vel.RotatedByRandom(MathF.PI * 0.23f);

                    NPC target = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1500, true, true));
                    if (target != null)
                        FargoSoulsUtil.NewProjectileDirectSafe(Projectile.InheritSource(Projectile), Projectile.Center + Vector2.UnitY * Main.rand.Next(-50, 50), vel, ProjectileID.SpectreWrath, Projectile.damage, 0, player.whoAmI, target.whoAmI);
                }
            }

            float num1123 = 900f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item82, Projectile.Center);
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= num1123)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= 30f)
            {
                Projectile.damage = 0;
                if (Projectile.ai[0] < num1123 - 120f)
                {
                    float num1124 = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = num1123 - 120f + num1124;
                    Projectile.netUpdate = true;
                }
            }
            float num1125 = 15f;
            float num1126 = 15f;
            Point point8 = Projectile.Center.ToTileCoordinates();
            Collision.ExpandVertically(point8.X, point8.Y, out int num1127, out int num1128, (int)num1125, (int)num1126);
            num1127++;
            num1128--;
            Vector2 value72 = new Vector2(point8.X, num1127) * 16f + new Vector2(8f);
            Vector2 value73 = new Vector2(point8.X, num1128) * 16f + new Vector2(8f);
            Vector2 vector145 = Vector2.Lerp(value72, value73, 0.5f);
            Vector2 value74 = new(0f, value73.Y - value72.Y);
            value74.X = value74.Y * 0.2f;
            /*
            Projectile.width = (int)(value74.X * 0.65f);
            Projectile.height = (int)value74.Y;
            Projectile.Center = vector145;
            */
            if (Projectile.owner == Main.myPlayer)
            {
                bool flag75 = false;
                Vector2 center16 = Main.player[Projectile.owner].Center;
                Vector2 top = Main.player[Projectile.owner].Top;
                for (float num1129 = 0f; num1129 < 1f; num1129 += 0.05f)
                {
                    Vector2 position2 = Vector2.Lerp(value72, value73, num1129);
                    if (Collision.CanHitLine(position2, 0, 0, center16, 0, 0) || Collision.CanHitLine(position2, 0, 0, top, 0, 0))
                    {
                        flag75 = true;
                        break;
                    }
                }
                if (!flag75 && Projectile.ai[0] < num1123 - 120f)
                {
                    float num1130 = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = num1123 - 120f + num1130;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] < num1123 - 120f)
            {
                for (int num1131 = 0; num1131 < 1; num1131++)
                {
                    float value75 = -0.5f;
                    float value76 = 0.9f;
                    float amount3 = Main.rand.NextFloat();
                    Vector2 value77 = new(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(value75, value76, amount3));
                    value77.X *= MathHelper.Lerp(2.2f, 0.6f, amount3);
                    value77.X *= -1f;
                    Vector2 value78 = new(6f, 10f);
                    Vector2 position3 = vector145 + value74 * value77 * 0.5f + value78;
                    Dust dust34 = Main.dust[Dust.NewDust(position3, 0, 0, DustID.Sandnado, 0f, 0f, 0, default, 1f)];
                    dust34.position = position3;
                    dust34.customData = vector145 + value78;
                    dust34.fadeIn = 1f;
                    dust34.scale = 0.3f;
                    if (value77.X > -1.2f)
                    {
                        dust34.velocity.X = 1f + Main.rand.NextFloat();
                    }
                    dust34.velocity.Y = Main.rand.NextFloat() * -0.5f - 1f;
                }
                return;
            }
        }
        public void SyncMouse(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
        public void Movement(Player player)
        {
            Vector2 idlePosition = mousePos;
            Vector2 toIdlePosition = idlePosition - Projectile.Center;
            float distance = toIdlePosition.Length();
            float speed = 25f;
            float inertia = 15f;
            toIdlePosition.Normalize();
            toIdlePosition *= speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + toIdlePosition) / inertia;
            if (distance == 0)
                Projectile.velocity = Vector2.Zero;
            if (distance < Projectile.velocity.Length())
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * distance;
            if (Projectile.velocity == Vector2.Zero && distance > 10)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            float halfheight = 220;
            float density = 50f;
            for (float i = 0; i < (int)density; i++)
            {
                float factor = 1f;
                Color color = new(212f * factor, 192f * factor, 100f * factor);
                color.A /= 2;
                float lerpamount = Math.Abs(density / 2 - i) > density / 2 * 0.6f ? Math.Abs(density / 2 - i) / (density / 2) : 0f; //if too low or too high up, start making it transparent
                color = Color.Lerp(color, Color.Transparent, lerpamount);
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 offset = Vector2.SmoothStep(Projectile.Center + Vector2.UnitY * halfheight, Projectile.Center - Vector2.UnitY * halfheight, i / density);
                float scale = MathHelper.Lerp(Projectile.scale * 0.8f, Projectile.scale * 2.5f, i / density);
                Main.EntitySpriteDraw(texture, offset - Main.screenPosition,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    Projectile.GetAlpha(color),
                    i / 6f - Main.GlobalTimeWrappedHourly * 5f + Projectile.rotation,
                    texture.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}