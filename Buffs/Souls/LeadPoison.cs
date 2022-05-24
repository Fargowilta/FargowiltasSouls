using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class LeadPoison : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Poison");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铅中毒");
        }

        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().LeadPoison = true;
            if (npc.buffTime[buffIndex] == 2) //note: this totally also makes the npc reapply lead to themselves so its basically permanent debuff
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC spread = Main.npc[i];

                    if (i != npc.whoAmI && spread.active && !spread.townNPC && !spread.friendly && spread.lifeMax > 5 && Vector2.Distance(npc.Center, spread.Center) < 50)
                    {
                        spread.AddBuff(ModContent.BuffType<LeadPoison>(), 30);
                    }
                }
            }
        }
    }
}