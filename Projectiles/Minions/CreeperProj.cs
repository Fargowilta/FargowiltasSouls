using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class CreeperProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Creeper");
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item20, projectile.position);
                projectile.localAI[0] = 1f;
                projectile.ai[1] = -1f;
            }

            int dust = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 100, new Color(244, 66, 113), 1f);
            Main.dust[dust].velocity *= 0.1f;
            if (projectile.velocity == Vector2.Zero)
            {
                Main.dust[dust].velocity.Y -= 1f;
                Main.dust[dust].scale = 1.2f;
            }
            else
            {
                Main.dust[dust].velocity += projectile.velocity * 0.2f;
            }

            /*Main.dust[dust].position.X = projectile.Center.X + 4f + Main.rand.Next(-2, 3);
            Main.dust[dust].position.Y = projectile.Center.Y + Main.rand.Next(-2, 3);*/
            Main.dust[dust].noGravity = true;

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1]);
            if (npc != null) //has target
            {
                float rotation = projectile.velocity.ToRotation();
                Vector2 vel = npc.Center - projectile.Center;
                float targetAngle = vel.ToRotation();
                projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.008f));
            }
            else //currently has no target
            {
                if (--projectile.localAI[1] < 0)
                {
                    projectile.localAI[1] = 10;
                    projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 2000);
                    if (projectile.ai[1] != -1)
                        projectile.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}