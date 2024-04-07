using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class LeadPoisonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lead Poison");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铅中毒");
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().LeadPoison = true;
            if (npc.buffTime[buffIndex] == 2) //note: this totally also makes the npc reapply lead to themselves so its basically permanent debuff
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC spread = Main.npc[i];

                    if (i != npc.whoAmI && spread != null && spread.active && !spread.townNPC && !spread.friendly && spread.lifeMax > 5 && Vector2.Distance(npc.Center, spread.Center) < 50)
                    {
                        spread.AddBuff(ModContent.BuffType<LeadPoisonBuff>(), 30);
                    }
                }
            }
        }
    }
}