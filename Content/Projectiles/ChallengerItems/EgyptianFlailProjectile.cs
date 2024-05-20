using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.Audio;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.ModPlayers;
using System;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class EgyptianFlailProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.RangeMultiplier = 0.65f;
            Projectile.rotation += Projectile.ai[0];
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Gold);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] <= 0)
            {
                WhipExplosion(target);
                Projectile.ai[1] = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);


            Main.DrawWhip_CoolWhip(Projectile, list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            return false;
        }

        public void WhipExplosion(NPC target)
        {
            Vector2 vel = Main.rand.NextVector2Unit();
            vel *= 3f;

            SoundEngine.PlaySound(SoundID.Item103, target.Center);

            int dam = 50;

            void ShootTentacle(Vector2 baseVel, float variance, int aiMin, int aiMax)
            {
                Vector2 speed = baseVel.RotatedBy(variance * (Main.rand.NextDouble() - 0.5));
                float ai0 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                if (Main.rand.NextBool())
                    ai0 *= -1f;
                float ai1 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                if (Main.rand.NextBool())
                    ai1 *= -1f;
                Projectile.NewProjectile(target.GetSource_FromThis(), target.Center, speed, ModContent.ProjectileType<ShadowflameTentacle>(), dam, 4f, Projectile.owner, ai0, ai1);
            };

            int max = 5;
            float rotationOffset = MathHelper.TwoPi / max;
            for (int i = 0; i < max; i++) {
                ShootTentacle(vel.RotatedBy(rotationOffset * i), rotationOffset, 30, 50);
            }
        }
    }
}
