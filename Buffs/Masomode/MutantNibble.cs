using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class MutantNibble : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Nibble");
            Description.SetDefault("You cannot heal at all");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变啃啄");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "无法恢复生命");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //disables potions, moon bite effect, feral bite effect, disables lifesteal
            player.GetModPlayer<FargoSoulsPlayer>().MutantNibble = true;

            //player.potionDelay = player.buffTime[buffIndex];
            player.moonLeech = true;

            //feral bite stuff
            player.rabid = true;

            if (FargoSoulsWorld.MasochistModeReal && Main.rand.NextBool(1200))
            {
                switch (Main.rand.Next(10))
                {
                    case 0: player.AddBuff(ModContent.BuffType<Defenseless>(), Main.rand.Next(300)); break;
                    case 1: player.AddBuff(ModContent.BuffType<Lethargic>(), Main.rand.Next(240)); break;
                    case 2: player.AddBuff(ModContent.BuffType<Flipped>(), Main.rand.Next(120)); break;
                    case 3: player.AddBuff(ModContent.BuffType<Hexed>(), Main.rand.Next(120)); break;
                    case 4: player.AddBuff(ModContent.BuffType<MarkedforDeath>(), Main.rand.Next(120)); break;
                    case 5: player.AddBuff(ModContent.BuffType<Purified>(), Main.rand.Next(60)); break;
                    case 6: player.AddBuff(ModContent.BuffType<Rotting>(), Main.rand.Next(300)); break;
                    case 7: player.AddBuff(ModContent.BuffType<SqueakyToy>(), Main.rand.Next(120)); break;
                    case 8: player.AddBuff(ModContent.BuffType<Unstable>(), Main.rand.Next(90)); break;
                    case 9: player.AddBuff(ModContent.BuffType<Berserked>(), Main.rand.Next(180)); break;
                }
            }

            player.GetDamage(DamageClass.Generic) += 0.2f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().MutantNibble = true;
        }
    }
}