using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class TimberChampionHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Timber");
            DisplayName.AddTranslation(GameCulture.Chinese, "木英灵");
            Main.npcFrameCount[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 98;
            npc.height = 76;
            npc.damage = 130;
            npc.defense = 50;
            npc.lifeMax = 240000;
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath1;
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
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;

            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            music = musicMod != null ? ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/Champions") : MusicID.Boss1;
            musicPriority = MusicPriority.BossHigh;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void AI()
        {
            if (npc.localAI[2] == 0)
            {
                Main.PlaySound(SoundID.Roar, npc.Center, 0);
                npc.TargetClosest(false);
                npc.localAI[2] = 1;
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            npc.direction = npc.spriteDirection = npc.Center.X < player.position.X ? 1 : -1;
            Vector2 targetPos;

            switch ((int)npc.ai[0])
            {
                case 0: //laser rain
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y -= 1f;

                        break;
                    }
                    else
                    {
                        targetPos = player.Center + npc.DirectionFrom(player.Center) * 400f;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.2f, 24f);
                    }

                    if (++npc.ai[1] > 30)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 1: //laser rain
                    if (npc.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, (player.Center - npc.Center) / 120,
                            ModContent.ProjectileType<TimberSquirrel>(), npc.damage / 4, 0f, Main.myPlayer);
                    }

                    if (npc.ai[1] > 90)
                    {
                        npc.velocity *= 0.9f;
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 300;

                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.25f, 24f);
                    }

                    if (++npc.ai[1] < 120)
                    {
                        /*for (int i = 0; i < 5; i++) //warning dust
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 16, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[d].velocity *= 3f;
                            Main.dust[d].noGravity = true;
                        }*/
                    }
                    else if (npc.ai[1] == 120)
                    {
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] < 270) //spam lasers everywhere
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 spawnPos = player.Center;
                                spawnPos.X += Main.rand.NextFloat(-1000, 1000);
                                spawnPos.Y -= Main.rand.NextFloat(600, 800);
                                Vector2 speed = Main.rand.NextFloat(7.5f, 12.5f) * Vector2.UnitY;
                                Projectile.NewProjectile(spawnPos, speed, ModContent.ProjectileType<TimberLaser>(), 
                                    npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, 100f);
                            }
                        }
                    }
                    else
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 2:
                    goto case 0;

                case 3: //shoot acorns
                    targetPos = player.Center;
                    targetPos.X += npc.Center.X < player.Center.X ? -400 : 400;
                    targetPos.Y -= 100;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.25f, 32f);

                    if (++npc.ai[2] > 35) //acorn
                    {
                        npc.ai[2] = 0;
                        const float gravity = 0.2f;
                        float time = 40f;
                        Vector2 distance = player.Center - npc.Center;// + player.velocity * 30f;
                        distance.X += player.velocity.X * 40;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 20; i++)
                        {
                            Projectile.NewProjectile(npc.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 3,
                                ModContent.ProjectileType<Acorn>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.ai[1] > 200)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //trees that shoot acorns
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 300;
                    if (targetPos.Y > player.position.Y - 100)
                        targetPos.Y = player.position.Y - 100;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f, 24f);

                    if (--npc.ai[2] < 0)
                    {
                        npc.ai[2] = 65;
                        
                        for (int i = 0; i < 5; i++) //spawn trees
                        {
                            Vector2 spawnPos = player.Center;
                            spawnPos.X += Main.rand.NextFloat(-1500, 1500) + player.velocity.X * 75;
                            spawnPos.Y -= Main.rand.NextFloat(300);
                            for (int j = 0; j < 100; j++) //go down until solid tile found
                            {
                                Tile tile = Main.tile[(int)spawnPos.X / 16, (int)spawnPos.Y / 16];
                                if (tile == null)
                                    tile = new Tile();
                                if (tile.nactive() && (Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]))
                                    break;
                                spawnPos.Y += 16;
                            }
                            for (int j = 0; j < 50; j++) //go up until non-solid tile found
                            {
                                Tile tile = Main.tile[(int)spawnPos.X / 16, (int)spawnPos.Y / 16];
                                if (tile == null)
                                    tile = new Tile();
                                if (!(tile.nactive() && (Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type])))
                                    break;
                                spawnPos.Y -= 16;
                            }
                            spawnPos.Y -= 152; //offset for height of tree
                            Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<TimberTree>(), npc.damage / 4, 0f, Main.myPlayer, npc.target);
                        }
                    }

                    if (++npc.ai[1] > 300)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 6:
                    goto case 0;

                case 7: //chains
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 150;
                    Movement(targetPos, 0.3f, 24f);

                    if (npc.ai[1] < 240)
                    {
                        if (++npc.ai[2] > 8)
                        {
                            npc.ai[2] = 0;

                            Main.PlaySound(SoundID.Item92, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = 32f * npc.DirectionTo(player.Center).RotatedByRandom(Math.PI / 2);
                                Projectile.NewProjectile(npc.Center, speed,
                                    ModContent.ProjectileType<SquirrelHook2>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                            }
                        }
                    }
                    else
                    {
                        npc.velocity *= 0.9f;
                    }

                    if (++npc.ai[1] > 300)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 8: //electrify chains
                    npc.velocity = Vector2.Zero;

                    if (npc.ai[1] == 0)
                    {
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                    }

                    if (++npc.ai[1] > 120)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 9:
                    goto case 0;

                case 10:
                    goto case 3;

                case 11:
                    goto case 0;

                case 12: //noah snowballs
                    targetPos = player.Center;
                    targetPos.X += player.velocity.X * 45f;
                    targetPos.Y -= 200;
                    Movement(targetPos, 0.45f, 32f);
                    
                    if (++npc.ai[2] > 5)
                    {
                        npc.ai[2] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = npc.ai[1] > 120 ? npc.damage / 4 : 0;
                            for (int i = -2; i <= 2; i++)
                            {
                                Vector2 speed = new Vector2(5f * i, -20f);
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<Snowball2>(), damage, 0f, Main.myPlayer, npc.target);
                            }
                        }
                    }

                    if (++npc.ai[1] > 420)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
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

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter > 3)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                    npc.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Guilty>(), 600);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/TimberGore1"), npc.scale);
                pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/TimberGore2"), npc.scale);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[0] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            int[] drops = {
                ModContent.ItemType<WoodEnchant>(),
                ModContent.ItemType<BorealWoodEnchant>(),
                ModContent.ItemType<RichMahoganyEnchant>(),
                ModContent.ItemType<EbonwoodEnchant>(),
                ModContent.ItemType<ShadewoodEnchant>(),
                ModContent.ItemType<PalmWoodEnchant>(),
                ModContent.ItemType<PearlwoodEnchant>()
            };
            int lastDrop = -1; //don't drop same ench twice
            for (int i = 0; i < 2; i++)
            {
                int thisDrop = Main.rand.Next(drops.Length);

                if (lastDrop == thisDrop) //try again
                {
                    if (++thisDrop >= drops.Length) //drop first ench in line if looped past array
                        thisDrop = 0;
                }

                lastDrop = thisDrop;
                Item.NewItem(npc.position, npc.Size, drops[thisDrop]);
            }
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = npc.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            Texture2D texture2D14 = mod.GetTexture("NPCs/Champions/TimberChampionHead_Trail");
            Texture2D texture2D15 = mod.GetTexture("NPCs/Champions/TimberChampionHead_Glow");
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = Color.White * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D14, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(drawColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D15, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
