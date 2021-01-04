using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Champions;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.NPCs.Champions
{
    public class EarthChampionHand : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Earth");
            DisplayName.AddTranslation(GameCulture.Chinese, "大地英灵");
            Main.npcFrameCount[npc.type] = 2;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 100;
            npc.height = 100;
            npc.damage = 125;
            npc.defense = 80;
            npc.lifeMax = 300000;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;

            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;

            npc.trapImmune = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.localAI[3] == 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!(npc.ai[2] > -1 && npc.ai[2] < Main.maxNPCs && Main.npc[(int)npc.ai[2]].active
                && Main.npc[(int)npc.ai[2]].type == ModContent.NPCType<EarthChampion>()))
            {
                npc.life = 0;
                npc.checkDead();
                npc.active = false;
                return;
            }
            
            NPC head = Main.npc[(int)npc.ai[2]];

            npc.lifeMax = head.lifeMax;
            npc.damage = head.damage;
            npc.defDamage = head.defDamage;
            npc.defense = head.defense;
            npc.defDefense = head.defDefense;
            npc.target = head.target;
            npc.dontTakeDamage = head.dontTakeDamage;

            npc.life = npc.lifeMax;

            Player player = Main.player[npc.target];
            Vector2 targetPos;

            npc.direction = npc.spriteDirection = (int)npc.ai[3];
            npc.localAI[3] = 0;
            
            switch ((int)npc.ai[0])
            {
                case -1: //healing
                    targetPos = head.Center;
                    targetPos.Y += head.height;
                    targetPos.X += head.width * npc.ai[3] / 2;
                    Movement(targetPos, 0.8f, 32f);

                    if (npc.ai[3] > 0)
                        npc.rotation = -(float)Math.PI / 4 - (float)Math.PI / 2;
                    else
                        npc.rotation = -(float)Math.PI / 4 + (float)Math.PI;

                    if (++npc.ai[1] > 120) //clench fist as boss heals
                    {
                        npc.localAI[3] = 1;

                        if (npc.ai[1] > 240)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case 0: //float near head
                    npc.noTileCollide = true;

                    targetPos = head.Center;
                    targetPos.Y += 250;
                    targetPos.X += 300 * -npc.ai[3];
                    Movement(targetPos, 0.8f, 32f);

                    npc.rotation = 0;

                    if (++npc.ai[1] > 60)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 1: //dashes
                    if (++npc.ai[1] < 75) //hover near a side
                    {
                        npc.rotation = 0;

                        targetPos = player.Center + player.DirectionTo(npc.Center) * 400;
                        if (npc.ai[3] < 0 && targetPos.X < player.Center.X + 400) //stay on your original side
                            targetPos.X = player.Center.X + 400;
                        if (npc.ai[3] > 0 && targetPos.X > player.Center.X - 400)
                            targetPos.X = player.Center.X - 400;

                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, head.localAI[2] == 1 ? 2.4f : 1.2f, 32f);

                        if (head.localAI[2] == 1)
                            npc.position += player.velocity / 3f;
                    }
                    else if (npc.ai[1] < 120) //prepare to dash, enable hitbox
                    {
                        if (head.localAI[2] == 1)
                            npc.position += player.velocity / 10f;

                        npc.localAI[3] = 1;
                        npc.velocity *= npc.localAI[2] == 1 ? 0.8f : 0.95f;
                        npc.rotation = npc.DirectionTo(player.Center).ToRotation() - (float)Math.PI / 2;
                    }
                    else if (npc.ai[1] == 120) //dash
                    {
                        npc.localAI[3] = 1;
                        npc.velocity = npc.DirectionTo(player.Center) * (head.localAI[2] == 1 ? 20 : 16);
                    }
                    else //while dashing
                    {
                        npc.velocity *= 1.02f;

                        npc.localAI[3] = 1;
                        npc.rotation = npc.velocity.ToRotation() - (float)Math.PI / 2;

                        for (int i = 0; i < 5; i++) //flame jet behind self
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, -npc.velocity.X * 0.25f, -npc.velocity.Y * 0.25f, Scale: 3f);
                            Main.dust[d].position -= Vector2.Normalize(npc.velocity) * npc.width / 2;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 4f;
                        }

                        //passed player, prepare another dash
                        if ((++npc.localAI[1] > 60 && npc.Distance(player.Center) > 1000) ||
                            (npc.ai[3] > 0 ? 
                            npc.Center.X > Math.Min(head.Center.X, player.Center.X) + 300 : npc.Center.X < Math.Max(head.Center.X, player.Center.X) - 300))
                        {
                            npc.ai[1] = head.localAI[2] == 1 ? 15 : 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;

                            if (head.localAI[2] == 1 && FargoSoulsWorld.MasochistMode && Main.netMode != NetmodeID.MultiplayerClient) //explosion chain
                            {
                                Projectile.NewProjectile(npc.Center + (Vector2.Normalize(npc.velocity) * 100), Vector2.Zero, ModContent.ProjectileType<EarthChainBlast>(),
                                    npc.damage / 4, 0f, Main.myPlayer, npc.velocity.ToRotation(), 7);
                            }

                            npc.velocity = Vector2.Zero;
                        }
                    }

                    if (++npc.localAI[0] > 660) //proceed
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 2:
                    goto case 0;

                case 3: //petal shots
                    if (npc.ai[3] > 0)
                    {
                        targetPos = player.Center;
                        targetPos.Y += player.velocity.Y * 60;
                        targetPos.X = player.Center.X - 400;

                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.4f, 32f);
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.X += 400;
                        targetPos.Y += 600 * (float)Math.Sin(2 * Math.PI / 77 * npc.ai[1]);

                        Movement(targetPos, 0.8f, 32f);
                    }

                    if (++npc.localAI[0] > (head.localAI[2] == 1 ? 18 : 24))
                    {
                        npc.localAI[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.UnitX * npc.ai[3], ModContent.ProjectileType<FlowerPetal>(), 
                                npc.damage / 4, 0f, Main.myPlayer, head.localAI[2] == 1 && FargoSoulsWorld.MasochistMode ? 0 : 1);
                        }
                    }

                    if (++npc.ai[1] > 360)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //slam three times
                case 6:
                case 7:
                    if (++npc.ai[1] < 90) //float over head
                    {
                        npc.noTileCollide = true;

                        targetPos = head.Center;
                        targetPos.Y -= head.height;
                        targetPos.X += 50 * -npc.ai[3];
                        Movement(targetPos, 2.0f, 32f);

                        npc.rotation = 0;
                    }
                    else if (npc.ai[1] == 90) //dash down
                    {
                        npc.velocity = Vector2.UnitY * (head.localAI[2] == 1 ? 36 : 24);
                        npc.localAI[0] = player.position.Y;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        npc.localAI[3] = 1;

                        if (npc.ai[3] > 0)
                            npc.rotation = -(float)Math.PI / 2;
                        else
                            npc.rotation = (float)Math.PI / 2;

                        if (npc.position.Y + npc.height > npc.localAI[0]) //become solid to smash on tiles
                            npc.noTileCollide = false;

                        //extra checks to prevent noclipping
                        if (!npc.noTileCollide)
                        {
                            if (Collision.SolidCollision(npc.position, npc.width, npc.height)
                                || npc.position.Y + npc.height > Main.maxTilesY * 16 - 16)
                                npc.velocity.Y = 0;
                        }

                        if (npc.velocity.Y == 0) //we've hit something
                        {
                            if (npc.localAI[0] != 0)
                            {
                                npc.localAI[0] = 0;

                                if (Main.netMode != NetmodeID.MultiplayerClient) //spawn geysers and bombs
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, 0, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FuseBomb>(), npc.damage / 4, 0f, Main.myPlayer);
                                    
                                    if (head.localAI[2] == 1 && FargoSoulsWorld.MasochistMode)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            Vector2 vel = Vector2.Normalize(npc.oldVelocity).RotatedBy(Math.PI * 2 / 4 * (npc.ai[3] < 0 ? i : i + 0.5));
                                            Projectile.NewProjectile(npc.Center, 1.5f * vel, ModContent.ProjectileType<EarthPalladOrb>(), npc.damage / 4, 0f, Main.myPlayer);
                                        }
                                    }
                                    else
                                    {
                                        Vector2 spawnPos = npc.Center;
                                        for (int i = 0; i <= 3; i++)
                                        {
                                            int tilePosX = (int)spawnPos.X / 16 + 250 * i / 16 * (int)-npc.ai[3];
                                            int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                            Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<EarthGeyser>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                                        }
                                    }
                                }
                            }

                            npc.localAI[1]++;

                            if (npc.localAI[1] > (head.localAI[2] == 1 ? 20 : 30)) //proceed after short pause
                            {
                                npc.netUpdate = true;
                                npc.ai[0]++;
                                npc.ai[1] = 0;
                                npc.localAI[0] = 0;
                                npc.localAI[1] = 0;
                                npc.velocity = Vector2.Zero;

                                for (int i = 0; i < Main.maxNPCs; i++) //find the other hand
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == npc.type && i != npc.whoAmI && Main.npc[i].ai[2] == npc.ai[2])
                                    {
                                        Main.npc[i].velocity = Vector2.Zero;
                                        Main.npc[i].ai[0] = npc.ai[0];
                                        Main.npc[i].ai[1] = npc.ai[1];
                                        Main.npc[i].localAI[0] = npc.localAI[0];
                                        Main.npc[i].localAI[1] = npc.localAI[1];
                                        Main.npc[i].netUpdate = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 8: //wait while head does fireballs
                    npc.noTileCollide = true;
                    
                    targetPos.X = head.Center.X + 330 * -npc.ai[3];
                    targetPos.Y = player.Center.Y;
                    if (head.localAI[2] == 1) //if p2 and player is behind where hand should be, fuck them up
                    {
                        if (Math.Sign(player.Center.X - targetPos.X) == Math.Sign(-npc.ai[3]))
                            targetPos.X = player.Center.X;
                    }
                    if (npc.Distance(targetPos) > 30)
                        Movement(targetPos, 0.8f, 32f);

                    npc.rotation = 0;

                    if (++npc.localAI[0] > 120 && head.localAI[2] == 1)
                    {
                        npc.localAI[3] = 1;
                    }

                    if (npc.ai[1] > 60) //grace period over, if head reverts back then leave this state
                    {
                        if (head.ai[0] != 1)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.localAI[0] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        npc.ai[1]++;

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
                        npc.position += player.velocity / 2;

                    if (npc.ai[3] > 0)
                    {
                        targetPos = player.Center;
                        targetPos.Y = player.Center.Y - 400;
                        targetPos.X += player.velocity.X * 60;

                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.6f, 32f);

                        npc.rotation = (float)Math.PI / 2;
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 300;
                        targetPos.X += 1000 * (float)Math.Sin(2 * Math.PI / 77 * npc.ai[1]);

                        Movement(targetPos, 1.8f, 32f);
                        
                        npc.rotation = -(float)Math.PI / 2;

                        npc.localAI[0] += 0.5f;
                    }

                    if (++npc.localAI[0] > 60 && npc.ai[1] > 120)
                    {
                        npc.localAI[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY * 2f, ModContent.ProjectileType<CrystalBomb>(),
                                npc.damage / 4, 0f, Main.myPlayer, player.position.Y);
                        }
                    }

                    if (++npc.ai[1] > 600)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = npc.localAI[3] == 1 ? 0 : frameHeight;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage = 0;
            return true;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (!projectile.minion)
            {
                projectile.penetrate = 0;
                projectile.timeLeft = 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * 2;
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * 2;
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > cap)
                npc.velocity.X = cap * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > cap)
                npc.velocity.Y = cap * Math.Sign(npc.velocity.Y);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (FargoSoulsWorld.MasochistMode)
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }

            Texture2D glowmask = ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/EarthChampionHand_Glow");

            if (npc.dontTakeDamage)
            {
                Vector2 offset = Vector2.UnitX * Main.rand.NextFloat(-180, 180);
                Main.spriteBatch.Draw(texture2D13, npc.Center + offset - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor) * 0.5f, npc.rotation, origin2, npc.scale, effects, 0f);
                Main.spriteBatch.Draw(glowmask, npc.Center + offset - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor) * 0.5f, npc.rotation, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Main.spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
