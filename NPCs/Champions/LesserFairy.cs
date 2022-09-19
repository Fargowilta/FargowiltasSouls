using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    public class LesserFairy : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_75";

        public int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lesser Fairy");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "小精灵");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Pixie];
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<LifeChampion>()],
                quickUnlock: true
            );
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 180;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;

            AnimationType = NPCID.Pixie;
            NPC.aiStyle = -1;

            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 87);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

            if (Main.rand.NextBool(40))
            {
                SoundEngine.PlaySound(SoundID.Pixie, NPC.Center);
            }

            NPC.direction = NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
            NPC.rotation = NPC.velocity.X * 0.1f;

            if (++counter > 60 && counter < 240)
            {
                if (!NPC.HasValidTarget)
                    NPC.TargetClosest();

                if (NPC.Distance(Main.player[NPC.target].Center) < 300)
                {
                    NPC.velocity = NPC.DirectionTo(Main.player[NPC.target].Center) * NPC.velocity.Length();
                }
            }
            else if (counter > 300)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Purified>(), 300);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 4)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 87, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }
        }
    }
}
