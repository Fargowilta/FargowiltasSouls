using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.Champions;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class EarthChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Champion of Earth");
            //DisplayName.AddTranslation(GameCulture.Chinese, "大地英灵");
            Main.npcFrameCount[npc.type] = 2;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 180;
            npc.damage = 130;
            npc.defense = 80;
            npc.lifeMax = 320000;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(0, 15);
            npc.boss = true;

            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;

            npc.trapImmune = true;

            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            music = musicMod != null ? ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/Champions") : MusicID.Boss1;
            musicPriority = MusicPriority.BossHigh;

            npc.dontTakeDamage = true;
            npc.alpha = 255;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            EModeGlobalNPC.championBoss = npc.whoAmI;

            if (npc.localAI[3] == 0) //just spawned
            {
                if (!npc.HasValidTarget)
                    npc.TargetClosest(false);

                if (npc.ai[1] == 0)
                {
                    npc.Center = Main.player[npc.target].Center + new Vector2(500 * Math.Sign(npc.Center.X - Main.player[npc.target].Center.X), -250);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center + Vector2.UnitY * 1000, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, -Vector2.UnitY.ToRotation(), 10);
                        Projectile.NewProjectile(npc.Center - Vector2.UnitY * 1000, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, Vector2.UnitY.ToRotation(), 10);
                    }
                }

                if (++npc.ai[1] > 6 * 9) //nice
                {
                    npc.localAI[3] = 1;
                    npc.ai[1] = 0;
                    npc.netUpdate = true;

                    if (!Main.dedServ && Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        const int max = 8;
                        const float baseRotation = MathHelper.TwoPi / max;
                        for (int i = 0; i < max; i++)
                        {
                            float rotation = baseRotation * (i + Main.rand.NextFloat(-0.5f, 0.5f));
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, rotation, 3);
                        }

                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<EarthChampionHand>(), npc.whoAmI, 0, 0, npc.whoAmI, 1);
                        if (n < Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);

                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<EarthChampionHand>(), npc.whoAmI, 0, 0, npc.whoAmI, -1);
                        if (n < Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
                return;
            }

            Player player = Main.player[npc.target];
            Vector2 targetPos;

            if (npc.HasValidTarget && npc.Distance(player.Center) < 2500 && player.ZoneUnderworldHeight)
                npc.timeLeft = 600;

            npc.dontTakeDamage = false;
            npc.alpha = 0;

            switch ((int)npc.ai[0])
            {
                case -1:
                    npc.localAI[2] = 1;

                    //npc.dontTakeDamage = true;
                    npc.ai[1]++;

                    npc.velocity *= 0.95f;

                    /*if (npc.ai[1] < 120)
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 375;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.6f, 24f, true);
                    }
                    else*/
                    if (npc.ai[1] == 120) //begin healing
                    {
                        Main.PlaySound(SoundID.NPCDeath10, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -3);

                            if (!Main.dedServ && Main.LocalPlayer.active)
                                Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 8;
                                float baseRotation = MathHelper.TwoPi / max * Main.rand.NextFloat();
                                for (int i = 0; i < max; i++)
                                {
                                    float rotation = baseRotation + MathHelper.TwoPi / max * (i + Main.rand.NextFloat(-0.5f, 0.5f));
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, rotation, 3);
                                }
                            }
                        }
                    }
                    else if (npc.ai[1] > 120) //healing
                    {
                        npc.velocity *= 0.9f;
                        
                        int heal = (int)(npc.lifeMax / 2 / 120 * Main.rand.NextFloat(1f, 1.5f));
                        npc.life += heal;
                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);

                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(npc.Center, 0, 0, 174, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 8f;
                        }

                        if (npc.ai[1] > 240)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case 0: //float over player
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f
                        || !player.ZoneUnderworldHeight) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y += 1f;

                        return;
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 325;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.4f, 16f, true);
                    }

                    if (npc.localAI[2] == 0 && npc.life < npc.lifeMax / 2)
                    {
                        npc.ai[0] = -1;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;

                        for (int i = 0; i < Main.maxNPCs; i++) //find hands, update
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<EarthChampionHand>() && Main.npc[i].ai[2] == npc.whoAmI)
                            {
                                Main.npc[i].ai[0] = -1;
                                Main.npc[i].ai[1] = 0;
                                Main.npc[i].localAI[0] = 0;
                                Main.npc[i].localAI[1] = 0;
                                Main.npc[i].netUpdate = true;
                            }
                        }
                    }
                    break;

                case 1: //fireballs
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f
                        || !player.ZoneUnderworldHeight) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y += 1f;

                        return;
                    }
                    else
                    {
                        targetPos = player.Center;
                        for (int i = 0; i < 22; i++) //collision check above player's head
                        {
                            targetPos.Y -= 16;
                            Tile tile = Framing.GetTileSafely(targetPos); //if solid, stay below it
                            if (tile.active() && !tile.inActive() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type])
                            {
                                targetPos.Y += 50 + 16;
                                break;
                            }
                        }

                        if (npc.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.2f, 12f, true);
                            npc.position += (targetPos - npc.Center) / 30;
                        }

                        if (--npc.ai[2] < 0)
                        {
                            npc.ai[2] = 75;
                            Main.PlaySound(4, npc.Center, 13);
                            if (npc.ai[1] > 10 && Main.netMode != NetmodeID.MultiplayerClient) //shoot spread of fireballs, but not the first time
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    Projectile.NewProjectile(npc.Center + Vector2.UnitY * 60,
                                        (npc.localAI[2] == 1 ? 12 : 8) * npc.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(8 * i)),
                                        ProjectileID.Fireball, npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (++npc.ai[1] > 480)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.netUpdate = true;
                        }
                    }

                    if (npc.localAI[2] == 0 && npc.life < npc.lifeMax / 2)
                    {
                        npc.ai[0] = -1;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;

                        for (int i = 0; i < Main.maxNPCs; i++) //find hands, update
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<EarthChampionHand>() && Main.npc[i].ai[2] == npc.whoAmI)
                            {
                                Main.npc[i].ai[0] = -1;
                                Main.npc[i].ai[1] = 0;
                                Main.npc[i].localAI[0] = 0;
                                Main.npc[i].localAI[1] = 0;
                                Main.npc[i].netUpdate = true;
                            }
                        }
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
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
                npc.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > cap)
                npc.velocity.X = cap * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > cap)
                npc.velocity.Y = cap * Math.Sign(npc.velocity.Y);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = 0;
            switch ((int)npc.ai[0])
            {
                case -1:
                    if (npc.ai[1] > 120)
                        npc.frame.Y = frameHeight;
                    break;

                case 1:
                    if (npc.ai[2] < 20)
                        npc.frame.Y = frameHeight;
                    break;

                default:
                    break;
            }
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

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/EarthGore" + i.ToString()), npc.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[2] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            FargoSoulsGlobalNPC.DropEnches(npc, ModContent.ItemType<Items.Accessories.Forces.EarthForce>());
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

            Texture2D glowmask = ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/EarthChampion_Glow");

            if (npc.dontTakeDamage)
            {
                Vector2 offset = Vector2.UnitX * Main.rand.NextFloat(-180, 180);
                Main.spriteBatch.Draw(texture2D13, npc.Center + offset - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor) * 0.5f, npc.rotation, origin2, npc.scale, effects, 0f);
                Main.spriteBatch.Draw(glowmask, npc.Center + offset - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor) * 0.5f, npc.rotation, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Main.spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * npc.Opacity, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
