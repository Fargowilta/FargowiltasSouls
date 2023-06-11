//JAVYZ TODO: Banished Baron
/*
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using FargowiltasSouls.Buffs.Masomode;
using Terraria.GameContent.Bestiary;
using FargowiltasSouls.Patreon.ManliestDove;
using System.Reflection;
using System.Linq;
using Terraria.Audio;
using FargowiltasSouls.Items.BossBags;
using FargowiltasSouls.Items.Weapons.Challengers;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using System.Diagnostics;
using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Challengers;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.NPCs.Challengers
{
    [AutoloadBossHead]
    public class BanishedBaron : ModNPC
    {
        Player player => Main.player[NPC.target];
        //TODO: re-enable boss checklist compat, localizationhelper addSpawnInfo

        SoundStyle BaronRoar = new SoundStyle("FargowiltasSouls/Sounds/BaronRoar");
        SoundStyle BaronYell = new SoundStyle("FargowiltasSouls/Sounds/BaronYell");

        #region Variables
        private enum StateEnum //ALL states
        {
            Opening,
            Phase2Transition,
            P1BigNuke,
            P1RocketStorm,
            P1SurfaceMines,
            P1FadeDash,
            P1SineSwim,
            P2PredictiveDash,
            P2CarpetBomb,
            P2RocketStorm,
            P2SpinDash,
            P2MineFlurry,
            Swim,
        }

        private static List<int> P1Attacks = new List<int>() //these are randomly chosen attacks in p1
        {
            (int)StateEnum.P1BigNuke,
            (int)StateEnum.P1RocketStorm,
            (int)StateEnum.P1SurfaceMines,
            (int)StateEnum.P1FadeDash,
            (int)StateEnum.P1SineSwim,
        };
        private static List<int> P2Attacks = new List<int>() //these are randomly chosen attacks in p2
        {
            (int)StateEnum.P2PredictiveDash,
            (int)StateEnum.P2CarpetBomb,
            (int)StateEnum.P2RocketStorm,
            (int)StateEnum.P2SpinDash,
            (int)StateEnum.P2MineFlurry,
        };

        private bool Attacking = true;
        bool HitPlayer = true;
        private int Trail = 0;

        private int Frame = 0;
        private int Phase = 1;
        private int Anim = 0;
        private int WaterwallDistance = 0;
        private int dustcounter = 0;

        private List<int> availablestates = new List<int>(0);

        private Vector2 LockVector1 = Vector2.Zero;
        private Vector2 LockVector2 = Vector2.Zero;
        private Vector2 WaterwallCenter = Vector2.Zero;

        //NPC.ai[] overrides
        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];

        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banished Baron");
            Main.npcFrameCount[NPC.type] = 19;
            NPCID.Sets.TrailCacheLength[NPC.type] = 18; //decrease later if not needed
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 26500;
            NPC.defense = 0;
            NPC.damage = 70;
            NPC.knockBackResist = 0f;
            NPC.width = 194;
            NPC.height = 132;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = new SoundStyle("FargowiltasSouls/Sounds/BaronDeath");

            Music = MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.value = Item.buyPrice(0, 2);

        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {

        }
        #endregion
        #region Overrides

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {

            return true;
        }
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (HitPlayer)
            {
                return base.CanHitPlayer(target, ref CooldownSlot);
            }
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (HitPlayer)
            {
                return base.CanHitNPC(target);
            }
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void HitEffect(int hitDirection, double HitDamage)
        {
            if (NPC.life <= 0)
            {
                //gore

                return;
            }
        }
        public override bool CheckDead()
        {
            return base.CheckDead();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            float rot = NPC.rotation + (NPC.direction == 1 ? 0 :MathHelper.Pi);
            int currentFrame = NPC.frame.Y / (bodytexture.Height / Main.npcFrameCount[NPC.type]);
            SpriteEffects flip = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            for (int i = 0; i < Math.Min(Trail, NPCID.Sets.TrailCacheLength[NPC.type]); i++) //math.min to safeguard against uncached trail
            {
                Vector2 value4 = NPC.oldPos[i];
                int oldFrame = Frame;
                Rectangle oldRectangle = new Rectangle(0, oldFrame * bodytexture.Height / Main.npcFrameCount[NPC.type], bodytexture.Width, bodytexture.Height / Main.npcFrameCount[NPC.type]);
                DrawData oldGlow = new DrawData(bodytexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldRectangle), NPC.GetAlpha(drawColor) * (0.5f / i), rot, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, flip, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }

            spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: NPC.GetAlpha(drawColor), rotation: rot, scale: NPC.scale, effects: flip, layerDepth: 0f);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            //FRAMES OF ANIMATIONS:
            //P1 Normal fly loop: 0-5 (Anim 0)
            //P1 Open mouth: 6-7 (Anim 1)
            //Unused: 8-10
            //P2 Normal fly loop: 11-16 (Anim 0)
            //P2 Open mouth: 17-18 (Anim 1)
            double fpf = 60 / 10; //  60/fps
            int StartFrame = 0;
            int EndFrame = 5;
            switch (Phase)
            {
                case 1:
                    switch (Anim)
                    {
                        case 0:
                            //no change required, default values
                            break;
                        case 1:
                            StartFrame = 6;
                            EndFrame = 7;
                            break;
                    }
                    break;
                case 2:
                    switch (Anim)
                    {
                        case 0:
                            StartFrame = 11;
                            EndFrame = 16;
                            break;
                        case 1:
                            StartFrame = 17;
                            EndFrame = 18;
                            break;
                    }
                    break;
            }
            NPC.spriteDirection = NPC.direction;
            if (++NPC.frameCounter >= fpf)
            {
                if (++Frame > EndFrame || Frame < StartFrame)
                {
                    Frame = StartFrame;
                }
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = frameHeight * Frame;
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.BanishedBaron], -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //TODO: Add loot
            //npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BanishedBaronBag>()));
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BanishedBaronTrophy>(), 10));

            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BanishedBaronRelic>()));

            //LeadingConditionRule rule = new LeadingConditionRule(new Conditions.NotExpert());
            //rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<EnchantedLifeblade>(), ModContent.ItemType<Lightslinger>(), ModContent.ItemType<CrystallineCongregation>(), ModContent.ItemType<KamikazePixieStaff>()));
            //rule.OnSuccess(ItemDropRule.Common(ItemID.HallowedFishingCrateHard, 1, 1, 5)); //hallowed crate
            //rule.OnSuccess(ItemDropRule.Common(ItemID.SoulofLight, 1, 1, 3));
            //rule.OnSuccess(ItemDropRule.Common(ItemID.PixieDust, 1, 15, 25));

            //npcLoot.Add(rule);
        }
        #endregion
        #region AI
        public override void AI()
        {
            //Defaults
            NPC.noTileCollide = Phase == 2 || FargoSoulsWorld.MasochistModeReal || Collision.SolidCollision(NPC.Center, 1, 1) || !Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height); //no tile collide in p2, tile collide in p1 except if in tiles, or if obstructed
            NPC.defense = NPC.defDefense;
            NPC.direction = NPC.spriteDirection = NPC.rotation.ToRotationVector2().X > 0 ? 1 : -1;

            if (HitPlayer)
            {
                Lighting.AddLight(NPC.Center, TorchID.Pink);
            }

            if (!AliveCheck(player))
                return;

            if (State == 0 && Timer == 0) //opening
            {
                NPC.position = player.Center + new Vector2(player.direction * 300, -100) - NPC.Size / 2;
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
                LockVector1 = player.Center;
                NPC.velocity = Vector2.Zero;
            }
            //Phase 1/2 specific passive attributes
            switch (Phase)
            {
                case 1:
                    if (State != (float)StateEnum.Opening || Timer >= 60) //also do it during opening but only after 1 sec
                    {
                        //inflict all nearby permanently with Please Go Underwater debuff that PRESSES them down
                        if (NPC.Distance(Main.LocalPlayer.Center) < 2000)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<BaronsBurden>(), 1);
                        }
                    }
                    if (Collision.WetCollision(NPC.position, NPC.width, NPC.height) && NPC.velocity.Length() > 0)
                    {
                        //stream of water dust behind
                        Dust.NewDust(NPC.Center - NPC.rotation.ToRotationVector2() * new Vector2(NPC.width / 2, 0) - new Vector2(5, 5), 10, 10, DustID.Water, -NPC.velocity.X, -NPC.velocity.Y);
                    }
                    break;
                case 2:
                    if (WaterwallDistance < 1000)
                        WaterwallDistance += 10;
                    WaterWalls(WaterwallCenter, WaterwallDistance);
                    break;
            }
            //Normal looping attack AI

            if (Attacking) //Phases and random attack choosing
            {
                switch (State) //Attack Choices
                {
                    case (float)StateEnum.Opening:
                        Opening();
                        break;
                    case (float)StateEnum.Phase2Transition:
                        Phase2Transition();
                        break;
                    case (float)StateEnum.Swim:
                        Swim();
                        break;
                    case (float)StateEnum.P1BigNuke:
                        P1BigNuke();
                        break;
                    case (float)StateEnum.P1RocketStorm:
                        P1RocketStorm();
                        break;
                    case (float)StateEnum.P1SurfaceMines:
                        P1SurfaceMines();
                        break;
                    case (float)StateEnum.P1FadeDash:
                        P1FadeDash();
                        break;
                    case (float)StateEnum.P1SineSwim:
                        P1SineSwim();
                        break;
                    case (float)StateEnum.P2PredictiveDash:
                        P2PredictiveDash();
                        break;
                    case (float)StateEnum.P2CarpetBomb:
                        P2CarpetBomb();
                        break;
                    case (float)StateEnum.P2RocketStorm:
                        P2RocketStorm();
                        break;
                    case (float)StateEnum.P2SpinDash:
                        P2SpinDash();
                        break;
                    case (float)StateEnum.P2MineFlurry:
                        P2MineFlurry();
                        break;
                    default:
                        StateReset();
                        break;
                }
            }
            Timer++;
        }
        #endregion
        #region States
        void Opening()
        {
            Anim = 1;
            //NPC.rotation = (float)(Math.Sin(MathHelper.ToRadians(Timer) * 16) * MathHelper.Pi/24);
            RotateTowards(player.Center, 0.5f);
            if (Timer == 30)
            {
                SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -24);
            }
            if (Timer > 90)
            {
                Anim = 0;
                StateReset();
            }
        }
        void Phase2Transition()
        {
            
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/BaronHit"), NPC.Center);
                if (!Main.dedServ)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 100;
            }
            if (Timer < 90)
            {
                Vector2 up = NPC.Center + new Vector2(3 * Math.Sign(NPC.DirectionTo(player.Center).X), -10);
                RotateTowards(up, 5f);
            }
            if (Timer == 90)
            {
                NPC.velocity = NPC.rotation.ToRotationVector2() * 20;
                SoundEngine.PlaySound(BaronYell, NPC.Center);
                
                if (player.Center.X < 1000)
                {
                    WaterwallCenter = new Vector2(1000, player.Center.Y); 
                }
                else if (player.Center.X > Main.maxTilesX - 1000)
                {
                    WaterwallCenter = new Vector2(Main.maxTilesX - 1000, player.Center.Y);
                }
                else
                {
                    WaterwallCenter = player.Center;
                }
                WaterwallCenter = Main.screenPosition + new Vector2(Main.screenWidth/2, Main.screenHeight / 2); //not player center to work with map borders
                Phase = 2;
            }
            if (!Collision.WetCollision(NPC.position, NPC.width, NPC.height) && Timer > 90)
            {
                NPC.velocity *= 0.95f;
                if (NPC.velocity.Length() < 0.1f)
                {
                    availablestates.Clear();
                    StateReset();

                }

                if (Main.LocalPlayer.wet)
                {
                    Main.LocalPlayer.velocity.Y -= 0.5f;
                }
            }
            
        }
        #region Phase 1 Attacks
        void Swim()
        {
            if (Timer == 1)
            {
                LockVector1 = player.Center;
            }

            if (Vector2.Distance(NPC.Center, LockVector1) < 25 || Timer > 150 || AI3 == 1)
            {

                AI3 = 1;
                NPC.velocity *= 0.935f;
                RotateTowards(player.Center, 3);
                if (NPC.velocity.Length() < 0.1f)
                {
                    NPC.velocity = Vector2.Zero;
                    StateReset();
                }
            }
            else
            {
                Vector2 vectorToIdlePosition = LockVector1 - NPC.Center;
                float speed = 20f;
                float inertia = 20f;
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                if (NPC.velocity == Vector2.Zero)
                {
                    NPC.velocity.X = -0.15f;
                    NPC.velocity.Y = -0.05f;
                }
                NPC.rotation = NPC.velocity.ToRotation();
            }
        }

        void WaterWalls(Vector2 location, int threshold)
        {
            //Aura
            if (true)
            {
                const int DistBetweenDusts = 10;
                if (dustcounter > 5)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        for (int l = -80; l < 80; l += 2)
                        {
                            int DustX2 = (int)(location.X + (threshold * i));
                            int DustY2 = (int)(Main.LocalPlayer.Center.Y + (l * DistBetweenDusts));
                            int DustType = DustID.WaterCandle;
                            int d = Dust.NewDust(new Vector2(DustX2, DustY2), 1, 1, DustType, Scale: 1.5f);
                            Main.dust[d].noGravity = true;
                        }
                    }
                    
                    dustcounter = 0;
                }
                dustcounter++;

                
                Player player = Main.LocalPlayer;
                bool OutsideRange = Math.Abs(player.Center.X - location.X) >= threshold;
                bool OutsideRangex2 = Math.Abs(player.Center.X - location.X) >= threshold * 2;
                bool OutsideRangex4 = Math.Abs(player.Center.X - location.X) >= threshold * 4;
                if (player.active && !player.dead && !player.ghost) //pull into arena
                {
                    if (OutsideRange && !OutsideRangex4)
                    {
                        if (OutsideRangex2)
                        {
                            player.controlLeft = false;
                            player.controlRight = false;
                            player.controlUp = false;
                            player.controlDown = false;
                            player.controlUseItem = false;
                            player.controlUseTile = false;
                            player.controlJump = false;
                            player.controlHook = false;
                            if (player.grapCount > 0)
                                player.RemoveAllGrapplingHooks();
                            if (player.mount.Active)
                                player.mount.Dismount(player);
                            player.velocity.X = 0f;
                            player.velocity.Y = -0.4f;
                            player.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = 2;
                        }

                        Vector2 movement = new Vector2(location.X - player.Center.X, 0);
                        float difference = movement.Length() - threshold;
                        movement.Normalize();
                        movement *= difference < 17f ? difference : 17f;
                        player.position += movement;

                        for (int i = 0; i < 10; i++)
                        {
                            int DustType = DustID.WaterCandle;
                            int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default(Color), 1.25f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 5f;
                        }
                    }
                }
            }
        }
        void P1BigNuke()
        {
            if (Timer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, NPC.whoAmI);
            }

            RotateTowards(player.Center, 2);
            if (Timer < 30)
                Anim = 1;
            else
                Anim = 0;
            if (Timer == 30)
            {
                SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.rotation.ToRotationVector2() * 10f;
                    AI2 = Main.rand.NextFloat(150, 220); //nuke duration
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, AI2, player.whoAmI);
                }
            }
            if (Timer > AI2 - 10 && AI2 > 0)
            {
                Anim = 0;
                StateReset();
            }
        }
        void P1RocketStorm()
        {
            RotateTowards(player.Center, 3);
            if (Timer < 60 && Timer % 6 == 0 && Timer > 10)
            {
                Anim = 1;
                SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = (NPC.rotation + (MathHelper.PiOver2 * -NPC.direction) + Main.rand.NextFloat(-MathHelper.PiOver2 / 3, MathHelper.PiOver2 / 3)).ToRotationVector2() * 15;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, player.whoAmI);
                }
            }
            if (Timer > 60)
            {
                Anim = 0;
                StateReset();
            }
        }
        void P1SurfaceMines()
        {
            Anim = 1;
            if (Timer == 1)
            {
                LockVector1 = new Vector2(player.Center.X, NPC.Center.Y);
                AI3 = 1;
                SoundEngine.PlaySound(BaronYell, NPC.Center);
            }
            LockVector1.Y = NPC.Center.Y;
            RotateTowards(LockVector1, 8);
            if (Timer > 10 && Timer % 15 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.rotation.ToRotationVector2() * AI3;
                    vel = new Vector2(vel.Length() * NPC.direction, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI);
                }
                AI3 += 8;
            }
            if (Timer > 80)
            {
                Anim = 0;
                StateReset();
            }
        }
        void P1FadeDash()
        {
            const int ReactionTime = 50;
            if (Timer == 1)
            {
                LockVector1 = new Vector2(NPC.Center.X + Math.Sign(player.Center.X - NPC.Center.X), NPC.Center.Y + Main.rand.NextFloat(-1, 1));
            }
            if (Timer < 60)
            {
                RotateTowards(LockVector1, 2);
                if (NPC.velocity.Length() < 1)
                {
                    NPC.velocity += NPC.rotation.ToRotationVector2() * 0.02f;
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length();
                NPC.Opacity -= 1f / 50;
                if (NPC.Opacity < 0)
                    NPC.Opacity = 0;
                HitPlayer = false;
            }
            if (Timer == 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LockVector1 = player.Center + new Vector2(400, 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                    NPC.rotation = NPC.DirectionTo(LockVector1).ToRotation();
                }
            }
            if (Timer > 60 && Timer < 90) //move Very Fast to "teleport" spot (to avoid tile collision)
            {
                NPC.rotation = NPC.DirectionTo(LockVector1).ToRotation();
                NPC.velocity = ((NPC.Distance(LockVector1) / 10) + 1) * NPC.rotation.ToRotationVector2();
            }
            if (Timer == 90)
            {
                NPC.velocity = Vector2.Zero;
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
            }
            if (Timer > 90 && Timer < 90 + ReactionTime)
            {
                RotateTowards(player.Center, 2);
                if (NPC.velocity.Length() < 3)
                {
                    NPC.velocity += NPC.rotation.ToRotationVector2() * 0.03f;
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length();
                NPC.Opacity += 1f / ReactionTime;
                if (NPC.Opacity < 0)
                    NPC.Opacity = 0;
                HitPlayer = true;
            }
            if (Timer == 90 + ReactionTime)
            {
                SoundEngine.PlaySound(BaronRoar, NPC.Center);
                NPC.velocity = NPC.rotation.ToRotationVector2() * 40f;
            }
            if (Timer > 90 + ReactionTime)
            {
                NPC.velocity *= 0.97f;
            }
            if (Timer > 90 + ReactionTime && NPC.velocity.Length() < 0.5)
            {
                StateReset();
            }
        }
        void P1SineSwim()
        {
            int Duration = FargoSoulsWorld.MasochistModeReal ? 100 : FargoSoulsWorld.EternityMode ? 150 : 180;
            const int Waves = 2;
            const int Ymax = 600;
            const int Xstart = 800;

            float prog = Timer / Duration;

            NPC.noTileCollide = true;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                LockVector1 = player.Center + new Vector2(Xstart * AI2, 0);
                SoundEngine.PlaySound(BaronYell, NPC.Center);
            }
            if (AI3 == 0)
            {
                Vector2 target = LockVector1 + new Vector2(-AI2 * prog * (1.75f * Xstart), Ymax * (float)Math.Sin(MathHelper.TwoPi * prog * Waves));
                NPC.rotation = NPC.DirectionTo(target).ToRotation();
                NPC.velocity = NPC.DirectionTo(target) * NPC.Distance(target) / 1.2f;
                if (NPC.velocity.Length() > 20)
                {
                    NPC.velocity *= 20 / NPC.velocity.Length();
                }
                if (prog > 1)
                {
                    AI3 = 1;
                }
            }
            if (AI3 == 1)
            {
                NPC.velocity *= 0.96f;
            }
            if (NPC.velocity.Length() < 1f)
            {
                NPC.noTileCollide = false; //this is probably automatically set in AI anyway but just to make sure
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        #endregion
        #region Phase 2 Attacks
        void P2PredictiveDash()
        {
            float PredictStr = 35;
            if (Timer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, NPC.whoAmI);
            }
            if (Timer < 60)
            {
                NPC.velocity = player.velocity;
                LockVector1 = NPC.DirectionTo(player.Center + (player.velocity * PredictStr)) * PredictStr;
                RotateTowards(NPC.Center + LockVector1, 4);
            }
            if (Timer == 60)
            {
                SoundEngine.PlaySound(BaronRoar, NPC.Center);
                Trail = 8;
                NPC.velocity = LockVector1;
                Vector2 uv = Vector2.Normalize(LockVector1);
                Vector2 lp = player.Center - NPC.Center;
                float lambda = Vector2.Dot(uv, lp);
                LockVector2 = NPC.Center + (uv * lambda);
            }
            if (Timer >= 60 && NPC.velocity.Length() > 5)
            {
                if (Timer % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                    for (int i = -1; i < 2; i += 2)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 v = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v * 20), v * 0.3f, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                        }
                    }
                }
            }

            if (NPC.Distance(LockVector2) <= NPC.velocity.Length())
            {
                AI3 = 1;
            }
            if (AI3 == 1)
            {
                NPC.velocity *= 0.97f;
            }
            if (NPC.velocity.Length() < 0.5f && AI3 == 1)
            {
                Trail = 0;
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2CarpetBomb()
        {
            const int Xstart = 800;
            const int YMax = 500;
            const int YMin = 300;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                
            }
            if (AI3 == 0) //startup, reach starting point
            {
                float x = Xstart * AI2;
                LockVector1 = player.Center + new Vector2(x, -YMin);
                NPC.rotation = NPC.DirectionTo(LockVector1).ToRotation();
                if (NPC.velocity.Length() < 30 + player.velocity.Length())
                {
                    NPC.velocity += 0.5f * NPC.rotation.ToRotationVector2();
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.velocity.Length() * NPC.rotation.ToRotationVector2();
                if (NPC.Distance(LockVector1) < 25)
                {
                    AI3 = 1; //reached point, start attack
                }
            }
            if (AI3 > 0) //arc attack
            {
                const int ArcTime = 90;

                //funky real math i did on paper and translated to code

                Vector2 Curve(float p) //makes a smooth wide curve
                {
                    float x = AI2 * Xstart * (1 - (2 * p));
                    return new Vector2(x, (float)(-YMin - ((YMax - YMin) * Math.Sin(MathHelper.Pi * p))));
                }
                float prog = AI3 / ArcTime;
                
                //float DyDp = (float)(-((YMax - YMin) * Math.Cos(MathHelper.Pi * prog))) * (-1/(2*AI2*Xstart));
                Vector2 curve = Curve(prog);
                Vector2 dydx = Curve(prog + 0.00001f) - curve;
                NPC.rotation = dydx.ToRotation();
                LockVector1 = player.Center + curve; 
                //NPC.rotation = (LockVector1 - NPC.Center).ToRotation();
                //NPC.rotation = NPC.DirectionTo(player.Center).RotatedBy(MathHelper.PiOver2 * AI2).ToRotation();
                NPC.velocity = LockVector1 - NPC.Center;
                AI3++;

                if ((AI3 % (ArcTime/6)) == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = new Vector2(0, Main.rand.Next(15, 25));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                    }
                }

                if (prog >= 1)
                {
                    AI3 = -1; //end attack, start next part
                    SoundEngine.PlaySound(BaronYell, NPC.Center);
                }
            }
            if (AI3 < 0) //home into player after arc
            {
                
                if (NPC.Distance(player.Center) < 300 && AI3 < -30)
                {
                    AI2 = -2;
                }
                if (AI2 == -2)
                {
                    NPC.velocity *= 0.975f;
                }
                else
                {
                    float maxSpeed = 15 + (player.velocity.Length() / 2);
                    if (NPC.velocity.Length() < maxSpeed)
                    {
                        NPC.velocity += NPC.rotation.ToRotationVector2() * 0.02f;
                        NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length() * 1.03f;
                    }
                    else
                    {
                        NPC.velocity = NPC.rotation.ToRotationVector2() * maxSpeed;
                    }
                    float rotSpeed = 2.5f;
                    RotateTowards(player.Center, rotSpeed);
                }
                AI3--;

            }
            if ((AI3 < -30 && NPC.velocity.Length() < 1.25) || AI3 < -220) //second condition is failsafe for circle cheese
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2RocketStorm()
        {
            const int distance = 500;
            float rotModifier = 1;
            float minRot = 0.5f;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                LockVector1 = new Vector2(distance * AI2, 0);

            }

            RotateTowards(player.Center, 4);

            float rotation = MathHelper.ToRadians(AI3 + (minRot * Math.Sign(AI3)));
            LockVector1 = LockVector1.RotatedBy(rotation * rotModifier);

            Vector2 target = player.Center + LockVector1;

            if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
            {
                AI3 = Main.rand.NextFloat(-1, 1);
            }

            if (AI3 == 0)
            {
                if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                {
                    NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                    NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                }
                else
                {
                    NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                }
            }
            else
            {
                NPC.velocity = (target - NPC.Center) / 10;
            }
            

            if (AI3 != 0 && Timer < 300)
            {
                if (Timer % 20 < 5)
                {
                    Anim = 1;
                }
                else
                {
                    Anim = 0;
                }
                if (Timer % 20 == 0)
                {
                    
                    SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = 10f;
                        int side = (Timer % 40 == 0) ? 1 : -1;
                        Vector2 v = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * side) * 1f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v * 20), v * speed, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI);
                        if (side == 1 && FargoSoulsWorld.EternityMode)
                        {
                            Vector2 v2 = NPC.rotation.ToRotationVector2() * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v2 * 20), v2 * 0.6f, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                        }
                    }
                }
                //fire rockets
            }

            if (Timer > 300)
            {
                Anim = 0;
            }
            if (Timer > 350)
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2SpinDash()
        {
            const int radius = 600;
            const float startRot = MathHelper.PiOver2 * 0.7f;
            float rotModifier = 3.5f;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);

            }
            if (AI3 == 0) //startup, reach starting point
            {

                LockVector1 = new Vector2(0, radius).RotatedBy(startRot * -AI2);
                Vector2 target = player.Center + LockVector1;
                NPC.rotation = NPC.DirectionTo(target).ToRotation();

                //velocity on rotation = radius * rotation per tick
                float rotVelocity = radius * MathHelper.ToRadians(3); //maximum value since random rotation velocity is not determined yet (could be done better by determining random before but would have to rework structure and i'm too lazy)
                if (NPC.velocity.Length() < rotVelocity + player.velocity.Length())
                {
                    NPC.velocity += 0.5f * NPC.rotation.ToRotationVector2();
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.velocity.Length() * NPC.rotation.ToRotationVector2();
                if (NPC.Distance(target) < 25)
                {
                    AI3 = Main.rand.NextFloat(0.2f, 1); //reached point, start attack, decide random rotation during charge windup
                }
            }
            if (AI3 > 0) //rotate a random bit below, and then dash
            {
                const int rotStart = 36;
                const int dashStart = 45;
                const int DashSpeed = 27;

                AI2 += Math.Sign(AI2); //internal timer, efficient variable use (?)
                
                Vector2 target = player.Center + LockVector1;
                NPC.velocity = (target - NPC.Center) / 10;


                if (Math.Abs(AI2) > rotStart)
                {
                    rotModifier = 0.1f; //rotate very slowly when preparing for dash
                    RotateTowards(player.Center, 20);
                    if (Math.Abs(AI2) > dashStart && Math.Abs(NPC.rotation - NPC.DirectionTo(player.Center).ToRotation()) < MathHelper.Pi / 32)
                    {
                        SoundEngine.PlaySound(BaronRoar, NPC.Center);
                        NPC.velocity = NPC.rotation.ToRotationVector2() * DashSpeed;
                        AI3 = -1;
                        AI2 = Math.Sign(AI2);
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.ToRotation();
                }

                float rotation = MathHelper.ToRadians(AI3 * Math.Sign(AI2) * rotModifier);
                LockVector1 = LockVector1.RotatedBy(rotation);

            }
            const int end = -110;
            const int bulletStart = -36;
            if (AI3 < 0) //spin after dash
            {
                if (AI3 == -1)
                {
                    AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                }
                AI3--;


                if (AI3 >= end)
                {
                    if (Math.Abs(AI2) < 60)
                    {
                        AI2 += Math.Sign(AI2) * 2;
                    }
                    NPC.velocity *= 0.995f;
                    NPC.velocity.Y += 0.2f;
                    NPC.rotation += -AI2 * MathHelper.Pi / 16 / 60;
                }
                else
                {
                    if (Math.Abs(AI2) > 4)
                    {
                        AI2 -= Math.Sign(AI2) * 2;
                    }

                    NPC.velocity *= 0.955f;
                    NPC.rotation += -AI2 * MathHelper.Pi / 16 / 60;
                }
                if (AI3 < bulletStart)
                {
                    Anim = 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = Main.rand.NextFloat(8, 13);
                        Vector2 v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + v, v * speed, ModContent.ProjectileType<BaronScrap>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }
                }
            }
            if (AI3 < end - 20)
            {
                Anim = 0;
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2MineFlurry()
        {
            const int distance = 500;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                LockVector1 = new Vector2(distance * AI2, 0);

            }
            
            

            LockVector1 = new Vector2(distance * AI2, 0);

            Vector2 target = player.Center + LockVector1;

            if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
            {
                AI3 = 1;
            }

            if (AI3 == 0)
            {
                Timer--; //timer doesnt go up until you reach start position (i should probably do this on the other attacks too for the startup)
                RotateTowards(player.Center, 4);
                if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                {
                    NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                    NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                }
                else
                {
                    NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                }
            }
            else
            {
                NPC.velocity = (target - NPC.Center) / 10;
            }

            const int cd = 30;
            bool time = (Timer >= 60 && Timer <= 60 + (cd*5)) || (Timer >= 260 && Timer <= 260 + (cd*5));
            if (time)
            {
                Anim = 1;
                if (Timer % cd == 0) //choose position and spawn telegraph for 20 ticks
                {
                    LockVector2 = new Vector2(0, Main.rand.NextFloat(-250, 250));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 4, NPC.whoAmI);
                }
                RotateTowards(player.Center + LockVector2, 5);
                if (Timer % cd == cd-1) //shoot
                {
                    SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = Main.rand.Next(10, 38);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed * NPC.rotation.ToRotationVector2(), ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                    }
                }
            }
            else
            {
                Anim = 0;
            }

            if (Timer > 420)
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        #endregion
        #endregion
        #region Help Methods
        void StateReset()
        {
            NPC.TargetClosest(false);
            if ((State == (int)StateEnum.Swim) || Phase == 2)
                RandomizeState();
            else
                State = (int)StateEnum.Swim;

            if (NPC.life < NPC.lifeMax / 2 && Phase == 1)
            {
                State = (float)StateEnum.Phase2Transition;
            }
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
        }
        void RandomizeState() //it's done this way so it cycles between attacks in a random order: for increased variety
        {
            int index;
            if (availablestates.Count < 1)
            {
                availablestates.Clear();
                if (Phase == 1)
                {
                    availablestates.AddRange(P1Attacks);
                }
                else if (Phase == 2)
                {
                    availablestates.AddRange(P2Attacks);
                }
                availablestates.Remove((int)State); //avoid possible repeat after refilling list
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                index = Main.rand.Next(availablestates.Count);
                State = availablestates[index];
                availablestates.RemoveAt(index);
            }
            NPC.netUpdate = true;
        }
        bool AliveCheck(Player p, bool forceDespawn = false)
        {
            if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
            {
                NPC.TargetClosest();
                p = Main.player[NPC.target];
                if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
                {
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    NPC.velocity.Y += 1f;
                    if (NPC.timeLeft == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                        }
                    }
                    return false;
                }
            }
            if (NPC.timeLeft < 300)
                NPC.timeLeft = 300;

            return true;
        }
        void RotateTowards(Vector2 target, float speed)
        {
            Vector2 PV = NPC.DirectionTo(target);
            Vector2 LV = NPC.rotation.ToRotationVector2();
            float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
            //change rotation towards target
            NPC.rotation = NPC.rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
        #endregion
    }
}
*/