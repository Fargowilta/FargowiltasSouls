using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Pets
{
    public class AccursedSarcophagus : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Accursed Sarcophagus");
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TikiSpirit);
            AIType = ProjectileID.TikiSpirit;

            Projectile.width = 30;
            Projectile.height = 52;
        }

        int realFrameCounter;
        int realFrame;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.dead)
            {
                modPlayer.AccursedSarcophagus = false;
            }
            if (modPlayer.AccursedSarcophagus)
            {
                Projectile.timeLeft = 2;
            }

            if (++realFrameCounter > 6)
            {
                realFrameCounter = 0;
                if (++realFrame >= Main.projFrames[Projectile.type])
                    realFrame = 0;
            }

            Projectile.frame = realFrame;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = /*Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally :*/ SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);

            Texture2D glowTexture = ModContent.Request<Texture2D>($"{Texture}_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}