using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    public class CoffinRandomStuff : ModProjectile
    {
        public const int Frames = 7;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron Scrap");
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = Frames;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60 * 6;
        }
        public ref float ObjectType => ref Projectile.ai[0];
        public ref float StartHeight => ref Projectile.ai[1];
        public static float Gravity(float objectType) =>
            objectType switch
            {
                5 => 0.15f, // vase
                6 => 0.17f, // bone
                _ => 0.2f // gem
            };
        public override void OnSpawn(IEntitySource source)
        {
            StartHeight = Projectile.Center.Y;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = ObjectType switch
            {
                5 => 28,
                6 => 16,
                _ => 32
            };
            Projectile.Center = Projectile.position;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] < 12)
            {
                Projectile.localAI[1]++;
                Projectile.scale = MathHelper.Lerp(0, 1, Projectile.localAI[1] / 12);
            }

            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;

            Projectile.rotation += MathF.Tau * Projectile.localAI[0] / 33;

            Projectile.velocity.Y += Gravity(ObjectType);
            Projectile.tileCollide = Projectile.Center.Y > StartHeight;
            Projectile.frame = (int)ObjectType;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frame = (int)ObjectType;

            //draw projectile
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = lightColor;
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
