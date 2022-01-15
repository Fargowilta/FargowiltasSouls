using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class Probe2 : ModProjectile
    {
        public override string Texture => "Terraria/NPC_139";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Probe");
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.timeLeft *= 5;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().Probes)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
            {
                projectile.damage = (int)(35f * player.minionDamage);
                if (player.GetModPlayer<FargoPlayer>().MasochistSoul)
                    projectile.damage *= 3;
            }

            projectile.ai[0] -= (float)Math.PI / 60f;
            projectile.Center = player.Center + new Vector2(-60, 0).RotatedBy(projectile.ai[0]);

            if (projectile.ai[1] >= 0f && projectile.ai[1] < Main.maxNPCs)
            {
                NPC npc = Main.npc[(int)projectile.ai[1]];
                projectile.rotation = (npc.Center - projectile.Center).ToRotation();
                if (npc.CanBeChasedBy() && Collision.CanHitLine(npc.Center, 0, 0, Main.player[projectile.owner].Center, 0, 0))
                {
                    if (--projectile.localAI[0] < 0f)
                    {
                        projectile.localAI[0] = player.GetModPlayer<FargoPlayer>().MasochistSoul ? 15f : 30f;
                        if (projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(projectile.Center, new Vector2(8f, 0f).RotatedBy(projectile.rotation),
                                ModContent.ProjectileType<ProbeLaser>(), projectile.damage, projectile.knockBack, projectile.owner);
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    projectile.ai[1] = -1f;
                    projectile.netUpdate = true;
                }
                projectile.rotation += (float)Math.PI;
            }
            else
            {
                if (projectile.owner == Main.myPlayer)
                    projectile.rotation = (Main.MouseWorld - projectile.Center).ToRotation() + (float)Math.PI;
            }

            if (++projectile.localAI[1] > 20f)
            {
                projectile.localAI[1] = 0f;
                projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000f, true);
                projectile.netUpdate = true;
            }
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