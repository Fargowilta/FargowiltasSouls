using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Berserked : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Berserked");
            //Description.SetDefault("10% increased damage and speed but you cannot control yourself");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "狂暴");
            //Description.AddTranslation(GameCulture.Chinese, "你控几不住你记几");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //causes player to constantly use weapon
            //seemed to have strange interactions with stunning debuffs like frozen or stoned...
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoPlayer>().Berserked = true;
            player.moveSpeed += 0.1f;
            player.controlUseItem = true;
            player.releaseUseItem = true;
            //player.HeldItem.autoReuse = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 2;
        }
    }
}
