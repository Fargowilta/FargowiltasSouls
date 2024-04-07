using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{
    //ai0: 1 = p2 homing missile, 2 = p2 straight missile, 3 = p1 homing torpedo
    public class BaronRocket : ModProjectile
    {
        public bool home = true;
        public bool BeenOutside = false;
        public bool Rocket => Projectile.ai[0] != 3 && Projectile.ai[0] != 4;
        public int MaxFrames => Rocket ? 2 : 4;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron's Rocket");
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
        }

        Vector2 HomePos = Vector2.Zero;
        public override void AI()
        {
            Vector2 backPos = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 54f * Projectile.scale / 2 + Main.rand.NextVector2Circular(10, 10);
            if (Main.rand.NextBool(6) && Projectile.velocity.LengthSquared() > 9)
            {
                if (Rocket)
                {
                    Dust.NewDust(backPos, 2, 2, DustID.Torch, -Projectile.velocity.X, -Projectile.velocity.Y, 0, default, 1f);
                }
                else
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Particle p = new Bubble(backPos, -Projectile.velocity.RotatedByRandom(MathF.PI * 0.12f) * Main.rand.NextFloat(0.6f, 1f) / 2f, 1, 30, rotation: Main.rand.NextFloat(MathF.Tau));
                        p.Spawn();
                    }
                    Dust.NewDust(backPos, 2, 2, DustID.Water, -Projectile.velocity.X, -Projectile.velocity.Y, 0, default, 1f);
                }
            }


            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.Pi).ToRotation();

            if (++Projectile.localAI[0] > 600f)
            {
                Projectile.Kill();
            }
            //a bit after spawning, become tangible when it finds an open space
            if (!Projectile.tileCollide && Projectile.localAI[0] > 60 * Projectile.MaxUpdates)
            {
                Tile tile = Framing.GetTileSafely(Projectile.Center);
                if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
                {
                    Projectile.localAI[1]++;
                    if (Projectile.localAI[1] > 15)
                    {

                        Projectile.tileCollide = true;
                    }
                }
            }
            if (++Projectile.frameCounter > 8)
            {
                if (++Projectile.frame >= MaxFrames)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            if (HomePos == Vector2.Zero) //get homing pos
            {
                Player player = Main.player[(int)Projectile.ai[1]];
                if (player != null && player.active && !player.ghost)
                {
                    HomePos = player.Center;
                }
            }
            if (Projectile.ai[0] == 2) //accelerating rocket
            {
                Projectile.velocity *= 1.05f;
            }
            if (Projectile.ai[0] == 4) //accelerating torpedo p1
            {
                Projectile.velocity *= 1.05f;
            }
            if (Projectile.ai[0] == 3 || Projectile.ai[0] == 1) //homing rocket or torpedo
            {
                Player player = Main.player[(int)Projectile.ai[1]];
                bool homeTime = (Projectile.ai[0] == 3 && Projectile.localAI[0] > 10) || (Projectile.ai[0] == 1 && Projectile.localAI[0] > 40);
                if (!homeTime)
                {
                    Projectile.velocity *= 0.96f;
                }
                if (player != null && player.active && !player.ghost && homeTime)
                {
                    Vector2 vectorToIdlePosition = LerpWithoutClamp(HomePos, player.Center, Projectile.ai[2]) - Projectile.Center;
                    float speed = Projectile.ai[0] == 1 ? 24f : 18f;
                    float inertia = Projectile.ai[0] == 1 ? 24f : 20f;
                    float deadzone = WorldSavingSystem.MasochistModeReal ? 150f : 180f;
                    float num = vectorToIdlePosition.Length();
                    if (num > deadzone && home)
                    {
                        vectorToIdlePosition.Normalize();
                        vectorToIdlePosition *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                    }
                    if (Projectile.velocity == Vector2.Zero)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    if (num > deadzone)
                    {
                        BeenOutside = true;
                    }
                    if (num < deadzone && BeenOutside)
                    {
                        home = false;
                    }
                }
            }
            //}
        }
        Vector2 LerpWithoutClamp(Vector2 A, Vector2 B, float t)
        {
            return A + (B - A) * t;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 600);
            if (!WorldSavingSystem.EternityMode)
            {
                return;
            }
            target.AddBuff(BuffID.OnFire3, 600);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.Pink * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }*/
        //(public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity * 0.9f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Rocket ? Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value : ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/BanishedBaron/BaronRocketTorp", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = texture2D13.Height / MaxFrames; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + drawOffset + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);


            return false;
        }
    }
}
