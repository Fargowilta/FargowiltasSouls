using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EridanusMinion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Champions/CosmosChampion";

        public int drawTrailOffset;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus");
            Main.projFrames[Projectile.type] = 9;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 75;
            Projectile.height = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.alpha = 0;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead && Main.player[Projectile.owner].GetModPlayer<FargoSoulsPlayer>().EridanusEmpower)
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Player player = Main.player[Projectile.owner];

            NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy())
            {
                Projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                Projectile.netUpdate = true;
            }

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }

            Projectile.rotation = 0;

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];

                if (npc.CanBeChasedBy() && player.Distance(Projectile.Center) < 2500 && Projectile.Distance(npc.Center) < 2500)
                {
                    Projectile.direction = Projectile.spriteDirection = Projectile.Center.X < npc.Center.X ? 1 : -1;

                    switch (player.GetModPlayer<FargoSoulsPlayer>().EridanusTimer / (60 * 10)) //attack according to current class
                    {
                        case 0: //melee
                            {
                                float length = player.Distance(npc.Center) - 300;
                                if (length > 300)
                                    length = 300;
                                Vector2 home = player.Center + player.DirectionTo(npc.Center) * length;
                                Projectile.Center = Vector2.Lerp(Projectile.Center, home, 0.15f);
                                Projectile.velocity *= 0.8f;

                                if (++Projectile.localAI[0] > 5) //spam close range fists
                                {
                                    Projectile.localAI[0] = 0;
                                    if (Main.myPlayer == Projectile.owner && player.HeldItem.DamageType == DamageClass.Melee)
                                    {
                                        const float maxRange = 700;
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.DirectionTo(npc.Center) * 40, 16f * Projectile.DirectionTo(npc.Center).RotatedByRandom(MathHelper.ToRadians(15)),
                                            ModContent.ProjectileType<EridanusFist>(), (int)(Projectile.originalDamage * Main.player[Projectile.owner].GetDamage(DamageClass.Melee).Additive / 3), Projectile.knockBack / 2, Main.myPlayer, maxRange);
                                    }
                                }

                                Projectile.frame = player.HeldItem.DamageType == DamageClass.Melee ? 6 : 5;
                                Projectile.rotation = Projectile.DirectionTo(npc.Center).ToRotation();
                                if (Projectile.spriteDirection < 0)
                                    Projectile.rotation += (float)Math.PI;
                            }
                            break;

                        case 1: //ranged
                            {
                                Vector2 home = player.Center;
                                home.X -= 50 * player.direction;
                                home.Y -= 40;
                                Projectile.Center = Vector2.Lerp(Projectile.Center, home, 0.15f);
                                Projectile.velocity *= 0.8f;

                                if (++Projectile.localAI[0] > 65) //shoot giant homing bullet
                                {
                                    Projectile.localAI[0] = 0;
                                    if (Main.myPlayer == Projectile.owner && player.HeldItem.DamageType == DamageClass.Ranged)
                                    {
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 12f * Projectile.DirectionTo(npc.Center), ModContent.ProjectileType<EridanusBullet>(),
                                            (int)(Projectile.originalDamage * Main.player[Projectile.owner].GetDamage(DamageClass.Ranged).Additive * 1.5f), Projectile.knockBack * 2, Main.myPlayer, npc.whoAmI);
                                    }
                                }

                                if (player.HeldItem.DamageType == DamageClass.Ranged)
                                {
                                    if (Projectile.localAI[0] < 15)
                                        Projectile.frame = 8;
                                    else if (Projectile.localAI[0] > 50)
                                        Projectile.frame = 7;
                                }
                            }
                            break;

                        case 2: //magic
                            {
                                Vector2 home = player.Center + (npc.Center - player.Center) / 3;
                                Projectile.Center = Vector2.Lerp(Projectile.Center, home, 0.15f);
                                Projectile.velocity *= 0.8f;

                                if (player.HeldItem.DamageType == DamageClass.Magic && Projectile.localAI[0] > 45)
                                    Projectile.frame = 7;

                                if (++Projectile.localAI[0] > 60)
                                {
                                    if (Projectile.localAI[0] > 90)
                                        Projectile.localAI[0] = 0;

                                    if (player.HeldItem.DamageType == DamageClass.Magic)
                                        Projectile.frame = 8;

                                    if (Projectile.localAI[0] % 5 == 0 && player.HeldItem.DamageType == DamageClass.Magic) //rain lunar flares
                                    {
                                        SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

                                        if (Main.myPlayer == Projectile.owner)
                                        {
                                            Vector2 spawnPos = Projectile.Center;
                                            spawnPos.X += Main.rand.NextFloat(-250, 250);
                                            spawnPos.Y -= 600f;

                                            Vector2 vel = 10f * npc.DirectionFrom(spawnPos);

                                            spawnPos += npc.velocity * Main.rand.NextFloat(10f);

                                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileID.LunarFlare,
                                                (int)(Projectile.originalDamage * player.GetDamage(DamageClass.Magic).Additive / 2), Projectile.knockBack / 2, Main.myPlayer, 0, npc.Center.Y);
                                        }
                                    }
                                }
                            }
                            break;

                        default: //minion
                            {
                                Vector2 home = npc.Center;
                                home.X += 350 * Math.Sign(player.Center.X - npc.Center.X);
                                if (Projectile.Distance(home) > 50)
                                    Movement(home, 0.8f, 32f);

                                Projectile.frame = 5;

                                bool playerIsAttacking = player.controlUseItem
                                    && (player.HeldItem.DamageType == DamageClass.Melee
                                    || player.HeldItem.DamageType == DamageClass.Ranged
                                    || player.HeldItem.DamageType == DamageClass.Magic
                                    || player.HeldItem.DamageType == DamageClass.Throwing)
                                    && player.HeldItem.pick == 0 && player.HeldItem.axe == 0 && player.HeldItem.hammer == 0;

                                if (++Projectile.localAI[0] > 15)
                                {
                                    Projectile.localAI[0] = 0;
                                    if (Main.myPlayer == Projectile.owner && !playerIsAttacking)
                                    {
                                        int modifier = Math.Sign(Projectile.Center.Y - npc.Center.Y);
                                        FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center + 3000 * Projectile.DirectionFrom(npc.Center) * modifier,
                                            Projectile.DirectionTo(npc.Center) * modifier, ModContent.ProjectileType<EridanusDeathray>(),
                                            Projectile.originalDamage, Projectile.knockBack / 4, Main.myPlayer);
                                    }
                                }

                                if (!playerIsAttacking && Projectile.localAI[0] < 7)
                                    Projectile.frame = 6;

                                Projectile.rotation = Projectile.DirectionTo(npc.Center).ToRotation();
                                if (Projectile.spriteDirection < 0)
                                    Projectile.rotation += (float)Math.PI;
                            }
                            break;
                    }
                }
                else //forget target
                {
                    Projectile.ai[0] = -1f;
                    Projectile.localAI[0] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Projectile.localAI[0] = 0f;

                Vector2 home = player.Center;
                home.X -= 50 * player.direction;
                home.Y -= 40;

                Projectile.direction = Projectile.spriteDirection = player.direction;

                if (Projectile.Distance(home) > 2000f)
                {
                    Projectile.Center = player.Center;
                    Projectile.velocity = Vector2.Zero;
                }
                else
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, home, 0.25f);
                    Projectile.velocity *= 0.8f;
                }

                if (++Projectile.localAI[1] > 6f)
                {
                    Projectile.localAI[1] = 0f;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1500);
                    Projectile.netUpdate = true;
                }
            }

            if (++drawTrailOffset > 2)
                drawTrailOffset = 0;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X -= speedModifier * 2;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(Projectile.velocity.X) > cap)
                Projectile.velocity.X = cap * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > cap)
                Projectile.velocity.Y = cap * Math.Sign(Projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<Buffs.Masomode.CurseoftheMoon>(), 360);

        public override bool? CanCutTiles() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTex = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/CosmosChampion_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D glowerTex = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/CosmosChampion_Glow2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int size = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = size * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, projTex.Width, size);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects flipper = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color projColor = Projectile.GetAlpha(lightColor);
            int add = 150;
            Color glowColor = new Color(add + Main.DiscoR / 3, add + Main.DiscoG / 3, add + Main.DiscoB / 3);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (i % 2 == (drawTrailOffset > 1 ? 1 : 0))
                    continue;

                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(glowTex, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor * 0.5f, num165, origin2, Projectile.scale, flipper, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.EntitySpriteDraw(projTex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projColor, Projectile.rotation, origin2, Projectile.scale, flipper, 0);
            Main.EntitySpriteDraw(glowerTex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, flipper, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.EntitySpriteDraw(glowerTex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, flipper, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}