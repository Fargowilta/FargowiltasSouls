using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.DeviBoss;

namespace FargowiltasSouls.NPCs.Champions
{
    public class LesserSquirrel : ModNPC
    {
        public override string Texture => "FargowiltasSouls/NPCs/Critters/TophatSquirrelCritter";

        public int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lesser Squirrel");
            DisplayName.AddTranslation(GameCulture.Chinese, "小松鼠");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 32;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 1800;
            //Main.npcCatchable[npc.type] = true;
            //npc.catchItem = (short)mod.ItemType("TophatSquirrel");
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 0f;
            npc.knockBackResist = .25f;
            //banner = npc.type;
            //bannerItem = mod.ItemType("TophatSquirrelBanner");

            animationType = NPCID.Squirrel;
            npc.aiStyle = 7;
            aiType = NPCID.Squirrel;

            //NPCID.Sets.TownCritter[npc.type] = true;

            //npc.closeDoor;

            npc.dontTakeDamage = true;
        }

        public override void AI()
        {
            if (npc.velocity.Y == 0)
                npc.dontTakeDamage = false;

            if (++counter > 600)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
            }
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Player.FindClosest(npc.Center, 0, 0);
                int n = NPC.FindFirstNPC(ModContent.NPCType<TimberChampion>());
                if (p != -1 && n != -1)
                {
                    Projectile.NewProjectile(npc.Center, 4f * npc.DirectionTo(Main.player[p].Center),
                        ModContent.ProjectileType<DeviLostSoul>(), Main.npc[n].damage / 4, 0, Main.myPlayer);
                }
            }
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f);
        }
    }
}
