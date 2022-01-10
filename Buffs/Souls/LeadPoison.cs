using Microsoft.Xna.Framework;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Souls
{
    public class LeadPoison : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Lead Poison");
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "铅中毒");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().LeadPoison = true;
            if (npc.buffTime[buffIndex] == 2) //note: this totally also makes the npc reapply lead to themselves so its basically permanent debuff
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC spread = Main.npc[i];

                    if (spread.active && !spread.townNPC && !spread.friendly && spread.lifeMax > 5 && !spread.HasBuff(mod.BuffType("LeadPoison")) && Vector2.Distance(npc.Center, spread.Center) < 50)
                    {
                        spread.AddBuff(mod.BuffType("LeadPoison"), 30);
                    }
                }
            }
        }
    }
}