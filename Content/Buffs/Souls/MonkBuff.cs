using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class MonkBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meditation");
            // Description.SetDefault("Your Monk Dash is ready");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (!(player.whoAmI == Main.myPlayer && !player.HasEffect<MonkDashEffect>()))
                player.buffTime[buffIndex] = 2;
            if (player.mount.Active)
            {
                return;
            }
            player.FargoSouls().HasDash = true;
            player.FargoSouls().FargoDash = DashManager.DashType.Monk;
        }
    }
}