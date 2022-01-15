using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SkeletronArmR : ModProjectile
    {
        public override string Texture => "Terraria/NPC_36";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Skeletron Hand");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 52;
            projectile.height = 52;
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

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().SkeletronArms)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
            {
                projectile.damage = 18;
                if (player.GetModPlayer<FargoPlayer>().SupremeDeathbringerFairy)
                    projectile.damage = 24;
                if (player.GetModPlayer<FargoPlayer>().MasochistSoul)
                    projectile.damage = 48;
                projectile.damage = (int)(projectile.damage * player.minionDamage);
            }

            //tentacle head movement (homing)
            Vector2 playerVel = player.position - player.oldPosition;
            projectile.position += playerVel;
            projectile.ai[0]++;
            if (projectile.ai[0] >= 0f)
            {
                Vector2 home = player.Center;
                home.X += 200f;
                home.Y -= 50f;
                Vector2 distance = home - projectile.Center;
                float range = distance.Length();
                distance.Normalize();
                if (projectile.ai[0] == 0f)
                {
                    if (range > 15f)
                    {
                        projectile.ai[0] = -1f; //if in fast mode, stay fast until back in range
                        if (range > 1300f)
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 3f + Main.rand.NextFloat(3f);
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    distance /= 8f;
                }
                if (range > 120f) //switch to fast return mode
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
                projectile.velocity += distance;
                if (range > 30f)
                    projectile.velocity *= 0.96f;

                if (projectile.ai[0] > 90f) //attack nearby enemy
                {
                    projectile.ai[0] = 20f;
                    NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 400));
                    if (npc != null)
                    {
                        projectile.velocity = npc.Center - projectile.Center;
                        projectile.velocity.Normalize();
                        projectile.velocity *= 16f;
                        projectile.velocity += npc.velocity / 2f;
                        projectile.velocity -= playerVel / 2f;
                        projectile.ai[0] *= -1f;
                    }
                    projectile.netUpdate = true;
                }
            }

            Vector2 angle = player.Center - projectile.Center;
            angle.X += 200f;
            angle.Y += 180f;
            projectile.rotation = (float)Math.Atan2(angle.Y, angle.X) + (float)Math.PI / 2f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = Math.Sign(target.Center.X - Main.player[projectile.owner].Center.X);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}