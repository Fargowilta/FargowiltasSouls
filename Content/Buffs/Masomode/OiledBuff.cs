using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class OiledBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Oiled");
            // Description.SetDefault("Taking more damage from being on fire");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "浸油");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "着火时将受到更多伤害");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Oiled = true;
        }
    }
}