using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class EarthChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Earth");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "大地英灵");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = $"FargowiltasSouls/NPCs/Champions/{Name}_Still",
                Scale = 0.75f,
                Position = new Vector2(0, 10),
                PortraitScale = 0.5f,
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                return NPC.GetBestiaryEntryColor();
            }
            return base.GetAlpha(drawColor);
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 180;
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
            NPC.value = Item.buyPrice(1);
            NPC.boss = true;

            NPC.trapImmune = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            EModeGlobalNPC.championBoss = NPC.whoAmI;

            if (NPC.localAI[3] == 0) //just spawned
            {
                if (!NPC.HasValidTarget)
                    NPC.TargetClosest(false);

                if (NPC.ai[1] == 0)
                {
                    NPC.Center = Main.player[NPC.target].Center + new Vector2(500 * Math.Sign(NPC.Center.X - Main.player[NPC.target].Center.X), -250);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitY * 1000, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, -Vector2.UnitY.ToRotation(), 10);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - Vector2.UnitY * 1000, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, Vector2.UnitY.ToRotation(), 10);
                    }
                }

                if (++NPC.ai[1] > 6 * 9) //nice
                {
                    NPC.localAI[3] = 1;
                    NPC.ai[1] = 0;
                    NPC.netUpdate = true;

                    if (!Main.dedServ && Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        const int max = 8;
                        const float baseRotation = MathHelper.TwoPi / max;
                        for (int i = 0; i < max; i++)
                        {
                            float rotation = baseRotation * (i + Main.rand.NextFloat(-0.5f, 0.5f));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EarthChainBlast2>(), 0, 0f, Main.myPlayer, rotation, 3);
                        }

                        FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<EarthChampionHand>(), NPC.whoAmI, 0, 0, NPC.whoAmI, 1);
                        FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<EarthChampionHand>(), NPC.whoAmI, 0, 0, NPC.whoAmI, -1);
                    }
                }
                return;
            }

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (NPC.HasValidTarget && NPC.Distance(player.Center) < 2500 && player.ZoneUnderworldHeight)
                NPC.timeLeft = 600;

            NPC.dontTakeDamage = false;
            NPC.alpha = 0;

            switch ((int)NPC.ai[0])
            {
                case -1:
                    NPC.localAI[2] = 1;

                    //NPC.dontTakeDamage = true;
                    NPC.ai[1]++;

                    NPC.velocity *= 0.95f;

                    /*if (NPC.ai[1] < 120)
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 375;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.6f, 24f, true);
                    }
                    else*/
                    if (NPC.ai[1] == 120) //begin healing
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -3);

                            if (!Main.dedServ && Main.LocalPlayer.active)
                                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

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
                    }
                    else if (NPC.ai[1] > 120) //healing
                    {
                        NPC.velocity *= 0.9f;

                        int heal = (int)(NPC.lifeMax / 2 / 120 * Main.rand.NextFloat(1f, 1.5f));
                        NPC.life += heal;
                        if (NPC.life > NPC.lifeMax)
                            NPC.life = NPC.lifeMax;
                        CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);

                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(NPC.Center, 0, 0, 174, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 8f;
                        }

                        if (NPC.ai[1] > 240)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 0: //float over player
                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f
                        || !player.ZoneUnderworldHeight) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y += 1f;

                        return;
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 325;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.4f, 16f, true);
                    }

                    if (NPC.localAI[2] == 0 && NPC.life < NPC.lifeMax / 2)
                    {
                        NPC.ai[0] = -1;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;

                        for (int i = 0; i < Main.maxNPCs; i++) //find hands, update
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<EarthChampionHand>() && Main.npc[i].ai[2] == NPC.whoAmI)
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
                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f
                        || !player.ZoneUnderworldHeight) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y += 1f;

                        return;
                    }
                    else
                    {
                        targetPos = player.Center;
                        for (int i = 0; i < 22; i++) //collision check above player's head
                        {
                            targetPos.Y -= 16;
                            Tile tile = Framing.GetTileSafely(targetPos); //if solid, stay below it
                            if (tile.HasTile && !tile.IsActuated && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                            {
                                targetPos.Y += 50 + 16;
                                break;
                            }
                        }

                        if (NPC.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.2f, 12f, true);
                            NPC.position += (targetPos - NPC.Center) / 30;
                        }

                        if (--NPC.ai[2] < 0)
                        {
                            NPC.ai[2] = 75;
                            SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                            if (NPC.ai[1] > 10 && Main.netMode != NetmodeID.MultiplayerClient) //shoot spread of fireballs, but not the first time
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitY * 60,
                                        (NPC.localAI[2] == 1 ? 12 : 8) * NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(8 * i)),
                                        ProjectileID.Fireball, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (++NPC.ai[1] > 480)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }

                    if (NPC.localAI[2] == 0 && NPC.life < NPC.lifeMax / 2)
                    {
                        NPC.ai[0] = -1;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;

                        for (int i = 0; i < Main.maxNPCs; i++) //find hands, update
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<EarthChampionHand>() && Main.npc[i].ai[2] == NPC.whoAmI)
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
                    NPC.ai[0] = 0;
                    goto case 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
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
                NPC.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0;
            switch ((int)NPC.ai[0])
            {
                case -1:
                    if (NPC.ai[1] > 120)
                        NPC.frame.Y = frameHeight;
                    break;

                case 1:
                    if (NPC.ai[2] < 20)
                        NPC.frame.Y = frameHeight;
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
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"EarthGore{i}").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.EarthChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(EarthForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<EarthChampionRelic>()));
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
            Main.EntitySpriteDraw(glowmask, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * NPC.Opacity, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
