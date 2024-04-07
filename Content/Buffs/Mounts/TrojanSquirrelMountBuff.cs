using FargowiltasSouls.Content.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Mounts
{
    public class TrojanSquirrelMountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Trojan Squirrel");
            // Description.SetDefault("You are riding a squirrel mech");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<TrojanSquirrelMount>(), player);
            player.buffTime[buffIndex] = 10;

            player.FargoSouls().SquirrelMount = true;
        }
    }
}
