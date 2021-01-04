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
    public class TimberChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Timber");
            DisplayName.AddTranslation(GameCulture.Chinese, "木英灵");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 234;
            npc.damage = 130;
            npc.defense = 50;
            npc.lifeMax = 180000;
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            //npc.value = Item.buyPrice(0, 15);
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
                npc.TargetClosest(false);
                npc.localAI[2] = 1;
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            npc.direction = npc.spriteDirection = npc.position.X < player.position.X ? 1 : -1;
            
            switch ((int)npc.ai[0])
            {
                case -1: //mourning wood movement
                    {
                        npc.noTileCollide = true;
                        npc.noGravity = true;

                        if (Math.Abs(player.Center.X - npc.Center.X) < npc.width / 2)
                        {
                            npc.velocity.X *= 0.9f;
                            if (Math.Abs(npc.velocity.X) < 0.1f)
                                npc.velocity.X = 0f;
                        }
                        else
                        {
                            float accel = 4f;
                            if (npc.direction > 0)
                                npc.velocity.X = (npc.velocity.X * 20 + accel) / 21;
                            else
                                npc.velocity.X = (npc.velocity.X * 20 - accel) / 21;
                        }

                        bool onPlatforms = false;
                        for (int i = (int)npc.position.X; i <= npc.position.X + npc.width; i += 16)
                        {
                            if (Framing.GetTileSafely(new Vector2(i, npc.position.Y + npc.height + npc.velocity.Y + 1)).type == TileID.Platforms)
                            {
                                onPlatforms = true;
                                break;
                            }
                        }

                        bool onCollision = Collision.SolidCollision(npc.position, npc.width, npc.height);

                        if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width
                            && npc.position.Y + npc.height < player.position.Y + player.height - 16)
                        {
                            npc.velocity.Y += 0.5f;
                        }
                        else if (onCollision || (onPlatforms && player.position.Y + player.height <= npc.position.Y + npc.height))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y = 0f;

                            if (onCollision)
                            {
                                if (npc.velocity.Y > -0.2f)
                                    npc.velocity.Y -= 0.025f;
                                else
                                    npc.velocity.Y -= 0.2f;

                                if (npc.velocity.Y < -4f)
                                    npc.velocity.Y = -4f;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < 0f)
                                npc.velocity.Y = 0f;

                            if (npc.velocity.Y < 0.1f)
                                npc.velocity.Y += 0.025f;
                            else
                                npc.velocity.Y += 0.5f;
                        }

                        if (npc.velocity.Y > 10f)
                            npc.velocity.Y = 10f;
                    }
                    break;

                case 0: //jump at player
                    npc.noTileCollide = false;
                    npc.noGravity = false;

                    if (++npc.ai[1] == 60)
                    {
                        npc.TargetClosest();

                        if (npc.localAI[0] == 0 && npc.life < npc.lifeMax * .66f) //spawn palm tree supports
                        {
                            npc.localAI[0] = 1;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PalmTreeHostile>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                            }
                        }

                        if (npc.localAI[1] == 0 && npc.life < npc.lifeMax * .33f) //spawn palm tree supports
                        {
                            npc.localAI[1] = 1;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PalmTreeHostile>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                            }
                        }

                        const float gravity = 0.4f;
                        const float time = 90f;
                        
                        Vector2 distance = player.Center - npc.Center;
                        distance.Y -= npc.height;

                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        npc.velocity = distance;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //explosive jump
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.DD2OgreSmash, npc.damage / 4, 0, Main.myPlayer);
                        }

                        Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 14);

                        for (int k = -2; k <= 2; k++) //explosions
                        {
                            Vector2 dustPos = npc.Center;
                            int width = npc.width / 5;
                            dustPos.X += width * k + Main.rand.NextFloat(-width, width);
                            dustPos.Y += Main.rand.NextFloat(npc.height / 2);

                            for (int i = 0; i < 30; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                                Main.dust[dust].velocity *= 1.4f;
                            }

                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, 6, 0f, 0f, 100, default(Color), 3.5f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 7f;
                                dust = Dust.NewDust(dustPos, 32, 32, 6, 0f, 0f, 100, default(Color), 1.5f);
                                Main.dust[dust].velocity *= 3f;
                            }

                            float scaleFactor9 = 0.5f;
                            for (int j = 0; j < 4; j++)
                            {
                                int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                                Main.gore[gore].velocity *= scaleFactor9;
                                Main.gore[gore].velocity.X += 1f;
                                Main.gore[gore].velocity.Y += 1f;
                            }
                        }
                    }
                    else if (npc.ai[1] > 60)
                    {
                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y += 0.4f;

                        if (npc.ai[1] > 60 + 90)
                        {
                            npc.TargetClosest();
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    else //less than 60
                    {
                        if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                            npc.velocity.X = Math.Abs(npc.velocity.Y) * Math.Sign(npc.velocity.X);
                        if (npc.velocity.Y == 0)
                            npc.velocity.X *= 0.99f;

                        if (npc.ai[0] != 0 && (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f))
                        {
                            npc.TargetClosest();
                            if (npc.timeLeft > 30)
                                npc.timeLeft = 30;

                            npc.noTileCollide = true;
                            npc.noGravity = true;
                            npc.velocity.Y -= 1f;

                            npc.ai[1] = 0; //prevent proceeding to next steps of ai while despawning
                        }
                        else
                        {
                            npc.timeLeft = 600;
                        }

                        goto case -1;
                    }
                    break;

                case 1: //acorn sprays
                    if (++npc.ai[2] > 35)
                    {
                        npc.ai[2] = 0;
                        const float gravity = 0.2f;
                        float time = 60f;
                        Vector2 distance = player.Center - npc.Center;// + player.velocity * 30f;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 20; i++)
                        {
                            Projectile.NewProjectile(npc.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 3,
                                ModContent.ProjectileType<Acorn>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                    
                    if (++npc.ai[1] > 120)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    goto case -1;

                case 2:
                    goto case 0;

                case 3: //snowball barrage
                    if (++npc.ai[2] > 5)
                    {
                        npc.ai[2] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] > 30 && npc.ai[1] < 120)
                        {
                            Vector2 offset;
                            offset.X = Main.rand.NextFloat(0, npc.width / 2) * npc.direction;
                            offset.Y = 16;
                            Projectile.NewProjectile(npc.Center + offset,
                                Vector2.UnitY * -12f, ModContent.ProjectileType<Snowball>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.ai[1] > 150)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    goto case -1;

                case 4:
                    goto case 0;

                case 5: //spray squirrels
                    if (++npc.ai[2] > 6)
                    {
                        npc.ai[2] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<LesserSquirrel>());
                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].velocity.X = Main.rand.NextFloat(-10, 10);
                                Main.npc[n].velocity.Y = Main.rand.NextFloat(-20, -10);
                                Main.npc[n].netUpdate = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                    }

                    if (++npc.ai[1] > 180)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    goto case -1;

                case 6:
                    goto case 0;

                case 7:
                    goto case 3;

                case 8:
                    goto case 0;

                case 9: //grappling hook
                    if (npc.ai[2] == 0) //shoot hook
                    {
                        npc.ai[2] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 16, ModContent.ProjectileType<SquirrelHook>(), 0, 0f, Main.myPlayer, npc.whoAmI);
                        }
                    }

                    if (++npc.ai[1] < 240) //charge power
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 2f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 8f;
                            d = Dust.NewDust(npc.Center, 0, 0, DustID.Shadowflame, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 16f;
                        }

                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 500);
                            offset.Y += (float)(Math.Cos(angle) * 500);
                            Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4, 4), 0, 0, DustID.Shadowflame, 0, 0, 100, Color.White, 2f)];
                            dust.velocity = npc.velocity;
                            if (Main.rand.Next(3) == 0)
                                dust.velocity += Vector2.Normalize(offset) * -5f;
                            dust.noGravity = true;
                        }
                    }
                    else if (npc.ai[1] == 240) //explode
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowflameBlast>(), npc.damage / 2, 0f, Main.myPlayer);
                        }
                    }
                    else if (npc.ai[1] > 300)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    goto case -1;

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
            switch ((int)npc.ai[0])
            {
                case 0:
                case 2:
                case 4:
                case 6:
                case 8:
                    if (npc.ai[1] <= 60)
                        npc.frame.Y = frameHeight * 6; //crouching for jump
                    else
                        npc.frame.Y = frameHeight * 7; //jumping
                    break;

                default:
                    if (++npc.frameCounter > 5) //walking animation
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 6)
                        npc.frame.Y = 0;

                    if (npc.velocity.X == 0)
                        npc.frame.Y = frameHeight; //stationary sprite if standing still

                    if (npc.velocity.Y > 4)
                        npc.frame.Y = frameHeight * 7; //jumping
                    break;
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
                for (int i = 3; i <= 10; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/TimberGore" + i.ToString()), npc.scale);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && FargoSoulsWorld.MasochistMode)
                {
                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<TimberChampionHead>(), npc.whoAmI, Target: npc.target);
                    if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return !FargoSoulsWorld.MasochistMode;
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
    }
}
