using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon
{
    public class PatreonGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.IsWeapon() && player.GetModPlayer<PatreonPlayer>().CompOrb
                && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                if (!player.CheckMana(10, true, false))
                    return false;

                player.GetModPlayer<PatreonPlayer>().CompOrbDrainCooldown = item.useTime + item.reuseDelay + 30;
            }

            return true;
        }
    }
}