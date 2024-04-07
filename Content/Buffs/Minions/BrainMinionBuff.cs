using FargowiltasSouls.Content.Projectiles.Minions;
using Terraria;
using Terraria.ModLoader;


namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class BrainMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brain of Cthulhu");
            // Description.SetDefault("The mini Brain of Cthulhu will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之脑");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "迷你克苏鲁之脑将会为你而战");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrainMinion>()] > 0) modPlayer.BrainMinion = true;
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