using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EridanusMinion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Champions/CosmosChampion";

        public const int baseDamage = 300;
        public int drawTrailOffset;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus");
            Main.projFrames[projectile.type] = 9;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 75;
            projectile.height = 100;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.alpha = 0;
            projectile.minionSlots = 0f;
            projectile.penetrate = -1;
            projectile.netImportant = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead && Main.player[projectile.owner].GetModPlayer<FargoPlayer>().EridanusEmpower)
            {
                projectile.timeLeft = 2;
            }
            else
            {
                projectile.Kill();
                return;
            }

            projectile.damage = (int)(baseDamage * Main.player[projectile.owner].minionDamage);

            Player player = Main.player[projectile.owner];

            NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
            {
                projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                projectile.netUpdate = true;
            }

            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
            }
            if (projectile.frame > 4)
            {
                projectile.frame = 0;
            }

            projectile.rotation = 0;

            if (projectile.ai[0] >= 0 && projectile.ai[0] < 200) //has target
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];

                if (npc.CanBeChasedBy(projectile) && player.Distance(projectile.Center) < 2500 && projectile.Distance(npc.Center) < 2500)
                {
                    projectile.direction = projectile.spriteDirection = projectile.Center.X < npc.Center.X ? 1 : -1;

                    switch (player.GetModPlayer<FargoPlayer>().EridanusTimer / (60 * 10)) //attack according to current class
                    {
                        case 0: //melee
                            {
                                float length = player.Distance(npc.Center) - 300;
                                if (length > 300)
                                    length = 300;
                                Vector2 home = player.Center + player.DirectionTo(npc.Center) * length;
                                projectile.Center = Vector2.Lerp(projectile.Center, home, 0.15f);
                                projectile.velocity *= 0.8f;

                                if (++projectile.localAI[0] > 5) //spam close range fists
                                {
                                    projectile.localAI[0] = 0;
                                    if (Main.myPlayer == projectile.owner && player.HeldItem.melee)
                                    {
                                        const float maxRange = 700;
                                        Projectile.NewProjectile(projectile.Center + projectile.DirectionTo(npc.Center) * 40, 16f * projectile.DirectionTo(npc.Center).RotatedByRandom(MathHelper.ToRadians(15)),
                                            ModContent.ProjectileType<EridanusFist>(), (int)(baseDamage * Main.player[projectile.owner].meleeDamage / 3), projectile.knockBack / 2, Main.myPlayer, maxRange);
                                    }
                                }

                                projectile.frame = player.HeldItem.melee ? 6 : 5;
                                projectile.rotation = projectile.DirectionTo(npc.Center).ToRotation();
                                if (projectile.spriteDirection < 0)
                                    projectile.rotation += (float)Math.PI;
                            }
                            break;

                        case 1: //ranged
                            {
                                Vector2 home = player.Center;
                                home.X -= 50 * player.direction;
                                home.Y -= 40;
                                projectile.Center = Vector2.Lerp(projectile.Center, home, 0.15f);
                                projectile.velocity *= 0.8f;

                                if (++projectile.localAI[0] > 65) //shoot giant homing bullet
                                {
                                    projectile.localAI[0] = 0;
                                    if (Main.myPlayer == projectile.owner && player.HeldItem.ranged)
                                    {
                                        Projectile.NewProjectile(projectile.Center, 12f * projectile.DirectionTo(npc.Center), ModContent.ProjectileType<EridanusBullet>(),
                                            (int)(baseDamage * Main.player[projectile.owner].rangedDamage * 1.5f), projectile.knockBack * 2, Main.myPlayer, npc.whoAmI);
                                    }
                                }
                                
                                if (player.HeldItem.ranged)
                                {
                                    if (projectile.localAI[0] < 15)
                                        projectile.frame = 8;
                                    else if (projectile.localAI[0] > 50)
                                        projectile.frame = 7;
                                }
                            }
                            break;

                        case 2: //magic
                            {
                                Vector2 home = player.Center + (npc.Center - player.Center) / 3;
                                projectile.Center = Vector2.Lerp(projectile.Center, home, 0.15f);
                                projectile.velocity *= 0.8f;

                                if (player.HeldItem.magic && projectile.localAI[0] > 45)
                                    projectile.frame = 7;

                                if (++projectile.localAI[0] > 60)
                                {
                                    if (projectile.localAI[0] > 90)
                                        projectile.localAI[0] = 0;

                                    if (player.HeldItem.magic)
                                        projectile.frame = 8;

                                    if (projectile.localAI[0] % 5 == 0 && player.HeldItem.magic) //rain lunar flares
                                    {
                                        Main.PlaySound(SoundID.Item88, projectile.Center);

                                        if (Main.myPlayer == projectile.owner)
                                        {
                                            Vector2 spawnPos = projectile.Center;
                                            spawnPos.X += Main.rand.NextFloat(-250, 250);
                                            spawnPos.Y -= 600f;

                                            Vector2 vel = 10f * npc.DirectionFrom(spawnPos);

                                            spawnPos += npc.velocity * Main.rand.NextFloat(10f);

                                            Projectile.NewProjectile(spawnPos, vel, ProjectileID.LunarFlare,
                                                (int)(baseDamage * player.magicDamage / 2), projectile.knockBack / 2, Main.myPlayer, 0, npc.Center.Y);
                                        }
                                    }
                                }
                            }
                            break;

                        default: //minion
                            {
                                Vector2 home = npc.Center;
                                home.X += 350 * Math.Sign(player.Center.X - npc.Center.X);
                                if (projectile.Distance(home) > 50)
                                    Movement(home, 0.8f, 32f);

                                projectile.frame = 5;

                                bool okToAttack = (!player.HeldItem.melee && !player.HeldItem.ranged && !player.HeldItem.magic) || !player.controlUseItem;

                                if (++projectile.localAI[0] > 15)
                                {
                                    projectile.localAI[0] = 0;
                                    if (Main.myPlayer == projectile.owner && okToAttack)
                                    {
                                        int modifier = Math.Sign(projectile.Center.Y - npc.Center.Y);
                                        Projectile.NewProjectile(projectile.Center + 3000 * projectile.DirectionFrom(npc.Center) * modifier,
                                            projectile.DirectionTo(npc.Center) * modifier, ModContent.ProjectileType<EridanusDeathray>(), 
                                            projectile.damage, projectile.knockBack / 4, Main.myPlayer);
                                    }
                                }

                                if (okToAttack && projectile.localAI[0] < 7)
                                    projectile.frame = 6;

                                projectile.rotation = projectile.DirectionTo(npc.Center).ToRotation();
                                if (projectile.spriteDirection < 0)
                                    projectile.rotation += (float)Math.PI;
                            }
                            break;
                    }
                }
                else //forget target
                {
                    projectile.ai[0] = -1f;
                    projectile.localAI[0] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else //no target
            {
                projectile.localAI[0] = 0f;

                Vector2 home = player.Center;
                home.X -= 50 * player.direction;
                home.Y -= 40;

                projectile.direction = projectile.spriteDirection = player.direction;
                
                if (projectile.Distance(home) > 2000f)
                {
                    projectile.Center = player.Center;
                    projectile.velocity = Vector2.Zero;
                }
                else
                {
                    projectile.Center = Vector2.Lerp(projectile.Center, home, 0.25f);
                    projectile.velocity *= 0.8f;
                }
                
                if (++projectile.localAI[1] > 6f)
                {
                    projectile.localAI[1] = 0f;

                    float maxDistance = 1500f;
                    int possibleTarget = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile))// && Collision.CanHitLine(projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            float npcDistance = player.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                possibleTarget = i;
                            }
                        }
                    }

                    if (possibleTarget >= 0)
                    {
                        projectile.ai[0] = possibleTarget;
                        projectile.netUpdate = true;
                    }
                }
            }

            if (++drawTrailOffset > 2)
                drawTrailOffset = 0;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
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
                projectile.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (projectile.velocity.Y < 0)
                    projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                projectile.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (projectile.velocity.Y > 0)
                    projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(projectile.velocity.X) > cap)
                projectile.velocity.X = cap * Math.Sign(projectile.velocity.X);
            if (Math.Abs(projectile.velocity.Y) > cap)
                projectile.velocity.Y = cap * Math.Sign(projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<Buffs.Masomode.CurseoftheMoon>(), 360);

        public override bool? CanCutTiles() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D projTex = Main.projectileTexture[projectile.type];
            Texture2D glowTex = mod.GetTexture("NPCs/Champions/CosmosChampion_Glow");
            Texture2D glowerTex = mod.GetTexture("NPCs/Champions/CosmosChampion_Glow2");
            int size = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = size * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, projTex.Width, size);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects flipper = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color projColor = projectile.GetAlpha(lightColor);
            int add = 150;
            Color glowColor = new Color(add + Main.DiscoR / 3, add + Main.DiscoG / 3, add + Main.DiscoB / 3);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (i % 2 == (drawTrailOffset > 1 ? 1 : 0))
                    continue;

                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                spriteBatch.Draw(glowTex, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor * 0.5f, num165, origin2, projectile.scale, flipper, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(projTex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projColor, projectile.rotation, origin2, projectile.scale, flipper, 0f);
            spriteBatch.Draw(glowerTex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, projectile.rotation, origin2, projectile.scale, flipper, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(glowerTex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, projectile.rotation, origin2, projectile.scale, flipper, 0f);
           
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}