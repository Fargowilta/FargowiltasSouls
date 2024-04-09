using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Armor;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Systems
{
    public class SetBonusManager : ModSystem
    {
        public override void Load()
        {
            On_Player.KeyDoubleTap += new On_Player.hook_KeyDoubleTap(SetBonusKeyEffects);
        }
        public override void Unload()
        {
            On_Player.KeyDoubleTap -= new On_Player.hook_KeyDoubleTap(SetBonusKeyEffects);
        }
        public void SetBonusKeyEffects(On_Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            orig.Invoke(player, keyDir);
            if (keyDir == (Main.ReversedUpDownArmorSetBonuses ? 1 : 0))
            {
                GladiatorBanner.ActivateGladiatorBanner(player);
                PalmwoodEffect.ActivatePalmwoodSentry(player);
                EridanusHat.EridanusSetBonusKey(player);
                GaiaHelmet.GaiaSetBonusKey(player);
                NekomiHood.NekomiSetBonusKey(player);
                StyxCrown.StyxSetBonusKey(player);
                ForbiddenEffect.ActivateForbiddenStorm(player);
                VortexEnchant.ActivateVortex(player);
            }

        }
    }
}
