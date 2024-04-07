
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Projectiles;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Content.Buffs;
using System.Linq;
using Microsoft.CodeAnalysis;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PirateInvasion;

namespace FargowiltasSouls.Content.Bosses.Magmaw
{
    [AutoloadBossHead]
    public partial class Magmaw : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        #region Variables

        //Visuals

        /// <summary>
        /// Amount of trail frames to draw
        /// </summary>
        public int Trail = 0;

        /// <summary>
        /// Current animation frame
        /// </summary>
        public int Frame = 0;
        /// <summary>
        /// Current animation
        /// </summary>
        public int Anim = 0;

        //Appendages
        /// <summary>
        /// Jaw position offset relative to NPC center
        /// </summary>
        public Vector2 JawOffset = Vector2.Zero;
        /// <summary>
        /// Jaw position center. NPC.Center + JawOffset
        /// </summary>
        public Vector2 JawCenter => NPC.Center + JawOffset;
        /// <summary>
        /// Right hand side, corresponding to world X-axis direction
        /// </summary>
        private const int Right = 1;
        /// <summary>
        /// Left hand side, corresponding to world X-axis direction
        /// </summary>
        private const int Left = -1;

        // AI-related
        /// <summary>
        /// The player the boss is targeting
        /// </summary>
        Player player => Main.player[NPC.target];

