using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class NullificationCurse : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Nullification Curse");
            //Description.SetDefault("You cannot dodge and Moon Lord has cycling damage type immunities!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
            //DisplayName.AddTranslation(GameCulture.Chinese, "无效诅咒");
            //Description.AddTranslation(GameCulture.Chinese, "无法躲避,并且月球领主进行循环伤害免疫!");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().noDodge = true;
        }
    }
}