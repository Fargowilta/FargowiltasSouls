
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class GrabbedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Grabbed");
            // Description.SetDefault("Mash movement keys to escape!");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "抓住你了！");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "狂点你的移动键来逃离这个！");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            fargoPlayer.Mash = true;

            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.RemoveAllGrapplingHooks();
            player.releaseHook = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
            //fargoPlayer.Stunned = true;
            fargoPlayer.NoUsingItems = 2;
        }
    }
}
