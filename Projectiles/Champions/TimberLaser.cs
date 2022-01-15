using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TimberLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 4;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            cooldownSlot = 1;

            projectile.scale = 2f;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampionHead>());
            if (npc != null)
            {
                if (projectile.Distance(npc.Center) < projectile.ai[1])
                {
                    Main.PlaySound(SoundID.NPCHit4, projectile.Center); //indicate it hit squrrl
                    for (int i = 0; i < 10; i++)
                        Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, projectile.velocity.X * 0.4f, -projectile.velocity.Y * 0.4f);
                    
                    projectile.Kill();
                    return;
                }
            }
            else
            {
                projectile.Kill();
                return;
            }

            if (projectile.alpha > 0)
            {
                projectile.alpha -= 10;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            Lighting.AddLight(projectile.Center, 0.525f, 0f, 0.75f);
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Guilty>(), 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.alpha < 200)
                return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
            return Color.Transparent;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}