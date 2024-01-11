using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TitaniumCDBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Titanium Shield Cooldown");
            // Description.SetDefault("You are charging up Titanium Shield");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetEffectFields<TitaniumFields>().TitaniumCD = true;
        }
    }
}
