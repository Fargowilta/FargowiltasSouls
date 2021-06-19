using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class InfestedEX : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Infested EX");
            Description.SetDefault("This can only get worse");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "感染EX");
            Description.AddTranslation(GameCulture.Chinese, "这只会变得更糟");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoPlayer p = player.GetModPlayer<FargoPlayer>();

            player.ClearBuff(ModContent.BuffType<Infested>());

            p.MaxInfestTime = 2;
            p.FirstInfection = false;
            p.Infested = true;
        }
    }
}
