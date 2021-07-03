using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Oiled : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Oiled");
            Description.SetDefault("Taking more damage from being on fire");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "涂油");
            Description.AddTranslation(GameCulture.Chinese, "着火时将受到更多伤害");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Oiled = true;
        }
    }
}
