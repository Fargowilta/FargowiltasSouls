using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DemonKing
{
    public class DukeFishronBuff : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Duke Fishron");
            //Description.SetDefault("Duke Fishron will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DukeFishronMinion>()] > 0) modPlayer.DukeFishron = true;
            if (!modPlayer.DukeFishron)
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