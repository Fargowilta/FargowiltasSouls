using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class ReverseManaFlow : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Reverse Mana Flow");
            //Description.SetDefault("Your magic weapons cost life instead of mana");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
            //DisplayName.AddTranslation(GameCulture.Chinese, "反魔力流");
            //Description.AddTranslation(GameCulture.Chinese, "魔法武器消耗生命,而不是法力");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //mana cost also damages
            player.GetModPlayer<FargoPlayer>().ReverseManaFlow = true;
            player.magicDamage -= 1.5f;
            if (player.HeldItem.magic)
                player.GetModPlayer<FargoPlayer>().AttackSpeed -= 0.5f;
        }
    }
}