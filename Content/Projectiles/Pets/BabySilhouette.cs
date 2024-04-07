using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Pets
{
    public class BabySilhouette : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Silhouette");
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenBeePet);
            AIType = ProjectileID.QueenBeePet;
            Projectile.width = 60;
            Projectile.height = 50;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].petFlagQueenBeePet = false;
            return true;
        }

        int realFrameCounter;
        int realFrame;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.dead)
            {
                modPlayer.BabySilhouette = false;
            }
            if (modPlayer.BabySilhouette)
            {
                Projectile.timeLeft = 2;
            }



            if (++realFrameCounter > 4)
            {
                realFrameCounter = 0;
                if (++realFrame >= Main.projFrames[Projectile.type])
                    realFrame = 0;
            }

            Projectile.frame = realFrame;

            if (Main.dayTime && Projectile.Center.ToTileCoordinates().Y <= Main.worldSurface
                && Main.wallLight[Framing.GetTileSafely(Projectile.Center).WallType])
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 0, default, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].velocity += Projectile.velocity * 0.9f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 0, default, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.5f;
                    Main.dust[d].velocity += Projectile.velocity * 0.9f;
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
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}