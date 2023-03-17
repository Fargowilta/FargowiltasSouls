//JAVYZ TODO: CURSED COFFIN BOSS
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

namespace FargowiltasSouls.NPCs.Challengers
{
    [AutoloadBossHead]
    public class CursedCoffin : ModNPC
    {
        //TODO: re-enable boss checklist compat, localizationhelper addSpawnInfo
        public override bool IsLoadingEnabled(Mod mod) => false; //prevent appearing

        #region Variables
        private enum StateEnum
        {
            Opening
        }

        private enum P1Attacks
        {

        }
        private bool Attacking = true;
        private bool Flying = false;
        private bool ExtraTrail = false;

        private int StateCount = Enum.GetValues(typeof(StateEnum)).Length;
        private int Frame = 0;

        private List<int> availablestates = new List<int>(0);

        private Vector2 LockVector1 = Vector2.Zero;

        //NPC.ai[] overrides
        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float Random => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];

        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Coffin");
            Main.npcFrameCount[NPC.type] = 4;
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
            NPC.lifeMax = 4000;
            NPC.defense = 0;
            NPC.damage = 55;
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
            return true;
        }
        public override void HitEffect(int hitDirection, double HitDamage)
        {
            if (NPC.life <= 0)
            {
                //

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
            int currentFrame = NPC.frame.Y / (bodytexture.Height / Main.npcFrameCount[NPC.type]);

            for (int i = 0; i < (ExtraTrail ? NPCID.Sets.TrailCacheLength[NPC.type] : NPCID.Sets.TrailCacheLength[NPC.type] / 4); i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                int oldFrame = Frame;
                Rectangle oldRectangle = new Rectangle(0, oldFrame * bodytexture.Height / Main.npcFrameCount[NPC.type], bodytexture.Width, bodytexture.Height / Main.npcFrameCount[NPC.type]);
                DrawData oldGlow = new DrawData(bodytexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldRectangle), drawColor * (0.5f / i), NPC.rotation, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }

            spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: drawColor, rotation: NPC.rotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Y = frameHeight * Frame;
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CursedCoffin], -1);
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
        #region AI
        public override void AI()
        {
            //Defaults
            Player player = Main.player[NPC.target];
            NPC.defense = NPC.defDefense;
            NPC.rotation = 0;

            //Targeting
            if (!player.active || player.dead || player.ghost || NPC.Distance(player.Center) > 2400)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || player.ghost || NPC.Distance(player.Center) > 2400)
                {
                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;
                    NPC.velocity.Y -= 0.4f;
                    return;
                }
            }
            NPC.timeLeft = 60;
            if (State == 0 && Timer == 0) //opening
            {
                NPC.position = player.Center + new Vector2(0, -700) - NPC.Size / 2;
                LockVector1 = player.Center;
                NPC.velocity = new Vector2(0, 0.25f);
                //NPC.velocity = new Vector2(0, 10);
            }
            //Normal looping attack AI
            if (Flying) //Flying AI
            {
                FlyingState();
            }

            if (Attacking) //Phases and random attack choosing
            {
                switch (State) //Attack Choices
                {
                    case (float)StateEnum.Opening:
                        Opening();
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
        public void FlyingState()
        {

        }
        public void Opening()
        {
            //TODO: add slam when it hits ground (little shockwave, dust)
            Flying = false;
            ExtraTrail = true;
            NPC.velocity.Y *= 1.04f;
            if (NPC.Center.Y >= LockVector1.Y)
            {
                NPC.noTileCollide = false;
                if (NPC.velocity.Y <= 1) //when you hit tile
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    //dust explosion
                    ExtraTrail = false;
                    StateReset();
                }
            }
            if (NPC.Center.Y >= LockVector1.Y + 500) //only go so far
            {
                NPC.velocity = Vector2.Zero;
            }
            
        }
        #endregion
        #region Help Methods
        public void StateReset()
        {
            NPC.TargetClosest(false);
            RandomizeState();
            Timer = 0;
            Random = 0;
            AI3 = 0;
        }
        public void RandomizeState() //it's done this way so it cycles between attacks in a random order: for increased variety
        {
            int index;
            if (availablestates.Count < 1)
            {
                availablestates.Clear();
                for (int j = 0; j < StateCount; j++)
                {
                    availablestates.Add(j);
                }
                availablestates.Remove((int)State);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                index = Main.rand.Next(availablestates.Count);
                State = availablestates[index];
                availablestates.RemoveAt(index);
            }
            NPC.netUpdate = true;
        }
        #endregion
    }
}
*/