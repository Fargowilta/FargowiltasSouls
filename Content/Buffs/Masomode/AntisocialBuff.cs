using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class AntisocialBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Antisocial");
            // Description.SetDefault("You have no friends and no summon damage");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "反社交");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你没有朋友");

        }

        public override void Update(Player player, ref int buffIndex)
        {
            //disables minions, disables pets
            player.FargoSouls().Asocial = true;

            player.GetDamage(DamageClass.Summon) *= 0.1f;
        }
    }
}