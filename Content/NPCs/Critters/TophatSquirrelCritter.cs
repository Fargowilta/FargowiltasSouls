using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Items.Placables;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.Critters
{
    public class TophatSquirrelCritter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Top Hat Squirrel");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TownCritter[NPC.type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "高顶礼帽松鼠");
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
            NPC.width = 50;
            NPC.height = 32;
            NPC.damage = 0;
            NPC.chaseable = false;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            Main.npcCatchable[NPC.type] = true;
            NPC.catchItem = (short)ModContent.ItemType<TopHatSquirrelCaught>();
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0f;
            NPC.knockBackResist = .25f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TophatSquirrelBanner>();

            AnimationType = NPCID.Squirrel;
            NPC.aiStyle = 7;
            AIType = NPCID.Squirrel;

            NPC.friendly = true;

            NPC.rarity = 1; //appears on lifeform analyzer
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);
        }
    }
}
