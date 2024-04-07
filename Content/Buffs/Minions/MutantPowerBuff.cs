using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class MutantPowerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Power");
            // Description.SetDefault("The power of Mutant is with you");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变之力");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "突变之力与你同在");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                //Effects moved to AccessoryEffects
            }
        }
    }
}