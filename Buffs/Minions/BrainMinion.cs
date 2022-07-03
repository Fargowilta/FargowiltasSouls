using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class BrainMinion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain of Cthulhu");
            Description.SetDefault("The mini Brain of Cthulhu will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之脑");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "迷你克苏鲁之脑将会为你而战");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.BrainMinion>()] > 0) modPlayer.BrainMinion = true;
            if (!modPlayer.BrainMinion)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}