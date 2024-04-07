using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class FishStickProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fish Stick");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.JavelinFriendly;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-45f);

            int max = 3;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector2_1 = Projectile.position;
                Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - (float)Math.PI / 2).ToRotationVector2() * Main.rand.Next(3, 8);
                Vector2 vector2_3 = vector2_2;
                int index2 = Dust.NewDust(vector2_1 + vector2_3, Projectile.width, Projectile.height, DustID.DungeonWater, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity /= 4f;
                Main.dust[index2].velocity -= Projectile.velocity;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width /= 2;
            height /= 2;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public static void ShootSharks(Vector2 target, float speed, IEntitySource source, int damage, float knockback, int whoAmI = -1)
        {
            foreach (Projectile tornado in Main.projectile.Where(p => p.active && p.ModProjectile is Whirlpool && p.owner == Main.myPlayer))
            {
                if (Main.rand.NextBool())
                {
                    float shootSpeed = speed;
                    Vector2 vel = Vector2.Normalize(target - tornado.Center + Main.rand.NextVector2Circular(32, 32))
                        * shootSpeed * Main.rand.NextFloat(1f, 1.5f);
                    //increase damage, compensate for probably missing the mouse-aimed ones
                    int sharkDamage = damage / FishStickWhirlpool.TornadoHeight;
                    Projectile.NewProjectile(source, tornado.Center, vel, ModContent.ProjectileType<FishStickShark>(), sharkDamage, knockback, Main.myPlayer, ai2: whoAmI);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                ShootSharks(target.Center, Projectile.velocity.Length(), Projectile.GetSource_FromThis(), Projectile.damage, Projectile.knockBack, target.whoAmI);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotationModifier = Projectile.spriteDirection > 0 ? MathHelper.PiOver2 : MathHelper.Pi;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i] + rotationModifier;
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation + rotationModifier, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}