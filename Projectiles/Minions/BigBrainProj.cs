using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class BigBrainProj : ModProjectile
    {
        public const int MaxMinionSlots = 7;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Big Brain");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[base.projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.alpha = 0;
            /*projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;*/
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.dead) modPlayer.BigBrainMinion = false;
            if (modPlayer.BigBrainMinion) projectile.timeLeft = 2;

            projectile.frameCounter++;
            if (projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 12;
            }

            float slotsModifier = Math.Min(projectile.minionSlots / MaxMinionSlots, 1f);

            projectile.ai[0] += 0.1f + 0.3f * slotsModifier;
            projectile.alpha = (int)(Math.Cos(projectile.ai[0] / 0.4f * MathHelper.TwoPi / 180) * 60) + 60;

            float oldScale = projectile.scale;
            projectile.scale = 0.75f + 0.5f * slotsModifier;

            projectile.position = projectile.Center;
            projectile.width = (int)(projectile.width * projectile.scale / oldScale);
            projectile.height = (int)(projectile.height * projectile.scale / oldScale);
            projectile.Center = projectile.position;

            NPC targetnpc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000, center: Main.player[projectile.owner].MountedCenter));
            bool targetting = targetnpc != null; //targetting code, prioritize targetted npcs, then look for closest if none is found

            if (targetting)
            {
                if (++projectile.localAI[0] > 5)
                {
                    projectile.localAI[0] = 0;
                    
                    if (projectile.owner == Main.myPlayer)
                    {
                        const float speed = 18f;
                        int damage = (int)(projectile.damage * projectile.scale); //damage directly proportional to projectile scale, change later???
                        int type = ModContent.ProjectileType<BigBrainIllusion>();

                        //Vector2 spawnpos = targetnpc.Center + Main.rand.NextVector2CircularEdge(150, 150);
                        //Projectile.NewProjectile(spawnpos, speed * Vector2.Normalize(targetnpc.Center - spawnpos), type, damage, projectile.knockBack, projectile.owner, projectile.scale);

                        Vector2 spawnFromMe = Main.player[projectile.owner].Center + (projectile.Center - Main.player[projectile.owner].Center).RotatedBy(MathHelper.TwoPi / 4 * Main.rand.Next(4));
                        Vector2 vel = speed * Vector2.Normalize(targetnpc.Center + targetnpc.velocity * 15 - spawnFromMe);
                        Projectile.NewProjectile(spawnFromMe, vel, type, damage, projectile.knockBack, projectile.owner, projectile.scale);
                    }
                }
            }

            projectile.Center = player.Center + new Vector2(0, (200 + projectile.alpha) * projectile.scale).RotatedBy(projectile.ai[1] + projectile.ai[0]/MathHelper.TwoPi);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int) (damage * projectile.scale);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for(int i = 0; i <= 3; i++) //simulate collision of 4 projectiles orbiting because i didnt want to make orbiting illusions seperate projectiles, also makes collision with scale changes better
            {
                Player player = Main.player[projectile.owner];
                Vector2 newCenter = player.Center + new Vector2(0, (200 + projectile.alpha) * projectile.scale).RotatedBy((i * MathHelper.PiOver2) + projectile.ai[1] + projectile.ai[0] / MathHelper.TwoPi);
                int width = (int)(projectile.scale * projectile.width);
                int height = (int)(projectile.scale * projectile.height);
                Rectangle newprojhitbox = new Rectangle((int)newCenter.X - width/2, (int)newCenter.Y - height/2, width, height);
                if (newprojhitbox.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameheight = texture.Height / Main.projFrames[projectile.type];
            Rectangle rectangle = new Rectangle(0, projectile.frame * frameheight, texture.Width, frameheight);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, rectangle.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            for(int i = 1; i <= 3; i++)
            {
                Player player = Main.player[projectile.owner];
                Vector2 newCenter = player.Center + new Vector2(0, (200 + projectile.alpha) * projectile.scale).RotatedBy((i * MathHelper.PiOver2) + projectile.ai[1] + projectile.ai[0] / MathHelper.TwoPi);
                Color newcolor = lightColor * projectile.Opacity * projectile.Opacity * projectile.Opacity * (Main.mouseTextColor / 255f);

                Main.spriteBatch.Draw(texture, newCenter - Main.screenPosition, new Rectangle?(rectangle), newcolor, projectile.rotation, rectangle.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}