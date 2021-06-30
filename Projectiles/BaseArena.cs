using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public abstract class BaseArena : ModProjectile
    {
        protected float rotationPerTick;
        protected readonly float npcType;
        protected readonly int dustType;
        protected readonly int increment;
        protected float threshold;
        protected float targetPlayer;

        protected BaseArena(float rotationPerTick, float threshold, int npcType, int dustType = 135, int increment = 2)
        {
            this.rotationPerTick = rotationPerTick;
            this.threshold = threshold;
            this.npcType = npcType;
            this.dustType = dustType;
            this.increment = increment;
        }

        public override void SetDefaults() //MAKE SURE YOU CALL BASE.SETDEFAULTS IF OVERRIDING
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;

            cooldownSlot = 0;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCheck =
                projectile =>
                {
                    return CanDamage() && targetPlayer == Main.myPlayer && Math.Abs((Main.LocalPlayer.Center - projectile.Center).Length() - threshold) < projectile.width / 2 * projectile.scale + Player.defaultHeight + 100;
                };

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;

            projectile.hide = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToMutantBomb = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override bool CanHitPlayer(Player target)
        {
            return targetPlayer == target.whoAmI && target.hurtCooldowns[cooldownSlot] == 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Math.Abs((targetHitbox.Center.ToVector2() - projHitbox.Center.ToVector2()).Length() - threshold) < projectile.width / 2 * projectile.scale;
        }

        protected virtual void Movement(NPC npc)
        {
            //this can also be used for general npc-reliant checks and killing the proj
        }

        public override void AI()
        {
            int ai1 = (int)projectile.ai[1];
            if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active && Main.npc[ai1].type == npcType)
            {
                projectile.alpha -= increment;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                Movement(Main.npc[ai1]);

                targetPlayer = Main.npc[ai1].target;

                Player player = Main.LocalPlayer;
                if (player.active && !player.dead && !player.ghost)
                {
                    float distance = player.Distance(projectile.Center);
                    if (distance > threshold && distance < threshold * 4f)
                    {
                        if (distance > threshold * 2f)
                        {
                            player.controlLeft = false;
                            player.controlRight= false;
                            player.controlUp = false;
                            player.controlDown = false;
                            player.controlUseItem = false;
                            player.controlUseTile = false;
                            player.controlJump = false;
                            player.controlHook = false;
                            if (player.mount.Active)
                                player.mount.Dismount(player);
                            player.velocity.X = 0f;
                            player.velocity.Y = -0.4f;
                        }

                        Vector2 movement = projectile.Center - player.Center;
                        float difference = movement.Length() - threshold;
                        movement.Normalize();
                        movement *= difference < 17f ? difference : 17f;
                        player.position += movement;

                        for (int i = 0; i < 20; i++)
                        {
                            int d = Dust.NewDust(player.position, player.width, player.height, dustType, 0f, 0f, 0, default(Color), 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 5f;
                        }
                    }
                }
            }
            else
            {
                projectile.velocity = Vector2.Zero;
                projectile.alpha += increment;
                if (projectile.alpha > 255)
                {
                    projectile.Kill();
                    return;
                }
            }

            projectile.timeLeft = 2;
            projectile.scale = (1f - projectile.alpha / 255f) * 2f;
            projectile.ai[0] += rotationPerTick;
            if (projectile.ai[0] > MathHelper.Pi)
            {
                projectile.ai[0] -= 2f * MathHelper.Pi;
                projectile.netUpdate = true;
            }
            else if (projectile.ai[0] < -MathHelper.Pi)
            {
                projectile.ai[0] += 2f * MathHelper.Pi;
                projectile.netUpdate = true;
            }

            projectile.localAI[0] = threshold;
        }

        public override void PostAI()
        {
            projectile.hide = false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity = target.DirectionTo(projectile.Center) * 4f;
        }

        public override void Kill(int timeLeft)
        {
            float modifier = (255f - projectile.alpha) / 255f;
            float offset = threshold * modifier;
            int max = (int)(300 * modifier);
            for (int i = 0; i < max; i++)
            {
                int d = Dust.NewDust(projectile.Center, 0, 0, dustType, Scale: 4f);
                Main.dust[d].velocity *= 6f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = projectile.Center + offset * Vector2.UnitX.RotatedByRandom(2 * Math.PI);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity * (targetPlayer == Main.myPlayer ? 1f : 0.15f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw

            Color color26 = projectile.GetAlpha(lightColor);

            for (int x = 0; x < 32; x++)
            {
                int frame = (projectile.frame + x) % Main.projFrames[projectile.type];
                int y3 = num156 * frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;

                float rotation = 2f * MathHelper.Pi / 32 * x + projectile.ai[0];

                Vector2 drawOffset = new Vector2(threshold * projectile.scale / 2f, 0f).RotatedBy(projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * MathHelper.Pi / 32f * x);
                const int max = 4;
                for (int i = 0; i < max; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(max - i) / max;
                    Vector2 value4 = projectile.Center + drawOffset.RotatedBy(rotationPerTick * -i);
                    float rot = rotation + projectile.rotation;
                    Main.spriteBatch.Draw(texture2D13, value4 - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, rot, origin2, projectile.scale, SpriteEffects.None, 0f);
                }

                float finalRot = rotation + projectile.rotation;
                Main.spriteBatch.Draw(texture2D13, projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, finalRot, origin2, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}