        public ref float IdleTime => ref NPC.localAI[0];
        public ref float CurrentState => ref NPC.localAI[1];
        public ref float LastState => ref NPC.localAI[2];
        //public ref float free => ref NPC.localAI[3];
        public ref float Timer => ref NPC.ai[0];
        public ref float AI1 => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];

        /// <summary>
        /// Contact damage
        /// </summary>
        public bool HitPlayer = true;
        public int Phase = 1;
        /// <summary>
        /// Amount of attacks the boss has executed since idling
        /// </summary>
        public int ChainDepth = 0;
        /// <summary>
        /// Max attacks the boss can do before idling again
        /// </summary>
        public int MaxChainDepth = 5;
        /// <summary>
        /// Whether the boss should reposition to the idle spot above the player while idling
        /// </summary>
        public bool IdleReposition = true;

        public Vector2 LockVector1 = Vector2.Zero;
        public Vector2 LockVector2 = Vector2.Zero;
        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPC.AddDebuffImmunities(new List<int>
            {
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.Suffocation,
                BuffID.OnFire,
                BuffID.OnFire3,
                ModContent.BuffType<HellFireBuff>(),
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<ClippedWingsBuff>()
            });
            /*
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = MathHelper.Pi,
                Position = Vector2.UnitX * 60
            });
            */
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 70000;
            NPC.defense = 65;
            NPC.damage = 72;
            NPC.knockBackResist = 0f;
            NPC.width = 125;
            NPC.height = 92;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath44;

            Music = MusicID.Boss2; //ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod) ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Baron") : MusicID.Boss2;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.value = Item.buyPrice(0, 15);

        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return NPC.GetBestiaryEntryColor();
            }
            return base.GetAlpha(drawColor);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write7BitEncodedInt(Phase);
            writer.Write7BitEncodedInt(ChainDepth);
            writer.Write7BitEncodedInt(MaxChainDepth);
            writer.Write(IdleReposition);
            writer.WriteVector2(LockVector1);
            writer.WriteVector2(LockVector2);

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            Phase = reader.Read7BitEncodedInt();
            ChainDepth = reader.Read7BitEncodedInt();
            MaxChainDepth = reader.Read7BitEncodedInt();
            IdleReposition = reader.ReadBoolean();
            LockVector1 = reader.ReadVector2();
            LockVector2 = reader.ReadVector2();
        }
        #endregion
        #region Overrides
        #region Hitbox
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (HitPlayer)
            {
                Vector2 boxPos = target.position;
                Vector2 boxDim = target.Size;
                return Collides(boxPos, boxDim);
            }
            return false;
        }
        public override bool CanHitNPC(NPC target)
        {
            if (HitPlayer)
            {
                Vector2 boxPos = target.position;
                Vector2 boxDim = target.Size;
                return Collides(boxPos, boxDim);
            }
            return false;
        }
        public bool Collides(Vector2 boxPos, Vector2 boxDim)
        {
            //circular hitbox-inator
            Vector2 ellipseDim = NPC.Size;
            Vector2 ellipseCenter = NPC.position + 0.5f * new Vector2(NPC.width, NPC.height);

            float x = 0f; //ellipse center
            float y = 0f; //ellipse center
            if (boxPos.X > ellipseCenter.X)
            {
                x = boxPos.X - ellipseCenter.X; //left corner
            }
            else if (boxPos.X + boxDim.X < ellipseCenter.X)
            {
                x = boxPos.X + boxDim.X - ellipseCenter.X; //right corner
            }
            if (boxPos.Y > ellipseCenter.Y)
            {
                y = boxPos.Y - ellipseCenter.Y; //top corner
            }
            else if (boxPos.Y + boxDim.Y < ellipseCenter.Y)
            {
                y = boxPos.Y + boxDim.Y - ellipseCenter.Y; //bottom corner
            }
            float a = ellipseDim.X / 2f;
            float b = ellipseDim.Y / 2f;

            return x * x / (a * a) + y * y / (b * b) < 1; //point collision detection
        }
        #endregion

        public override bool? CanFallThroughPlatforms() => true;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire3, 60 * 10);
            if (!WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.Oiled, 60 * 5);
                return;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type] && !FargoSoulsUtil.IsSummonDamage(projectile))
                modifiers.FinalDamage *= 0.8f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            float rot = NPC.rotation;
            SpriteEffects flip = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < Math.Min(Trail, NPCID.Sets.TrailCacheLength[NPC.type]); i++) //math.min to safeguard against uncached trail
            {
                float oldrot = NPC.oldRot[i] + (NPC.direction == 1 ? 0 : MathHelper.Pi);
                Vector2 value4 = NPC.oldPos[i];
                int oldFrame = Frame;
                Rectangle oldRectangle = new Rectangle(0, oldFrame * bodytexture.Height / Main.npcFrameCount[NPC.type], bodytexture.Width, bodytexture.Height / Main.npcFrameCount[NPC.type]);
                DrawData oldGlow = new DrawData(bodytexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldRectangle), NPC.GetAlpha(drawColor) * (0.5f / i), oldrot, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, flip, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }

            spriteBatch.Draw(bodytexture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), rot, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, flip, 0f);
            return false;
        }

        float AnimationSpeed = 1f;
        public override void FindFrame(int frameHeight)
        {
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.Magmaw], -1);
            NPC.SetEventFlagCleared(ref NPC.downedGolemBoss, 6);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            //TODO: gore
            /*
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"BaronGore{i}").Type, NPC.scale);
                }
            }
            */
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            //position += (JawCenter - NPC.Center) / 2;
            position = JawCenter + Vector2.UnitY * 35 * NPC.scale;
            return true;
            //return base.DrawHealthBar(hbPosition, ref scale, ref position);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //TODO: Add loot
            /*
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BanishedBaronBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BaronTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BaronRelic>()));

            LeadingConditionRule rule = new LeadingConditionRule(new Conditions.NotExpert());
            rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<TheBaronsTusk>(), ModContent.ItemType<RoseTintedVisor>(), ModContent.ItemType<NavalRustrifle>(), ModContent.ItemType<DecrepitAirstrikeRemote>()));
            rule.OnSuccess(ItemDropRule.Common(5003, 1, 1, 5)); //seaside crate
            rule.OnSuccess(ItemDropRule.OneFromOptions(3, ItemID.Sextant, ItemID.WeatherRadio, ItemID.FishermansGuide));
            rule.OnSuccess(ItemDropRule.Common(ItemID.FishingBobber, 4, 1, 1));
            rule.OnSuccess(ItemDropRule.Common(ItemID.FishingPotion, 3, 2, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.SonarPotion, 2, 2, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.CratePotion, 5, 2, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.GoldenBugNet, 50, 1, 1));
            rule.OnSuccess(ItemDropRule.Common(ItemID.FishHook, 50, 1, 1));
            rule.OnSuccess(ItemDropRule.Common(ItemID.GoldenFishingRod, 150, 1, 1));
            
            npcLoot.Add(rule);
            */
        }

        #endregion

        #region Help Methods
        bool AliveCheck(Player p, bool forceDespawn = false)
        {
            if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
            {
                NPC.TargetClosest();
                p = Main.player[NPC.target];
                if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
                {
                    NPC.noTileCollide = true;
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    NPC.velocity.Y += 1f;
                    if (NPC.timeLeft == 1)
                    {
                        if (FargoSoulsUtil.HostCheck)
                        {
                            FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                        }
                    }
                    return false;
                }
            }
            if (NPC.timeLeft < 600)
                NPC.timeLeft = 600;

            return true;
        }
        void RotateTowards(Vector2 target, float speed)
        {
            Vector2 LV = NPC.rotation.ToRotationVector2();
            Vector2 PV = NPC.SafeDirectionTo(target);
            float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
            //change rotation towards target
            NPC.rotation = NPC.rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
        #endregion
    }
}