using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TerraLightningOrb : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_465";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.scale = 0.5f;
            cooldownSlot = 1;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override void AI()
        {
            projectile.velocity = Vector2.Zero;

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.TerraChampion>());
            if (npc != null)
            {
                projectile.alpha -= 10;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                /*if (projectile.Distance(Main.player[Main.npc[ai0].target].Center) > 600)
                {
                    projectile.velocity = 3f * projectile.DirectionTo(Main.player[Main.npc[ai0].target].Center);
                }*/
            }
            else
            {
                projectile.alpha += 10;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }

            projectile.timeLeft = 2;

            Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                    projectile.frame = 0;
            }

            if (Main.rand.NextBool(3))
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-projectile.width * 0.2f * projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = projectile.Center + vector21 * projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-projectile.width * 0.6f * projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = projectile.Center + vector2 * projectile.scale;
                Main.dust[index2].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}