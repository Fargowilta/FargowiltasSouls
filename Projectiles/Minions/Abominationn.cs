using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class Abominationn : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Eternals/AbominationnSoul";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 50;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.netImportant = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            projectile.scale = 1;

            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().Abominationn)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
                projectile.damage = (int)(900f * player.minionDamage);

            if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
                    projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc.CanBeChasedBy(projectile))
                {
                    projectile.direction = projectile.spriteDirection = projectile.Center.X > npc.Center.X ? 1 : -1;
                    Vector2 targetPos = npc.Center + projectile.DirectionFrom(npc.Center) * 250;
                    if (projectile.Distance(targetPos) > 50)
                        Movement(targetPos, 1f);
                    if (++projectile.localAI[0] > 15)
                    {
                        projectile.localAI[0] = 0;
                        if (projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(projectile.Center, projectile.velocity + projectile.DirectionTo(npc.Center) * 30, mod.ProjectileType("AbominationnScythe"), projectile.damage, projectile.knockBack / 2, projectile.owner, npc.whoAmI);
                    }
                }
                else //forget target
                {
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500);
                    projectile.netUpdate = true;
                }
            }
            else //no target
            {
                projectile.direction = projectile.spriteDirection = projectile.Center.X > player.Center.X ? 1 : -1;

                Vector2 targetPos = player.Center;
                if (player.velocity.X > 0)
                    targetPos.X -= 100;
                else if (player.velocity.X < 0)
                    targetPos.X += 100;
                else
                    targetPos.X += 100 * -player.direction;
                targetPos.Y -= 50;

                if (projectile.Distance(targetPos) > 3000)
                    projectile.Center = player.Center;
                else if (projectile.Distance(targetPos) > 50)
                    Movement(targetPos, 1f);

                if (++projectile.localAI[1] > 6)
                {
                    projectile.localAI[1] = 0;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500);
                    if (projectile.ai[0] != -1)
                        projectile.netUpdate = true;
                }
            }

            if (++projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 4)
                    projectile.frame = 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier)
        {
            if (projectile.Center.X < targetPos.X)
            {
                projectile.velocity.X += speedModifier;
                if (projectile.velocity.X < 0)
                    projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                projectile.velocity.X -= speedModifier;
                if (projectile.velocity.X > 0)
                    projectile.velocity.X -= speedModifier * 2;
            }
            if (projectile.Center.Y < targetPos.Y)
            {
                projectile.velocity.Y += speedModifier;
                if (projectile.velocity.Y < 0)
                    projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                projectile.velocity.Y -= speedModifier;
                if (projectile.velocity.Y > 0)
                    projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(projectile.velocity.X) > 24)
                projectile.velocity.X = 24 * Math.Sign(projectile.velocity.X);
            if (Math.Abs(projectile.velocity.Y) > 24)
                projectile.velocity.Y = 24 * Math.Sign(projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("MutantNibble"), 600);
            target.AddBuff(BuffID.ShadowFlame, 600);
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
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity * 0.75f;
        }
    }
}
