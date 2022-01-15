using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearThrown : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        //throw with 25 velocity, 1000 damage, 10 knockback

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 180;
            projectile.extraUpdates = 1;
            projectile.scale = 1.3f;
            projectile.alpha = 0;
            projectile.ranged = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        float scaletimer;
        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (--projectile.localAI[0] < 0)
            {
                projectile.localAI[0] = 3;
                if (projectile.owner == Main.myPlayer)
                {
                    Vector2 baseVel = Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 2);

                    int p = Projectile.NewProjectile(projectile.Center, 16f * baseVel, 
                        ModContent.ProjectileType<PhantasmalSphere>(), projectile.damage, projectile.knockBack / 2, projectile.owner, 1f);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].melee = false;
                        Main.projectile[p].ranged = projectile.ranged;
                    }
                    p = Projectile.NewProjectile(projectile.Center, 16f * -baseVel,
                        ModContent.ProjectileType<PhantasmalSphere>(), projectile.damage, projectile.knockBack / 2, projectile.owner, 1f);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].melee = false;
                        Main.projectile[p].ranged = projectile.ranged;
                    }
                    
                    /*Vector2 vel = baseVel.RotatedByRandom(Math.PI / 4);
                    Projectile.NewProjectile(projectile.Center, 8f * vel, ModContent.ProjectileType<HentaiSpearArc>(), 
                        projectile.damage, projectile.knockBack / 2, projectile.owner, vel.ToRotation(), projectile.timeLeft / projectile.MaxUpdates);
                    vel = -baseVel.RotatedByRandom(Math.PI / 4);
                    Projectile.NewProjectile(projectile.Center, 8f * vel, ModContent.ProjectileType<HentaiSpearArc>(),
                        projectile.damage, projectile.knockBack / 2, projectile.owner, vel.ToRotation(), projectile.timeLeft / projectile.MaxUpdates);*/
                }
            }

            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1f;
                Main.PlaySound(SoundID.Item1, projectile.Center);
                if (projectile.owner == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(projectile.Center, Vector2.Normalize(projectile.velocity), 
                        ModContent.ProjectileType<HentaiSpearDeathray>(), projectile.damage, projectile.knockBack, 
                        projectile.owner, 0f, projectile.velocity.Length() * projectile.MaxUpdates);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].melee = false;
                        Main.projectile[p].ranged = projectile.ranged;
                    }
                }
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            scaletimer++;
        }

        public override bool CanDamage()
        {
            projectile.maxPenetrate = 1;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)), Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), projectile.damage, 0f, projectile.owner);
                if (p != Main.maxProjectiles)
                {
                    Main.projectile[p].melee = false;
                    Main.projectile[p].ranged = projectile.ranged;
                }
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glow = mod.GetTexture("Projectiles/MutantBoss/MutantEye_Glow");
            int rect1 = glow.Height / Main.projFrames[projectile.type];
            int rect2 = rect1 * projectile.frame;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(51, 255, 191, 0), Color.Transparent, 0.82f);
            Color glowcolor2 = Color.Lerp(new Color(194, 255, 242, 0), Color.Transparent, 0.6f);
            glowcolor = Color.Lerp(glowcolor, glowcolor2, 0.5f + (float)Math.Sin(scaletimer / 7) / 2); //make it shift between the 2 colors
            Vector2 drawCenter = projectile.Center + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 28);

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the projectile
            {
                Vector2 drawCenter2 = drawCenter + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 20).RotatedBy(MathHelper.Pi / 5 - (i * MathHelper.Pi / 5)); //use a normalized version of the projectile's velocity to offset it at different angles
                drawCenter2 -= (projectile.velocity.SafeNormalize(Vector2.UnitX) * 20); //then move it backwards
                float scale = projectile.scale;
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Main.spriteBatch.Draw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.25f, SpriteEffects.None, 0f);
            }

            for (int i = ProjectileID.Sets.TrailCacheLength[projectile.type] - 1; i > 0; i--) //scaling trail
            {

                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Vector2 value4 = projectile.oldPos[i] - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);
                Main.spriteBatch.Draw(glow, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.25f, SpriteEffects.None, 0f);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}