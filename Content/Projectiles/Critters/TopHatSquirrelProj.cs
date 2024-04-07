using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Critters
{
    internal class TopHatSquirrelProj : ModProjectile
    {
        public int Counter = 1;

        public override string Texture => "FargowiltasSouls/Content/Items/Weapons/Misc/TophatSquirrelWeapon";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Top Hat Squirrel");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            //Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 0.5f;
            Projectile.timeLeft = 100;
        }

        public override void AI()
        {
            Projectile.spriteDirection = System.Math.Sign(Projectile.velocity.X);
            Projectile.rotation += 0.2f * Projectile.spriteDirection;
            Projectile.scale += .02f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                int proj2 = ModContent.ProjectileType<TopHatSquirrelLaser>();

                FargoSoulsUtil.XWay(16, Projectile.GetSource_FromThis(), Projectile.Center, proj2, Projectile.velocity.Length() * 2f, Projectile.damage * 4, Projectile.knockBack);

                int max = Main.player[Projectile.owner].ownedProjectileCounts[proj2] >= 50 ? 25 : 50;
                for (int i = 0; i < max; i++)
                {
                    Vector2 pos = Projectile.Center + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(600, 1800) +
                        Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.Pi / 2)) * Main.rand.NextFloat(-900, 900);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, -Projectile.velocity * Main.rand.NextFloat(2f, 3f), proj2,
                        Projectile.damage * 4, Projectile.knockBack, Main.myPlayer);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
            {
                Color color27 = new(93, 255, 241, 0);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale * 1.1f, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}