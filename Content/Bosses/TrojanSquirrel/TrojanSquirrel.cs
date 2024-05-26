using FargowiltasSouls.Content.BossBars;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Placables.Relics;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.TrojanSquirrel
{
    public abstract class TrojanSquirrelPart : ModNPC
    {
        protected int baseWidth;
        protected int baseHeight;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Trojan Squirrel");

            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 3;

            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                    ModContent.BuffType<LethargicBuff>(),
                    ModContent.BuffType<ClippedWingsBuff>()
            ]);
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
                NPC.scale += 1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(NPC.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            NPC.scale = reader.ReadSingle();
        }

        public override void PostAI()
        {
            base.PostAI();

            if (this is TrojanSquirrel)
                NPC.position = NPC.Bottom;
            NPC.width = (int)(baseWidth * NPC.scale);
            NPC.height = (int)(baseHeight * NPC.scale);
            if (this is TrojanSquirrel)
                NPC.Bottom = NPC.position;
        }
    }

    public abstract class TrojanSquirrelLimb : TrojanSquirrelPart
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            this.ExcludeFromBestiary();
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
                if (FargoSoulsUtil.HostCheck)
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
        public bool Trail => body.ai[0] == 0 && body.localAI[0] > 0; //while charging
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

            if (Trail)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++) //math.min to safeguard against uncached trail
                {
                    float oldrot = NPC.oldRot[i];
                    Vector2 oldCenter = body.oldPos[i] + body.Size / 2;
                    DrawData oldGlow = new(texture2D13, oldCenter - screenPos + new Vector2(0f, NPC.gfxOffY - 53 * body.scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26 * (0.5f / i), oldrot, origin2, NPC.scale, effects, 0);
                    GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                    GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                    oldGlow.Draw(spriteBatch);
                }
            }


            Vector2 center = body.Center;

            Main.EntitySpriteDraw(texture2D13, center - screenPos + new Vector2(0f, NPC.gfxOffY - 53 * body.scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            return false;
        }
    }

    [AutoloadBossHead]
    public class TrojanSquirrel : TrojanSquirrelPart
    {
        private const float BaseWalkSpeed = 4f;
        string TownNPCName;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = $"FargowiltasSouls/Content/Bosses/TrojanSquirrel/{Name}_Still",
                Position = new Vector2(16 * 4, 16 * 4),
                PortraitPositionXOverride = 16 * 1.5f,
                PortraitPositionYOverride = 16 * 3
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            ]);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 800;

            NPC.width = baseWidth = 100;
            NPC.height = baseHeight = 120; //234

            NPC.value = Item.buyPrice(silver: 75);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/TrojanSquirrel") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.BossBar = ModContent.GetInstance<CompositeBossBar>();
        }
        /*
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        */
        public NPC head;
        public NPC arms;
        public int lifeMaxHead;
        public int lifeMaxArms;

        private bool spawned;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(head is NPC ? head.whoAmI : -1);
            writer.Write(arms is NPC ? arms.whoAmI : -1);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

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
                    TownNPCName = Main.npc[n].GivenName;

                    Main.npc[n].life = 0;
                    Main.npc[n].active = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }

            //spawnfrag fix
            int p = Player.FindClosest(NPC.Center, 0, 0);
            if (p.IsWithinBounds(Main.maxPlayers))
            {
                Player player = Main.player[p];
                if (player != null && player.active && !player.dead)
                {
                    if (NPC.Distance(player.Center) < 400)
                    {
                        NPC.Center = player.Center - Vector2.UnitX * 1000 * player.direction;
                    }
                };
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
            else if (onCollision || onPlatforms && !fallthrough)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y = 0f;

                if (NPC.velocity.Y > -0.2f)
                    NPC.velocity.Y -= 0.025f;
                else
                    NPC.velocity.Y -= 0.2f;

                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;

                if (Jumping) //landing effects
                {
                    SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.4f }, NPC.Bottom);
                    for (int i = 0; i < 4; i++)
                    {
                        int side = i % 2 == 0 ? 1 : -1;
                        float speed = Main.rand.NextFloat(4, 6);
                        Vector2 vel = (Vector2.UnitX * side * speed).RotatedByRandom(MathHelper.Pi / 11);
                        Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom - Vector2.UnitY * 10, vel, Main.rand.Next(11, 14), Scale: 2f);
                    }
                    Jumping = false;
                }
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

            Player target = Main.player[NPC.target];
            //anti-leaving soon when terrain above you
            if (onCollision && target != null && target.active && !target.dead && target.Center.Y > NPC.Center.Y + NPC.height * 1.5f && Math.Abs(target.Center.X - NPC.Center.X) < 400)
            {
                NPC.velocity.Y += 3f;
            }
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
                float maxwalkSpeed = BaseWalkSpeed * NPC.scale;

                if (head == null)
                    maxwalkSpeed *= 1.2f;
                if (arms == null)
                    maxwalkSpeed *= 1.2f;

                if (goFast)
                {
                    maxwalkSpeed *= 3f;
                    if (!WorldSavingSystem.EternityMode)
                        maxwalkSpeed *= 0.75f;
                }
                else if (!WorldSavingSystem.MasochistModeReal)
                {
                    maxwalkSpeed *= 0.75f;

                    if (head != null && head.ai[0] != 0 || arms != null && arms.ai[0] != 0)
                        maxwalkSpeed *= 0.5f;
                }

                if (NPC.dontTakeDamage)
                    maxwalkSpeed *= 0.75f;

                int walkModifier = WorldSavingSystem.EternityMode ? 30 : 40;
                if (WorldSavingSystem.MasochistModeReal || arms == null || head == null)
                    walkModifier = 20;

                if (NPC.direction > 0)
                    NPC.velocity.X = (NPC.velocity.X * walkModifier + maxwalkSpeed) / (walkModifier + 1);
                else
                    NPC.velocity.X = (NPC.velocity.X * walkModifier - maxwalkSpeed) / (walkModifier + 1);
            }

            TileCollision(target.Y > NPC.Bottom.Y, Math.Abs(target.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < target.Y);
        }
        public bool Jumping = false;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;

                NPC.TargetClosest(false);

                if (FargoSoulsUtil.HostCheck)
                {
                    head = FargoSoulsUtil.NPCExists(FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TrojanSquirrelHead>(), NPC.whoAmI, target: NPC.target));
                    arms = FargoSoulsUtil.NPCExists(FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TrojanSquirrelArms>(), NPC.whoAmI, target: NPC.target));
                }

                //drop summon
                if (WorldSavingSystem.EternityMode && !WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel] && FargoSoulsUtil.HostCheck)
                    Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<SquirrelCoatofArms>());

                //start by jumping
                NPC.ai[0] = 1f;
                NPC.ai[3] = 1f;

                for (int i = 0; i < 80; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default, 4f);
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

                            if (NPC.localAI[0] % 10 == 0) //hermes boot clouds
                            {
                                SoundEngine.PlaySound(SoundID.Run);
                                Vector2 vel = (-NPC.velocity).RotatedByRandom(MathHelper.Pi / 11f);
                                vel /= 2;
                                Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), NPC.Bottom - Vector2.UnitY * 10, vel, Main.rand.Next(11, 14), Scale: Main.rand.NextFloat(1.5f, 2f));
                                gore.timeLeft /= 2;
                            }

                            float distance = NPC.Center.X - target.X;
                            bool passedTarget = Math.Sign(distance) == NPC.localAI[1];
                            if (passedTarget && Math.Abs(distance) > 160)
                                NPC.localAI[0] = 0f;

                            target = new Vector2(NPC.Center.X + 256f * NPC.localAI[1], target.Y);

                            if (NPC.localAI[0] == 0f)
                                NPC.TargetClosest(false);

                            if (WorldSavingSystem.EternityMode && head == null && NPC.localAI[0] % 3 == 0 && FargoSoulsUtil.HostCheck)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Top.X, NPC.Top.Y, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-3),
                                    Main.rand.Next(326, 329), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 90;
                            }
                        }
                        else if (!NPC.HasValidTarget || NPC.Distance(player.Center) > 2400)
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

                            if (WorldSavingSystem.MasochistModeReal)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                                ExplodeAttack();
                            }
                        }

                        bool goFast = despawn || NPC.localAI[0] > 0;
                        Movement(target, goFast);

                        if (arms != null && (NPC.localAI[3] == -1 || NPC.localAI[3] == 1)) //from arms
                            NPC.direction = NPC.spriteDirection = (int)NPC.localAI[3];

                        bool canDoAttacks = WorldSavingSystem.EternityMode && !goFast;
                        if (canDoAttacks) //decide next action
                        {
                            float increment = 1f;
                            if (head == null)
                                increment += 0.5f;
                            if (arms == null)
                                increment += 0.5f;
                            if (WorldSavingSystem.MasochistModeReal)
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
                        NPC.velocity.X = 0;

                        TileCollision(player.Bottom.Y - 1 > NPC.Bottom.Y, Math.Abs(player.Center.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < player.Bottom.Y - 1);

                        int threshold = 105;
                        if (WorldSavingSystem.EternityMode)
                        {
                            if (head == null)
                                threshold -= 20;
                            if (arms == null)
                                threshold -= 20;
                            if (head == null && arms == null)
                                threshold -= 30;
                        }
                        if (WorldSavingSystem.MasochistModeReal || NPC.localAI[3] >= 2)
                            threshold -= 20;

                        if (NPC.ai[3] != 0f) //telegraphing jump
                        {
                            int dir = NPC.localAI[0] % 2 == 0 ? 1 : -1;
                            int maxShake = 8;
                            float shake = dir * maxShake * (NPC.localAI[0] / threshold);
                            NPC.position.X += shake;
                        }

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
                        float time = WorldSavingSystem.EternityMode && arms == null ? 60f : 90f;

                        if (NPC.localAI[0]++ == 0)
                        {
                            Vector2 distance = player.Top - NPC.Bottom;

                            if (WorldSavingSystem.EternityMode && arms == null)
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

                            distance.X /= time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            NPC.velocity = distance;

                            NPC.netUpdate = true;

                            SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);

                            for (int i = 0; i < 4; i++)
                            {
                                int side = i % 2 == 0 ? 1 : -1;
                                float speed = Main.rand.NextFloat(4, 6);
                                Vector2 vel = (Vector2.UnitX * side * speed).RotatedByRandom(MathHelper.Pi / 11);
                                Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom - Vector2.UnitY * 10, vel, Main.rand.Next(11, 14), Scale: 2f);
                            }

                            Jumping = true;
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
                    int d = Dust.NewDust(pos, width, height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default, 2.5f);
                    Main.dust[d].velocity.Y -= 1.5f;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }

                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Torch, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default, 2.5f);
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
                    int d = Dust.NewDust(pos, width, height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default, 1.5f);
                    Main.dust[d].noGravity = true;
                }

                if (Main.rand.NextBool())
                {
                    int d2 = Dust.NewDust(pos, width, height, DustID.Torch, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default, 3f);
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
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default, 4f);
                Main.dust[d].velocity.Y -= 1.5f;
                Main.dust[d].velocity *= 1.5f;
                Main.dust[d].noGravity = true;
            }

            if (WorldSavingSystem.EternityMode)
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

            if (WorldSavingSystem.MasochistModeReal && Main.getGoodWorld && FargoSoulsUtil.HostCheck)
            {
                int[] edibleTiles =
                [
                    TileID.WoodBlock,
                    TileID.AshWood,
                    TileID.BorealWood,
                    TileID.DynastyWood,
                    TileID.LivingWood,
                    TileID.PalmWood,
                    TileID.SpookyWood,
                    TileID.Ebonwood,
                    TileID.Pearlwood,
                    TileID.Shadewood,
                    TileID.Trees,
                    TileID.TreeAsh,
                    TileID.ChristmasTree,
                    TileID.PalmTree,
                    TileID.PineTree,
                    TileID.VanityTreeSakura,
                    TileID.VanityTreeYellowWillow,
                    TileID.LivingMahoganyLeaves
                ];
                for (float x = NPC.position.X; x < NPC.BottomRight.X; x += 16)
                {
                    for (float y = NPC.position.Y; y < NPC.BottomRight.Y; y += 16)
                    {
                        Tile tile = Framing.GetTileSafely(new Vector2(x, y));
                        if (tile != null && edibleTiles.Contains(tile.TileType))
                        {
                            int xCoord = (int)x / 16;
                            int yCoord = (int)y / 16;
                            WorldGen.KillTile(xCoord, yCoord, noItem: true);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendTileSquare(-1, xCoord, yCoord, 1);

                            NPC.scale += 0.01f;
                            NPC.netUpdate = true;
                            if (head is NPC)
                            {
                                head.scale += 0.01f;
                                head.netUpdate = true;
                            }
                            if (arms is NPC)
                            {
                                arms.scale += 0.01f;
                                arms.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        private void ExplodeAttack()
        {
            if (FargoSoulsUtil.HostCheck)
            {
                float offsetX = NPC.width;
                const float offsetY = 65;
                int max = WorldSavingSystem.MasochistModeReal ? 4 : 2;
                for (int i = -max; i <= max; i++)
                {
                    Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(offsetX * i, -offsetY), Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                    p.friendly = false;
                    p.hostile = true;
                }

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
                int dust = Dust.NewDust(pos, width, height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(pos, width, height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;

                dust = Dust.NewDust(pos, width, height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 3; j++)
            {
                int gore = Gore.NewGore(NPC.GetSource_FromThis(), center, default, Main.rand.Next(61, 64));
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
                        NPC.frameCounter += 1f / BaseWalkSpeed / NPC.scale * Math.Abs(NPC.velocity.X);

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

        public override void HitEffect(NPC.HitInfo hit)
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
            NPC.SetEventFlagCleared(ref WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel], -1);

            if (ModContent.TryFind("Fargowiltas", "Squirrel", out ModNPC squrrl) && !NPC.AnyNPCs(squrrl.Type))
            {
                int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, squrrl.Type);
                if (n != Main.maxNPCs)
                {
                    Main.npc[n].homeless = true;
                    if (TownNPCName != default)
                        Main.npc[n].GivenName = TownNPCName;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TrojanSquirrelBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TrojanSquirrelTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TrojanSquirrelRelic>()));

            LeadingConditionRule rule = new(new Conditions.NotExpert());
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
            rule.OnSuccess(ItemDropRule.Common(ItemID.SquirrelHook));
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
