using FargowiltasSouls.Projectiles.Champions;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    public class EarthChampionHand : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Earth");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "大地英灵");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 130;
            NPC.defense = 80;
            NPC.lifeMax = 320000;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;

            NPC.trapImmune = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return NPC.localAI[3] == 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            NPC head = FargoSoulsUtil.NPCExists(NPC.ai[2], ModContent.NPCType<EarthChampion>());
            if (head == null)
            {
                NPC.life = 0;
                NPC.checkDead();
                NPC.active = false;
                return;
            }

            NPC.lifeMax = head.lifeMax;
            NPC.damage = head.damage;
            NPC.defDamage = head.defDamage;
            NPC.defense = head.defense;
            NPC.defDefense = head.defDefense;
            NPC.target = head.target;

            NPC.life = NPC.lifeMax;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            NPC.direction = NPC.spriteDirection = (int)NPC.ai[3];
            NPC.localAI[3] = 0;

            switch ((int)NPC.ai[0])
            {
                case -1: //healing
                    targetPos = head.Center;
                    targetPos.Y += head.height;
                    targetPos.X += head.width * NPC.ai[3] / 2;

                    if (NPC.ai[3] > 0)
                        NPC.rotation = (float)Math.PI / 4 - (float)Math.PI / 2;
                    else
                        NPC.rotation = (float)Math.PI / 4;

                    if (NPC.ai[1] == 120)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 8;
                            float baseRotation = MathHelper.TwoPi / max * Main.rand.NextFloat();
                            for (int i = 0; i < max; i++)
                            {
                                float rotation = baseRotation + MathHelper.TwoPi / max * (i + Main.rand.NextFloat(-0.5f, 0.5f));
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, rotation, 3);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 120) //clench fist as boss heals
                    {
                        Movement(targetPos, 0.6f, 32f);

                        NPC.localAI[3] = 1;

                        if (NPC.ai[3] > 0)
                            NPC.rotation = -(float)Math.PI / 4 - (float)Math.PI / 2;
                        else
                            NPC.rotation = -(float)Math.PI / 4 + (float)Math.PI;

                        if (NPC.ai[1] > 240)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        Movement(targetPos, 1.2f, 32f);
                    }
                    break;

                case 0: //float near head
                    NPC.noTileCollide = true;

                    targetPos = head.Center;
                    targetPos.Y += 250;
                    targetPos.X += 300 * -NPC.ai[3];
                    Movement(targetPos, 0.8f, 32f);

                    NPC.rotation = 0;

                    if (++NPC.ai[1] > 60)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 1: //dashes
                    if (++NPC.ai[1] < 0) //do nothing lol
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - (float)Math.PI / 2;
                        NPC.localAI[3] = 1;
                    }
                    else if (NPC.ai[1] < 75) //hover near a side
                    {
                        NPC.rotation = 0;

                        targetPos = player.Center + player.DirectionTo(NPC.Center) * 400;
                        if (NPC.ai[3] < 0 && targetPos.X < player.Center.X + 400) //stay on your original side
                            targetPos.X = player.Center.X + 400;
                        if (NPC.ai[3] > 0 && targetPos.X > player.Center.X - 400)
                            targetPos.X = player.Center.X - 400;

                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, head.localAI[2] == 1 ? 2.4f : 1.2f, 32f);

                        if (head.localAI[2] == 1)
                            NPC.position += player.velocity / 3f;
                    }
                    else if (NPC.ai[1] < 120) //prepare to dash, enable hitbox
                    {
                        if (head.localAI[2] == 1)
                            NPC.position += player.velocity / 10f;

                        NPC.localAI[3] = 1;
                        NPC.velocity *= NPC.localAI[2] == 1 ? 0.8f : 0.95f;
                        NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - (float)Math.PI / 2;
                    }
                    else if (NPC.ai[1] == 120) //dash
                    {
                        NPC.localAI[3] = 1;
                        NPC.velocity = NPC.DirectionTo(player.Center) * (head.localAI[2] == 1 ? 20 : 16);
                    }
                    else //while dashing
                    {
                        NPC.velocity *= 1.02f;

                        NPC.localAI[3] = 1;
                        NPC.rotation = NPC.velocity.ToRotation() - (float)Math.PI / 2;

                        for (int i = 0; i < 5; i++) //flame jet behind self
                        {
                            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, -NPC.velocity.X * 0.25f, -NPC.velocity.Y * 0.25f, Scale: 3f);
                            Main.dust[d].position -= Vector2.Normalize(NPC.velocity) * NPC.width / 2;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 4f;
                        }

                        //passed player, prepare another dash
                        if ((++NPC.localAI[1] > 60 && NPC.Distance(player.Center) > 1000) ||
                            (NPC.ai[3] > 0 ?
                            NPC.Center.X > Math.Min(head.Center.X, player.Center.X) + 300 : NPC.Center.X < Math.Max(head.Center.X, player.Center.X) - 300))
                        {
                            NPC.ai[1] = head.localAI[2] == 1 ? 15 : 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;

                            if (head.localAI[2] == 1 && FargoSoulsWorld.EternityMode) //explosion chain
                            {
                                //NPC.ai[1] -= 30;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (Vector2.Normalize(NPC.velocity) * 100), Vector2.Zero, ModContent.ProjectileType<EarthChainBlast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.velocity.ToRotation(), 7);
                            }

                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 0.1f;
                        }
                    }

                    if (++NPC.localAI[0] > 660) //proceed
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2:
                    goto case 0;

                case 3: //petal shots
                    if (NPC.ai[3] > 0)
                    {
                        targetPos = player.Center;
                        targetPos.Y += player.velocity.Y * 60;
                        targetPos.X = player.Center.X - 400;

                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.4f, 32f);
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.X += 400;
                        targetPos.Y += 600 * (float)Math.Sin(2 * Math.PI / 77 * NPC.ai[1]);

                        Movement(targetPos, 0.8f, 32f);
                    }

                    if (++NPC.localAI[0] > (head.localAI[2] == 1 ? 18 : 24))
                    {
                        NPC.localAI[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * NPC.ai[3], ModContent.ProjectileType<FlowerPetal>(),
                                FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, head.localAI[2] == 1 && FargoSoulsWorld.EternityMode ? 0 : 1);
                        }
                    }

                    NPC.position.X += NPC.velocity.X; //move faster horizontally

                    if (++NPC.ai[1] > 360)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //slam three times
                case 6:
                case 7:
                    if (++NPC.ai[1] < 90) //float over head
                    {
                        NPC.noTileCollide = true;

                        targetPos = head.Center;
                        targetPos.Y -= head.height;
                        targetPos.X += 50 * -NPC.ai[3];
                        Movement(targetPos, 2.0f, 32f);

                        NPC.rotation = 0;
                    }
                    else if (NPC.ai[1] == 90) //dash down
                    {
                        NPC.velocity = Vector2.UnitY * (head.localAI[2] == 1 ? 36 : 24);
                        NPC.localAI[0] = player.position.Y;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.localAI[3] = 1;

                        if (NPC.ai[3] > 0)
                            NPC.rotation = -(float)Math.PI / 2;
                        else
                            NPC.rotation = (float)Math.PI / 2;

                        if (NPC.position.Y + NPC.height > NPC.localAI[0]) //become solid to smash on tiles
                            NPC.noTileCollide = false;

                        //extra checks to prevent noclipping
                        if (!NPC.noTileCollide)
                        {
                            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height)
                                || NPC.position.Y + NPC.height > Main.maxTilesY * 16 - 16)
                                NPC.velocity.Y = 0;
                        }

                        if (NPC.velocity.Y == 0) //we've hit something
                        {
                            if (NPC.localAI[0] != 0)
                            {
                                NPC.localAI[0] = 0;

                                if (Main.netMode != NetmodeID.MultiplayerClient) //spawn geysers and bombs
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, 0, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FuseBomb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);

                                    if (head.localAI[2] == 1 && FargoSoulsWorld.EternityMode)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            Vector2 vel = Vector2.Normalize(NPC.oldVelocity).RotatedBy(Math.PI * 2 / 4 * (NPC.ai[3] < 0 ? i : i + 0.5));
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 1.5f * vel, ModContent.ProjectileType<EarthPalladOrb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                                        }
                                    }
                                    else
                                    {
                                        Vector2 spawnPos = NPC.Center;
                                        for (int i = 0; i <= 3; i++)
                                        {
                                            int tilePosX = (int)spawnPos.X / 16 + 250 * i / 16 * (int)-NPC.ai[3];
                                            int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<EarthGeyser>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                                        }
                                    }
                                }
                            }

                            NPC.localAI[1]++;

                            if (NPC.localAI[1] > (head.localAI[2] == 1 ? 20 : 30)) //proceed after short pause
                            {
                                NPC.netUpdate = true;
                                NPC.ai[0]++;
                                NPC.ai[1] = 0;
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.velocity = Vector2.Zero;

                                for (int i = 0; i < Main.maxNPCs; i++) //find the other hand
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPC.type && i != NPC.whoAmI && Main.npc[i].ai[2] == NPC.ai[2])
                                    {
                                        Main.npc[i].velocity = Vector2.Zero;
                                        Main.npc[i].ai[0] = NPC.ai[0];
                                        Main.npc[i].ai[1] = NPC.ai[1];
                                        Main.npc[i].localAI[0] = NPC.localAI[0];
                                        Main.npc[i].localAI[1] = NPC.localAI[1];
                                        Main.npc[i].netUpdate = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 8: //wait while head does fireballs
                    NPC.noTileCollide = true;

                    /*targetPos.X = head.Center.X + 330 * -NPC.ai[3];
                    targetPos.Y = player.Center.Y;
                    if (head.localAI[2] == 1) //if p2 and player is behind where hand should be, fuck them up
                    {
                        if (Math.Sign(player.Center.X - targetPos.X) == Math.Sign(-NPC.ai[3]))
                            targetPos.X = player.Center.X;
                    }
                    if (NPC.Distance(targetPos) > 30)
                        Movement(targetPos, 0.8f, 32f);

                    if (++NPC.localAI[0] > 120 && head.localAI[2] == 1)
                        NPC.localAI[3] = 1;*/

                    targetPos = head.Center + Vector2.UnitX * -NPC.ai[3] * ((head.localAI[2] == 1 ? 450 : 600) + 500f * (1f - Math.Min(1f, NPC.localAI[1] / 240f)));
                    if (NPC.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f, 32f);

                    NPC.rotation = MathHelper.PiOver2 * -NPC.ai[3] + MathHelper.Pi;

                    if (++NPC.localAI[1] > 90 && NPC.localAI[1] % 6 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitY * Main.rand.NextFloat(50, 100), Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, Vector2.UnitY.ToRotation(), 16);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].localAI[0] = 2f;
                        }
                    }

                    if (NPC.ai[1] > 60) //grace period over, if head reverts back then leave this state
                    {
                        if (head.ai[0] != 1)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.ai[1]++;

                        if (head.ai[0] == 0) //just entered here, change head to shoot fireballs
                        {
                            head.ai[0] = 1;
                            head.netUpdate = true;
                        }
                    }
                    break;

                case 9:
                    goto case 0;

                case 10: //crystal bomb drop
                    if (head.localAI[2] == 1)
                        NPC.position += player.velocity / 2;

                    if (NPC.ai[3] > 0)
                    {
                        targetPos = player.Center;
                        targetPos.Y = player.Center.Y - 400;
                        targetPos.X += player.velocity.X * 60;

                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.6f, 32f);

                        NPC.rotation = (float)Math.PI / 2;
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 300;
                        targetPos.X += 1000 * (float)Math.Sin(2 * Math.PI / 77 * NPC.ai[1]);

                        Movement(targetPos, 1.8f, 32f);

                        NPC.rotation = -(float)Math.PI / 2;

                        NPC.localAI[0] += 0.5f;
                    }

                    if (++NPC.localAI[0] > 60 && NPC.ai[1] > 120)
                    {
                        NPC.localAI[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * 2f, ModContent.ProjectileType<CrystalBomb>(),
                                FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, player.position.Y);
                        }
                    }

                    if (++NPC.ai[1] > 600)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = NPC.localAI[3] == 1 ? 0 : frameHeight;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage = 0;
            return true;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (FargoSoulsUtil.CanDeleteProjectile(projectile))
            {
                projectile.penetrate = 0;
                projectile.timeLeft = 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (NPC.Center.X < targetPos.X)
            {
                NPC.velocity.X += speedModifier;
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += speedModifier * 2;
            }
            else
            {
                NPC.velocity.X -= speedModifier;
                if (NPC.velocity.X > 0)
                    NPC.velocity.X -= speedModifier * 2;
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Burning, 300);
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Lethargic>(), 300);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Texture2D glowmask = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Champions/{Name}_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            if (NPC.dontTakeDamage)
            {
                Vector2 offset = Vector2.UnitX * Main.rand.NextFloat(-180, 180);
                Main.EntitySpriteDraw(texture2D13, NPC.Center + offset - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, origin2, NPC.scale, effects, 0);
                Main.EntitySpriteDraw(glowmask, NPC.Center + offset - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(glowmask, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
