using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Pets
{
    public class BabyAbom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Abom");
            Main.projFrames[Projectile.type] = 8;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 26;
            AIType = ProjectileID.BabyHornet;
            Projectile.netImportant = true;
            Projectile.friendly = true;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].hornet = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.dead)
            {
                modPlayer.BabyAbom = false;
            }
            if (modPlayer.BabyAbom)
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.Distance(player.Center) > 3000)
                Projectile.Center = player.Center;

            for (int i = 0; i < 2; i++)
            {
                int index = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale),
                  DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(), 1.5f);
                Vector2 focus = Projectile.position;
                if (Projectile.direction >= 0)
                    focus.X += Projectile.width;
                focus.Y += Projectile.height / 2;
                Main.dust[index].position = (Main.dust[index].position + focus) / 2f;
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity = Main.dust[index].velocity * 0.3f;
                Main.dust[index].velocity = Main.dust[index].velocity - Projectile.velocity * 0.1f;
            }

            /*float distance = Projectile.width * Projectile.scale; //aura dust
            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * distance);
                offset.Y += (float)(Math.Cos(angle) * distance);
                Dust dust = Main.dust[Dust.NewDust(
                    Projectile.Center + Projectile.velocity + offset - new Vector2(4, 4), 0, 0,
                    DustID.Shadowflame, 0, 0, 100, Color.White, 1f)];
                dust.velocity = Projectile.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset) * -3f;
                dust.noGravity = true;
            }*/
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