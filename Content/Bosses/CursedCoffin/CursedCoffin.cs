using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    [AutoloadBossHead]
    public partial class CursedCoffin : ModNPC
    {
        public const bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;

        #region Variables

        private bool Attacking = true;
        private bool ExtraTrail = false;

        public bool PhaseTwo;

        public int MashTimer = 15;

        private int Frame = 0;

        private Vector2 LockVector1 = Vector2.Zero;

        private int LastAttackChoice { get; set; }

        public float Timer
        {
            get => StateMachine.CurrentState.Time;
            set => StateMachine.CurrentState.Time = (int)value;
        }

        //NPC.ai[] overrides
        /// <summary>
        /// Setting this to a number except 0 immediately forces the SpiritGrabPunish state.
        /// This happens when the Spirit grabs a player.
        /// </summary>
        public ref float ForceGrabPunish => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];

        public Vector2 MaskCenter() => NPC.Center - Vector2.UnitY * NPC.height * NPC.scale / 4;

        public static readonly Color GlowColor = new(224, 196, 252, 0);

        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 18; //decrease later if not needed
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPC.AddDebuffImmunities(new List<int>
            {
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.Suffocation,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<ClippedWingsBuff>(),
                ModContent.BuffType<TimeFrozenBuff>()
            });
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert,
                //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.any,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });

        }
        public const int BaseHP = 2222;
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = BaseHP;
            NPC.defense = 10;
            NPC.damage = 35;
            NPC.knockBackResist = 0f;
            NPC.width = 90;
            NPC.height = 150;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath6;

            Music = MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.value = Item.buyPrice(0, 2);

        }
        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            if (NPC.rotation != 0)
            {
                int centerX = npcHitbox.X + (npcHitbox.Width / 2);
                int centerY = npcHitbox.Y + (npcHitbox.Height / 2);

                float angle = NPC.rotation % MathF.Tau;
                float incline = MathF.Abs(MathF.Sin(angle));
                npcHitbox.Height = (int)(MathHelper.Lerp(NPC.height, NPC.width, incline) * NPC.scale);
                npcHitbox.Width = (int)(MathHelper.Lerp(NPC.width, NPC.height, incline) * NPC.scale);

                npcHitbox.X = (int)(centerX - (npcHitbox.Width / 2));
                npcHitbox.Y = (int)(centerY - (npcHitbox.Height / 2));
            }
			return true;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!PhaseTwo && projectile.Colliding(projectile.Hitbox, TopHitbox()) && Frame <= 1)
            {
                NPC.HitSound = SoundID.NPCHit54;
                modifiers.FinalDamage *= 1.3f;
            }
            else
            {
                NPC.HitSound = SoundID.NPCHit4;
            }
            base.ModifyHitByProjectile(projectile, ref modifiers);
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (!PhaseTwo && item.Hitbox.Intersects(TopHitbox()) && Frame <= 1)
            {
                NPC.HitSound = SoundID.NPCHit54;
                modifiers.FinalDamage *= 1.3f;
            }
            else
            {
                NPC.HitSound = SoundID.NPCHit4;
            }
            base.ModifyHitByItem(player, item, ref modifiers);
        }
        public Rectangle TopHitbox()
        {
            return new((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height / 3);
        }

        /*
        public Rectangle MaskHitbox()
        {
            Vector2 maskCenter = MaskCenter();
            int maskRadius = 24;
            return new((int)(maskCenter.X - maskRadius * NPC.scale), (int)(maskCenter.Y - maskRadius * NPC.scale), maskRadius * 2, maskRadius * 2);
        }
        */
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
            writer.Write(PhaseTwo);
            writer.Write7BitEncodedInt(LastAttackChoice);

            // 1. Write the number of states on the stack.
            writer.Write(StateMachine.StateStack.Count);

            // 2. Write the state IDs as ints to the stack in the order they are on the stack. Also write the timers.
            var stackArray = StateMachine.StateStack.ToArray();
            for (int i = 0; i < StateMachine.StateStack.Count; i++)
                writer.Write((int)stackArray[i].Identifier);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            PhaseTwo = reader.ReadBoolean();
            LastAttackChoice = reader.Read7BitEncodedInt();
            Timer = reader.ReadSingle();

            // 1. Read the number of states that should be added to the stack and were written.
            int stackCount = reader.ReadInt32();
            // Clear the stack in preperation for pushing the written states to it.
            StateMachine.StateStack.Clear();
            // 2. Read the state IDs and push them to the stack.
            for (int i = 0; i < stackCount; i++)
                StateMachine.StateStack.Push(StateMachine.StateRegistry[(BehaviorStates)reader.ReadInt32()]);
        }
        #endregion

        #region Overrides
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < (ExtraTrail ? NPCID.Sets.TrailCacheLength[NPC.type] : NPCID.Sets.TrailCacheLength[NPC.type] / 4); i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                int oldFrame = Frame;
                Rectangle oldRectangle = new(0, oldFrame * bodytexture.Height / Main.npcFrameCount[NPC.type], bodytexture.Width, bodytexture.Height / Main.npcFrameCount[NPC.type]);
                DrawData oldGlow = new(bodytexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldRectangle), drawColor * (0.5f / i), NPC.rotation, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, spriteEffects, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }

            spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: drawColor, rotation: NPC.rotation, scale: NPC.scale, effects: spriteEffects, layerDepth: 0f);

            if (!PhaseTwo)
            {
                float shakeFactor = 1;
                if (StateMachine.CurrentState != null && StateMachine.CurrentState.Identifier == BehaviorStates.PhaseTransition)
                    shakeFactor = 3 + 5 * (Timer / 60);
                Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_MaskGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Color glowColor = GlowColor;
                int glowTimer = (int)(Main.GlobalTimeWrappedHourly * 60) % 60;
                DrawData oldGlow = new(glowTexture, drawPos + Main.rand.NextVector2Circular(shakeFactor, shakeFactor), NPC.frame, glowColor * (0.75f + 0.25f * MathF.Sin(MathF.Tau * glowTimer / 60f)), NPC.rotation, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, spriteEffects, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Purple).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Y = frameHeight * Frame;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.CursedCoffin], -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //TODO: Add loot
            //npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CursedCoffinBag>()));
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedCoffinTrophy>(), 10));

            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CursedCoffinRelic>()));

            //LeadingConditionRule rule = new LeadingConditionRule(new Conditions.NotExpert());
            //rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<EnchantedLifeblade>(), ModContent.ItemType<Lightslinger>(), ModContent.ItemType<CrystallineCongregation>(), ModContent.ItemType<KamikazePixieStaff>()));
            //rule.OnSuccess(ItemDropRule.Common(ItemID.HallowedFishingCrateHard, 1, 1, 5)); //hallowed crate
            //rule.OnSuccess(ItemDropRule.Common(ItemID.SoulofLight, 1, 1, 3));
            //rule.OnSuccess(ItemDropRule.Common(ItemID.PixieDust, 1, 15, 25));

            //npcLoot.Add(rule);
        }
        #endregion
    }
}