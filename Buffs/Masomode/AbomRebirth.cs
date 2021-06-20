using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class AbomRebirth : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abominable Rebirth");
            Description.SetDefault("You cannot heal at all and cannot die unless struck");
            DisplayName.AddTranslation(GameCulture.Chinese, "憎恶重生");
            Description.AddTranslation(GameCulture.Chinese, "你无法恢复生命值，并且只有受到直接攻击后才会死亡");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().MutantNibble = true;
            player.GetModPlayer<FargoPlayer>().AbomRebirth = true;
        }
    }
}
