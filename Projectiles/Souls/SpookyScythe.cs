using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class SpookyScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 84;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 50;
            Projectile.tileCollide = false;
            Projectile.scale *= .5f;
            Projectile.timeLeft = 300;

            //this deals more dmg generally but e
            /*Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;*/

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            if (!Projectile.tileCollide && !Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height))
                Projectile.tileCollide = true;

            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 55, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 55, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId3].noGravity = true;

            Projectile.rotation += 0.4f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int num489 = 0; num489 < 5; num489++)
            {
                int num490 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, 10f, 30f, 100);
                Main.dust[num490].noGravity = true;
                Main.dust[num490].velocity *= 1.5f;
                Main.dust[num490].scale *= 0.9f;
            }

            /*for (int i = 0; i < 3; i++)
            {
                float x = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                float y = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                int p = Projectile.NewProjectile(Projectile.position.X + x, Projectile.position.Y + y, x, y, 45, (int) (Projectile.damage * 0.5), 0f, Projectile.owner);

                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            } */
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}