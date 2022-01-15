using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MothDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Moth Dust");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = -1;
            //projectile.hide = true;
            projectile.hostile = true;
            projectile.timeLeft = 180;

            projectile.scale = 0.5f;
        }

        public override void AI()
        {
            projectile.velocity *= .96f;

            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 70);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2.5f;
            }

            Lighting.AddLight(projectile.position, .3f, .1f, .3f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
            {
                target.AddBuff(mod.BuffType("Berserked"), 240);
                target.AddBuff(mod.BuffType("MutantNibble"), 240);
                target.AddBuff(mod.BuffType("Guilty"), 240);
                target.AddBuff(mod.BuffType("Lovestruck"), 240);
                target.AddBuff(mod.BuffType("Rotting"), 240);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Main.rand.Next(Fargowiltas.DebuffIDs.Count);
                    target.AddBuff(Fargowiltas.DebuffIDs[d], 240);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = texture2D13.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255), projectile.rotation, origin2, projectile.scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255, 0), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}