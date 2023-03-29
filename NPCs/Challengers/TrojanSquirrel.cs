using FargowiltasSouls.BossBars;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.BossBags;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Items.Placeables.Trophies;
using FargowiltasSouls.Items.Summons;
using FargowiltasSouls.Items.Weapons.Challengers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Challengers
{
    public abstract class TrojanSquirrelPart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Trojan Squirrel");

            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.damage = 24;
            NPC.defense = 2;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;

            if (Main.getGoodWorld)
            {
                NPC.scale *= 2;
                NPC.defense += 10;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }

    public abstract class TrojanSquirrelLimb : TrojanSquirrelPart
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.hide = true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        protected NPC body;

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC)
                body = sourceNPC;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(body is NPC ? body.whoAmI : -1);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            body = FargoSoulsUtil.NPCExists(reader.ReadInt32());
        }

        public override bool PreAI()
        {
            if (body != null)
                body = FargoSoulsUtil.NPCExists(body.whoAmI, ModContent.NPCType<TrojanSquirrel>());

            if (body == null)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.life = 0;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    NPC.active = false;
                }
                return false;
            }

            return base.PreAI();
        }

        public override bool CheckActive() => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);

            npcLoot.Add(ItemDropRule.DropNothing());
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            if (body != null)
                NPC.frame = body.frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (body == null)
                return base.PreDraw(spriteBatch, screenPos, drawColor);

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 center = body.Center;

            Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY - 53 * body.scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            return false;
        }
    }

    [AutoloadBossHead]
    public class TrojanSquirrel : TrojanSquirrelPart
    {
        private const float BaseWalkSpeed = 4f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = $"FargowiltasSouls/NPCs/Challengers/{Name}_Still",
                Position = new Vector2(16 * 4, 16 * 4),
                PortraitPositionXOverride = 16 * 1.5f,
                PortraitPositionYOverride = 16 * 3
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 800;

            NPC.width = 100;
            NPC.height = 120; //234

            NPC.value = Item.buyPrice(silver: 75);
            NPC.boss = true;

            Music = MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.BossBar = ModContent.GetInstance<CompositeBossBar>();
        }

        public NPC head;
        public NPC arms;
        public int lifeMaxHead;
        public int lifeMaxArms;

        private bool spawned;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(head is NPC ? head.whoAmI : -1);
            writer.Write(arms is NPC ? arms.whoAmI : -1);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            head = FargoSoulsUtil.NPCExists(reader.ReadInt32());
            arms = FargoSoulsUtil.NPCExists(reader.ReadInt32());
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (ModContent.TryFind("Fargowiltas", "Squirrel", out ModNPC modNPC))
            {
                int n = NPC.FindFirstNPC(modNPC.Type);
                if (n != -1 && n != Main.maxNPCs)
                {
                    NPC.Bottom = Main.npc[n].Bottom;

                    Main.npc[n].life = 0;
                    Main.npc[n].active = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }
        }

        private void TileCollision(bool fallthrough = false, bool dropDown = false)
        {
            bool onPlatforms = false;
            for (int i = (int)NPC.position.X; i <= NPC.position.X + NPC.width; i += 16)
            {
                if (Framing.GetTileSafely(new Vector2(i, NPC.Bottom.Y + 2)).TileType == TileID.Platforms)
                {
                    onPlatforms = true;
                    break;
                }
            }

            bool onCollision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);

            if (dropDown)
            {
                NPC.velocity.Y += 0.5f;
            }
            else if (onCollision || (onPlatforms && !fallthrough))
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y = 0f;

                if (NPC.velocity.Y > -0.2f)
                    NPC.velocity.Y -= 0.025f;
                else
                    NPC.velocity.Y -= 0.2f;

                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
            }
            else
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y = 0f;

                if (NPC.velocity.Y < 0.1f)
                    NPC.velocity.Y += 0.025f;
                else
                    NPC.velocity.Y += 0.5f;
            }

            if (NPC.velocity.Y > 10f)
                NPC.velocity.Y = 10f;
        }

        private void Movement(Vector2 target, bool goFast = false)
        {
            NPC.direction = NPC.spriteDirection = NPC.Center.X < target.X ? 1 : -1;

            if (Math.Abs(target.X - NPC.Center.X) < NPC.width / 2)
            {
                NPC.velocity.X *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < 0.1f)
                    NPC.velocity.X = 0f;
            }
            else
            {
                float maxwalkSpeed = BaseWalkSpeed;

                if (head == null)
                    maxwalkSpeed *= 1.2f;
                if (arms == null)
                    maxwalkSpeed *= 1.2f;

                if (goFast)
                {
                    maxwalkSpeed *= 3f;
                    if (!FargoSoulsWorld.EternityMode)
                        maxwalkSpeed *= 0.75f;
                }
                else if (!FargoSoulsWorld.MasochistModeReal)
                {
                    maxwalkSpeed *= 0.75f;

                    if ((head != null && head.ai[0] != 0) || (arms != null && arms.ai[0] != 0))
                        maxwalkSpeed *= 0.5f;
                }

                if (NPC.dontTakeDamage)
                    maxwalkSpeed *= 0.75f;

                int walkModifier = FargoSoulsWorld.EternityMode ? 30 : 40;
                if (FargoSoulsWorld.MasochistModeReal || arms == null || head == null)
                    walkModifier = 20;

                if (NPC.direction > 0)
                    NPC.velocity.X = (NPC.velocity.X * walkModifier + maxwalkSpeed) / (walkModifier + 1);
                else
                    NPC.velocity.X = (NPC.velocity.X * walkModifier - maxwalkSpeed) / (walkModifier + 1);
            }

            TileCollision(target.Y > NPC.Bottom.Y, Math.Abs(target.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < target.Y);
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;

                NPC.TargetClosest(false);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    head = FargoSoulsUtil.NPCExists(FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TrojanSquirrelHead>(), NPC.whoAmI, target: NPC.target));
                    arms = FargoSoulsUtil.NPCExists(FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TrojanSquirrelArms>(), NPC.whoAmI, target: NPC.target));
                }

                //drop summon
                if (FargoSoulsWorld.EternityMode && !FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel] && Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<SquirrelCoatofArms>());

                //start by jumping
                NPC.ai[0] = 1f;
                NPC.ai[3] = 1f;

                for (int i = 0; i < 80; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default(Color), 4f);
                    Main.dust[d].velocity.Y -= 1.5f;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }

                FargoSoulsUtil.GrossVanillaDodgeDust(NPC);

                SoundEngine.PlaySound(SoundID.Roar, Main.player[NPC.target].Center);
            }

            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

            bool despawn = false;

            switch ((int)NPC.ai[0])
            {
                case 0: //mourning wood movement
                    {
                        Vector2 target = player.Bottom - Vector2.UnitY;
                        if (NPC.localAI[0] > 0) //doing running attack
                        {
                            NPC.localAI[0] -= 1f;

                            float distance = NPC.Center.X - target.X;
                            bool passedTarget = Math.Sign(distance) == NPC.localAI[1];
                            if (passedTarget && Math.Abs(distance) > 160)
                                NPC.localAI[0] = 0f;

                            target = new Vector2(NPC.Center.X + 256f * NPC.localAI[1], target.Y);

                            if (NPC.localAI[0] == 0f)
                                NPC.TargetClosest(false);

                            if (FargoSoulsWorld.EternityMode && head == null && NPC.localAI[0] % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Top.X, NPC.Top.Y, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5),
                                    Main.rand.Next(326, 329), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 90;
                            }
                        }
                        else if (!NPC.HasValidTarget || NPC.Distance(player.Center) > (FargoSoulsWorld.EternityMode ? 1600 : 2400))
                        {
                            target = NPC.Center + new Vector2(256f * Math.Sign(NPC.Center.X - player.Center.X), -128);

                            NPC.TargetClosest(false);

                            despawn = true;
                        }

                        if (Math.Abs(NPC.velocity.Y) < 0.05f && NPC.localAI[3] >= 2)
                        {
                            if (NPC.localAI[3] == 2)
                            {
                                NPC.localAI[3] = 0f;
                            }
                            else
                            {
                                NPC.localAI[3] -= 1;
                                NPC.ai[0] = 1f;
                                NPC.ai[3] = 1f;
                            }

                            if (FargoSoulsWorld.MasochistModeReal)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                                ExplodeAttack();
                            }
                        }

                        bool goFast = despawn || NPC.localAI[0] > 0;
                        Movement(target, goFast);

                        if (arms != null && (NPC.localAI[3] == -1 || NPC.localAI[3] == 1)) //from arms
                            NPC.direction = NPC.spriteDirection = (int)NPC.localAI[3];

                        bool canDoAttacks = FargoSoulsWorld.EternityMode && !goFast;
                        if (canDoAttacks) //decide next action
                        {
                            float increment = 1f;
                            if (head == null)
                                increment += 0.5f;
                            if (arms == null)
                                increment += 0.5f;
                            if (FargoSoulsWorld.MasochistModeReal)
                                increment += 1f;
                            if (NPC.dontTakeDamage)
                                increment /= 2;

                            if (target.Y > NPC.Top.Y)
                                NPC.ai[1] += increment;
                            else
                                NPC.ai[2] += increment;

                            if (Math.Abs(NPC.velocity.Y) < 0.05f)
                            {
                                //its structured like this to ensure body picks the right attack for the situation after being delayed by head/arms
                                bool canProceed = !(head != null && head.ai[0] != 0) && !(arms != null && arms.ai[0] != 0);

                                int threshold = 300;
                                if (NPC.ai[1] > threshold)
                                {
                                    if (canProceed)
                                    {
                                        NPC.ai[0] = 1f;
                                        NPC.ai[1] = 0f;
                                        //NPC.ai[2] = 0f;
                                        NPC.ai[3] = 0f;
                                        NPC.localAI[0] = 0f;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.ai[1] -= 10f;
                                    }
                                }

                                if (NPC.ai[2] > threshold)
                                {
                                    if (canProceed)
                                    {
                                        NPC.ai[0] = 1f;
                                        //NPC.ai[1] = 0f;
                                        NPC.ai[2] = 0f;
                                        NPC.ai[3] = 1f;
                                        NPC.localAI[0] = 0f;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.ai[2] -= 10f;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 1: //telegraph something
                    {
                        NPC.velocity.X *= 0.9f;

                        TileCollision(player.Bottom.Y - 1 > NPC.Bottom.Y, Math.Abs(player.Center.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < player.Bottom.Y - 1);

                        int threshold = 120;
                        if (FargoSoulsWorld.EternityMode)
                        {
                            if (head == null)
                                threshold -= 20;
                            if (arms == null)
                                threshold -= 20;
                            if (head == null && arms == null)
                                threshold -= 30;
                        }
                        if (FargoSoulsWorld.MasochistModeReal || NPC.localAI[3] >= 2)
                            threshold -= 20;

                        if (++NPC.localAI[0] > threshold)
                        {
                            NPC.localAI[0] = 0f;
                            NPC.netUpdate = true;

                            if (NPC.ai[3] == 0f)
                            {
                                NPC.ai[0] = 0f;

                                NPC.localAI[0] = 300f;
                                NPC.localAI[1] = Math.Sign(player.Center.X - NPC.Center.X);
                                NPC.localAI[2] = player.Center.X;
                            }
                            else
                            {
                                NPC.ai[0] = 2f;
                            }
                        }
                    }
                    break;

                case 2: //jump
                    {
                        const float gravity = 0.4f;
                        float time = FargoSoulsWorld.EternityMode && arms == null ? 60f : 90f;

                        if (NPC.localAI[0]++ == 0)
                        {
                            Vector2 distance = player.Top - NPC.Bottom;

                            if (FargoSoulsWorld.EternityMode && arms == null)
                            {
                                distance.X += NPC.width * Math.Sign(player.Center.X - NPC.Center.X);

                                if (NPC.localAI[3] < 2)
                                {
                                    NPC.localAI[3] = 2; //flag to stomp again on landing
                                    if (head == null)
                                        NPC.localAI[3] += 2; //flag to do more stomps
                                }

                                ExplodeAttack();
                            }

                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            NPC.velocity = distance;

                            NPC.netUpdate = true;

                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                        }
                        else
                        {
                            NPC.velocity.Y += gravity;
                        }

                        if (NPC.localAI[0] > time)
                        {
                            NPC.TargetClosest(false);

                            NPC.velocity.X = Utils.Clamp(NPC.velocity.X, -20, 20);
                            NPC.velocity.Y = Utils.Clamp(NPC.velocity.Y, -10, 10);

                            NPC.ai[0] = 0f;
                            NPC.localAI[0] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }

            if (despawn)
            {
                if (NPC.timeLeft > 60)
                    NPC.timeLeft = 60;
            }
            else
            {
                if (NPC.timeLeft < 600)
                    NPC.timeLeft = 600;
            }

            if (head == null)
            {
                Vector2 pos = NPC.Top;
                pos.X += 2f * 16f * NPC.direction;
                pos.Y -= 8f;

                int width = 4 * 16;
                int height = 2 * 16;

                pos.X -= width / 2f;
                pos.Y -= height / 2f;

                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default(Color), 2.5f);
                    Main.dust[d].velocity.Y -= 1.5f;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }

                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Torch, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 3f;
                    Main.dust[d].velocity *= 1.5f;
                }
            }
            else
            {
                lifeMaxHead = head.lifeMax;
                head = FargoSoulsUtil.NPCExists(head.whoAmI, ModContent.NPCType<TrojanSquirrelHead>());
            }

            if (arms == null)
            {
                Vector2 pos = NPC.Center;
                pos.X -= 16f * NPC.direction;
                pos.Y -= 3f * 16f;

                int width = 2 * 16;
                int height = 2 * 16;

                pos.X -= width / 2f;
                pos.Y -= height / 2f;

                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default(Color), 1.5f);
                    Main.dust[d].noGravity = true;
                }

                if (Main.rand.NextBool())
                {
                    int d2 = Dust.NewDust(pos, width, height, DustID.Torch, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[d2].noGravity = true;
                }
            }
            else
            {
                lifeMaxArms = arms.lifeMax;
                arms = FargoSoulsUtil.NPCExists(arms.whoAmI, ModContent.NPCType<TrojanSquirrelArms>());
            }

            if (NPC.life < NPC.lifeMax / 2 && Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default(Color), 4f);
                Main.dust[d].velocity.Y -= 1.5f;
                Main.dust[d].velocity *= 1.5f;
                Main.dust[d].noGravity = true;
            }

            if (FargoSoulsWorld.EternityMode)
            {
                bool wasImmune = NPC.dontTakeDamage;
                NPC.dontTakeDamage = NPC.life < NPC.lifeMax / 2 && (head != null || arms != null);

                if (wasImmune != NPC.dontTakeDamage)
                {
                    for (int i = 0; i < 6; i++)
                        ExplodeDust(NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)));
                }
            }
            else
            {
                NPC.dontTakeDamage = false;
            }
        }

        private void ExplodeAttack()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float offsetX = NPC.width;
                const float offsetY = 65;
                int max = FargoSoulsWorld.MasochistModeReal ? 4 : 2;
                for (int i = -max; i <= max; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(offsetX * i, -offsetY), Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer);
            }
        }

        private void ExplodeDust(Vector2 center)
        {
            SoundEngine.PlaySound(SoundID.Item14, center);

            const int width = 32;
            const int height = 32;

            Vector2 pos = center - new Vector2(width, height) / 2f;

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(pos, width, height, DustID.Smoke, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(pos, width, height, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;

                dust = Dust.NewDust(pos, width, height, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 3; j++)
            {
                int gore = Gore.NewGore(NPC.GetSource_FromThis(), center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch ((int)NPC.ai[0])
            {
                case 0:
                    {
                        NPC.frameCounter += 1f / BaseWalkSpeed * Math.Abs(NPC.velocity.X);

                        if (NPC.frameCounter > 2.5f) //walking animation
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                        }

                        if (NPC.frame.Y >= frameHeight * 6)
                            NPC.frame.Y = 0;

                        if (arms != null && arms.ai[0] == 1 && arms.ai[3] == 1)
                            NPC.frame.Y = frameHeight * 6;

                        if (NPC.velocity.X == 0)
                            NPC.frame.Y = frameHeight; //stationary sprite if standing still

                        if (NPC.velocity.Y > 4)
                            NPC.frame.Y = frameHeight * 7; //jumping
                    }
                    break;

                case 1:
                    NPC.frame.Y = frameHeight * 6; //crouching for jump
                    break;

                case 2:
                    NPC.frame.Y = frameHeight * 7; //jumping
                    break;

                default:
                    goto case 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 3; i <= 7; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore{i}").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel], -1);

            if (ModContent.TryFind("Fargowiltas", "Squirrel", out ModNPC squrrl) && !NPC.AnyNPCs(squrrl.Type))
                FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, squrrl.Type);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TrojanSquirrelBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TrojanSquirrelTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TrojanSquirrelRelic>()));

            LeadingConditionRule rule = new LeadingConditionRule(new Conditions.NotExpert());
            rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<TreeSword>(), ModContent.ItemType<MountedAcornGun>(), ModContent.ItemType<SnowballStaff>(), ModContent.ItemType<KamikazeSquirrelStaff>()));
            rule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ItemID.Squirrel,
                ItemID.SquirrelRed
                //ItemID.SquirrelGold,
                //ItemID.GemSquirrelAmber,
                //ItemID.GemSquirrelAmethyst,
                //ItemID.GemSquirrelDiamond,
                //ItemID.GemSquirrelEmerald,
                //ItemID.GemSquirrelRuby,
                //ItemID.GemSquirrelSapphire,
                //ItemID.GemSquirrelTopaz
            ));
            rule.OnSuccess(ItemDropRule.Common(ItemID.WoodenCrate, 1, 1, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.HerbBag, 1, 1, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.Acorn, 1, 100, 100));
            rule.OnSuccess(ItemDropRule.Common(ModContent.Find<ModItem>("Fargowiltas", "LumberJaxe").Type, 10));

            npcLoot.Add(rule);
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
    }
}
