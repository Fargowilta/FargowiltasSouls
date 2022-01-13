using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class DetonatingBubble : ModNPC
    {
        public override string Texture => "Terraria/NPC_371";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Detonating Bubble");
            Main.npcFrameCount[npc.type] = 2;
            //DisplayName.AddTranslation(GameCulture.Chinese, "爆炸泡泡");
        }

        public override void SetDefaults()
        {
            npc.width = 36;
            npc.height = 36;
            npc.damage = 100;
            npc.lifeMax = 1;
            npc.HitSound = SoundID.NPCHit3;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.aiStyle = -1;
            npc.chaseable = false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.75);
            npc.lifeMax = 1;
        }

        public override void AI()
        {
            if (npc.buffTime[0] != 0)
            {
                npc.buffImmune[npc.buffType[0]] = true;
                npc.DelBuff(0);
            }

            if (npc.alpha > 50)
                npc.alpha -= 30;
            else
                npc.alpha = 50;

            npc.velocity *= 1.03f;

            npc.ai[0]++;
            if (npc.ai[0] >= 240f)
            {
                npc.life = 0;
                npc.checkDead();
                npc.active = false;
            }
        }

        public override bool CheckDead()
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().Needles = false;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 420);
            //target.AddBuff(mod.BuffType("SqueakyToy"), Main.rand.Next(60, 180));
            target.AddBuff(mod.BuffType("OceanicMaul"), 1800);
            target.GetModPlayer<FargoPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = Main.npcTexture[npc.type].Height / 2;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
    }
}