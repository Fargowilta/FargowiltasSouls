using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class BlenderOrbital : ModProjectile
    {
        public int Counter = 0;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Blender");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;

            projectile.extraUpdates = 1;
            projectile.tileCollide = false;

            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 15;

            projectile.aiStyle = -1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        int soundtimer;
        public override void AI()
        {
            int byUUID = FargoSoulsUtil.GetByUUIDReal(projectile.owner, (int)projectile.localAI[0], ModContent.ProjectileType<BlenderYoyoProj>());
            if (byUUID == -1)
            {
                if (projectile.owner == Main.myPlayer && projectile.rotation > 0)
                {
                    projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile proj = Main.projectile[byUUID];

                //rotation mumbo jumbo
                float distanceFromPlayer = 150 + 150 * (1 - (float)Math.Cos(projectile.localAI[1])); // + projectile.ai[0] * 32;

                projectile.position = proj.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(projectile.ai[1]);
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;

                float rotation = MathHelper.Pi / 20 / projectile.MaxUpdates * 0.75f;
                projectile.ai[1] += rotation;
                if (projectile.ai[1] > MathHelper.Pi)
                {
                    projectile.ai[1] -= MathHelper.TwoPi;
                    projectile.netUpdate = true;
                }

                projectile.localAI[1] += MathHelper.TwoPi / 120 / projectile.MaxUpdates; // * (0.5f + 1.5f * projectile.ai[0] / 5);
                if (projectile.localAI[1] > MathHelper.Pi)
                    projectile.localAI[1] -= MathHelper.TwoPi;

                projectile.damage = (int)(proj.damage * 0.5);
                projectile.knockBack = proj.knockBack;
            }

            projectile.timeLeft++;
            projectile.rotation += 0.25f / projectile.MaxUpdates;

            if (soundtimer > 0)
                soundtimer--;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[projectile.owner] = 6;

            /*Vector2 velocity = Vector2.Normalize(projectile.Center - target.Center) * 10;
            int proj2 = mod.ProjectileType("BlenderProj3");
            Projectile.NewProjectile(new Vector2(projectile.Center.X, projectile.Center.Y), velocity, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);*/

            if (soundtimer == 0)
            {
                soundtimer = 15;
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 22, 1.5f, 1f);
            }
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}