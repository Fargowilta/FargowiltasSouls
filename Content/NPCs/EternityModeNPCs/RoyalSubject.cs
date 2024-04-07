using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
    public class RoyalSubject : ModNPC
    {
        //public override string Texture => "FargowiltasSouls/Assets/ExtraTextures/Resprites/NPC_222";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Royal Subject");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "皇家工蜂");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type] = NPCID.Sets.SpecificDebuffImmunity[NPCID.QueenBee];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                      ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCID.QueenBee],
                      quickUnlock: true
                  );
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasSouls.Bestiary.RoyalSubject")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.aiStyle = 43;
            AIType = NPCID.QueenBee;
            NPC.damage = 25;
            NPC.defense = 8;
            NPC.lifeMax = 600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.npcSlots = 7f;
            NPC.scale = 1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7 * System.Math.Max(1.0, balance / 2));
            NPC.damage = (int)(NPC.damage * 0.9);
        }

        public override void AI()
        {
            if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee)
                && !NPC.AnyNPCs(NPCID.QueenBee))
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                return;
            }

            //tries to stinger, force into dash
            if (NPC.ai[0] != 0)
            {
                NPC.ai[0] = 0f;
                NPC.netUpdate = true;
            }

            if (NPC.ai[1] != 2f && NPC.ai[1] != 3f)
            {
                NPC.ai[1] = 2f;
                NPC.netUpdate = true;
            }

            NPC.position -= NPC.velocity / 3;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 180));
            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
            target.AddBuff(ModContent.BuffType<SwarmingBuff>(), 600);
        }

        public override bool CheckDead()
        {
            NPC queenBee = FargoSoulsUtil.NPCExists(EModeGlobalNPC.beeBoss, NPCID.QueenBee);
            if (queenBee != null && FargoSoulsUtil.HostCheck
                && queenBee.GetGlobalNPC<QueenBee>().BeeSwarmTimer < 600) //dont change qb ai during bee swarm attack
            {
                queenBee.ai[0] = 0f;
                queenBee.ai[1] = 4f; //trigger dashes, but skip the first one
                queenBee.ai[2] = -44f;
                queenBee.ai[3] = 0f;
                queenBee.netUpdate = true;
            }

            if (NPC.DeathSound != null)
                SoundEngine.PlaySound(NPC.DeathSound.Value, NPC.Center);

            NPC.active = false;

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 0.75f;
                }

                for (int i = 303; i <= 308; i++)
                    if (!Main.dedServ)
                        if (!Main.dedServ)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity / 2, i, NPC.scale);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.localAI[0] == 1)
            {
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
            else
            {
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y < 3 * frameHeight)
                    NPC.frame.Y = 3 * frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                    NPC.frame.Y = 3 * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Texture2D texture = recolor ? ModContent.Request<Texture2D>(Texture + "22").Value : Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = NPC.GetAlpha(drawColor);
            Main.EntitySpriteDraw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), rectangle, color, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
        //public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        //{
        //    if (!Terraria.GameContent.TextureAssets.Npc[NPCID.QueenBee].IsLoaded)
        //        return false;

        //    Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.QueenBee].Value;
        //    Rectangle rectangle = NPC.frame;
        //    Vector2 origin2 = rectangle.Size() / 2f;

        //    SpriteEffects effects = NPC.spriteDirection < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        //    Color color = NPC.GetAlpha(drawColor);
        //    Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, NPC.rotation, origin2, NPC.scale, effects, 0);
        //    return true;
        //}
    }
}