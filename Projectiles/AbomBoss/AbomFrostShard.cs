using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFrostShard : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_349";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Frost Shard");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 1;
            aiType = ProjectileID.FrostShard;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.extraUpdates = 1;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 5)
                    projectile.frame = 0;
            }

            projectile.velocity.X *= 0.95f;
            projectile.position.Y += projectile.velocity.Y / 2;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>()))
            {
                if (projectile.position.Y > Main.npc[EModeGlobalNPC.abomBoss].Center.Y + (Main.npc[EModeGlobalNPC.abomBoss].localAI[3] == 1 ? 2000 : 1400))
                    projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 76, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale = 0.7f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(BuffID.Frozen, 60);
            }
            target.AddBuff(BuffID.Frostburn, 120);
            //target.AddBuff(BuffID.Chilled, 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}