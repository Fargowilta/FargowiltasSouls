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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Brain");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[base.projectile.type] = true;
        }
        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 70;
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

            projectile.ai[0] += 0.4f;
            projectile.alpha = (int)(Math.Cos(projectile.ai[0] * MathHelper.TwoPi / 180) * 60) + 60;

            if (projectile.minionSlots <= 6) //projectile scale increases with minion slots consumed, caps at 6 slots
                projectile.scale = 0.75f + projectile.minionSlots / 12;
            else
                projectile.scale = 1.25f;

            bool targetting = false; //targetting code, prioritize targetted npcs, then look for closest if none is found
            NPC targetnpc = null;
            NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object)this, false))
            {
                Vector2 distancetotarget = minionAttackTargetNpc.Center - projectile.Center;
                if (distancetotarget.Length() < 1500)
                {
                    targetnpc = minionAttackTargetNpc;
                    targetting = true;
                }
            }
            else if (!targetting)
            {
                float distancemax = 1500;
                for (int index = 0; index < 200; ++index)
                {
                    if (Main.npc[index].CanBeChasedBy((object)this, false))
                    {
                        Vector2 distancetotarget = Main.npc[index].Center - projectile.Center;
                        if (distancetotarget.Length() < distancemax)
                        {
                            distancemax = distancetotarget.Length();
                            targetnpc = Main.npc[index];
                            targetting = true;
                        }
                    }
                }
            }

            if(targetting)
            {
                projectile.localAI[0]++;
                if (projectile.localAI[0] > 7)
                {
                    Vector2 spawnpos = targetnpc.Center + Main.rand.NextVector2CircularEdge(150, 150);
                    Main.PlaySound(SoundID.Item, (int)spawnpos.X, (int)spawnpos.Y, 104, 0.5f, -0.2f);
                    Vector2 totarget = Vector2.Normalize(targetnpc.Center - spawnpos);
                    int p = Projectile.NewProjectile(spawnpos, totarget * 12, mod.ProjectileType("BigBrainIllusion"), (int)(projectile.damage * projectile.scale), projectile.knockBack, projectile.owner); //damage directly proportional to projectile scale, change later???
                    if (p < 1000)
                    {
                        Main.projectile[p].scale = projectile.scale * 0.75f;
                        Main.projectile[p].netUpdate = true; //sync because randomized spawn position
                    }
                    projectile.localAI[0] = 0;
                }
            }

            projectile.Center = player.Center + new Vector2(0, (200 + projectile.alpha) * projectile.scale).RotatedBy(projectile.ai[1] + projectile.ai[0]/MathHelper.TwoPi);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int) (damage * projectile.scale);
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
                Color newcolor = Color.Lerp(lightColor, Color.Transparent, 0.85f);

                Main.spriteBatch.Draw(texture, newCenter - Main.screenPosition, new Rectangle?(rectangle), newcolor, projectile.rotation, rectangle.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}