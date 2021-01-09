using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PhantasmalBolt2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_462";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Bolt");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.PhantasmalBolt;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.hostile = true;
            projectile.extraUpdates = 3;

            projectile.timeLeft = 1200;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            int index = Dust.NewDust(projectile.Center, 0, 0, 229, 0.0f, 0.0f, 100, new Color(), 1f);
            Main.dust[index].noLight = true;
            Main.dust[index].noGravity = true;
            Main.dust[index].velocity = projectile.velocity;
            Main.dust[index].position -= Vector2.One * 4f;
            Main.dust[index].scale = 0.8f;
            if (++projectile.frameCounter >= 6 * 4) //projectile extra updates + 1
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 5)
                    projectile.frame = 0;
            }

            if (projectile.timeLeft % (projectile.extraUpdates + 1) == 0 && ++projectile.localAI[1] > 30) //only run once per tick
            {
                if (projectile.localAI[1] < 120) //accelerate
                {
                    projectile.velocity *= 1.025f;
                }

                if (projectile.localAI[1] < 120
                    && EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore)
                    && Main.npc[EModeGlobalNPC.moonBoss].HasValidTarget) //home
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.moonBoss].target].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.012f));
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 360);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - projectile.alpha / 255f);
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