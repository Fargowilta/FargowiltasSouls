using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class MutantNibble : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Mutant Nibble");
            //Description.SetDefault("You cannot heal at all");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "突变啃啄");
            //Description.AddTranslation(GameCulture.Chinese, "无法恢复生命");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //disables potions, moon bite effect, feral bite effect, disables lifesteal
            player.GetModPlayer<FargoPlayer>().MutantNibble = true;

            //player.potionDelay = player.buffTime[buffIndex];
            player.moonLeech = true;

            //feral bite stuff
            player.rabid = true;
            /*if (Main.rand.Next(1200) == 0)
            {
                switch (Main.rand.Next(10))
                {
                    case 0: player.AddBuff(mod.BuffType("Defenseless"), Main.rand.Next(300)); break;
                    case 1: player.AddBuff(mod.BuffType("Lethargic"), Main.rand.Next(240)); break;
                    case 2: player.AddBuff(mod.BuffType("Flipped"), Main.rand.Next(120)); break;
                    case 3: player.AddBuff(mod.BuffType("Hexed"), Main.rand.Next(120)); break;
                    case 4: player.AddBuff(mod.BuffType("MarkedforDeath"), Main.rand.Next(120)); break;
                    case 5: player.AddBuff(mod.BuffType("Purified"), Main.rand.Next(60)); break;
                    case 6: player.AddBuff(mod.BuffType("Rotting"), Main.rand.Next(300)); break;
                    case 7: player.AddBuff(mod.BuffType("SqueakyToy"), Main.rand.Next(120)); break;
                    case 8: player.AddBuff(mod.BuffType("Unstable"), Main.rand.Next(90)); break;
                    case 9: player.AddBuff(mod.BuffType("Berserked"), Main.rand.Next(180)); break;
                }
            }*/

            player.meleeDamage = player.meleeDamage + 0.2f;
            player.magicDamage = player.magicDamage + 0.2f;
            player.rangedDamage = player.rangedDamage + 0.2f;
            player.minionDamage = player.minionDamage + 0.2f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().MutantNibble = true;
        }
    }
}