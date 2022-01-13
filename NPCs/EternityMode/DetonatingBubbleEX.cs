using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class DetonatingBubbleEX : ModNPC
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
            npc.lifeMax = 5000;//500;
            npc.HitSound = SoundID.NPCHit3;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.aiStyle = -1;
            npc.chaseable = false;
            npc.buffImmune[BuffID.Suffocation] = true;
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

            npc.velocity *= 1.04f;

            npc.ai[0]++;
            if (npc.ai[0] >= 120f)
            {
                npc.life = 0;
                npc.checkDead();
                npc.active = false;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            return true;
        }

        public override bool CheckDead()
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().Needles = false;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (target.hurtCooldowns[1] == 0)
            {
                target.AddBuff(BuffID.Wet, 420);
                target.AddBuff(mod.BuffType("Defenseless"), 600);
                target.AddBuff(mod.BuffType("OceanicMaul"), 1800);
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
            }
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