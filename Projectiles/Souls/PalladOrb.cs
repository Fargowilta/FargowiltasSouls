using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class PalladOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Palladium Life Orb");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 2f;

            if (ModLoader.GetMod("Fargowiltas") != null)
                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", projectile);
        }

        public override void AI()
        {
            if (projectile.velocity.Length() < 32f) //accelerate over time
            {
                projectile.velocity = Vector2.Normalize(projectile.velocity) * (projectile.velocity.Length() + 32f / 300f);
            }

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0]);
            if (npc != null)
            {
                projectile.velocity = projectile.DirectionTo(npc.Center) * projectile.velocity.Length();
            }
            else
            {
                if (++projectile.localAI[0] > 6f)
                {
                    projectile.localAI[0] = 1f;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1500);
                    projectile.netUpdate = true;
                }
            }

            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                Main.PlaySound(SoundID.Item, projectile.Center, 14);
            }

            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].velocity *= 3;
            int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 1f);
            Main.dust[index3].velocity *= 2f;
            Main.dust[index3].noGravity = true;

            projectile.rotation += 0.4f;

            if (++projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                projectile.timeLeft = 0;
                projectile.position = projectile.Center;
                projectile.width = 500;
                projectile.height = 500;
                projectile.Center = projectile.position;
                projectile.penetrate = -1;
                projectile.Damage();
            }

            //if (!Main.dedServ && Main.LocalPlayer.active) Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;
            
            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 3f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 21f * projectile.scale;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index3].velocity *= 12f;
                Main.dust[index3].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.timeLeft > 0)
                projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}