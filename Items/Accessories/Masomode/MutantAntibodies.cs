using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class MutantAntibodies : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Antibodies");
            Tooltip.SetDefault(@"Grants immunity to Wet, Feral Bite, Mutant Nibble, and Oceanic Maul
Grants immunity to most debuffs caused by entering water
Grants effects of Wet debuff while riding Cute Fishron
Increases damage by 20%
'Healthy drug recommended by 0 out of 10 doctors'");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变抗体");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫潮湿,野性咬噬和突变啃啄和海洋重击
免疫大部分由水造成的Debuff
骑乘猪鲨坐骑时获得潮湿状态
增加20%伤害
'推荐健康药物指数: 0/10'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[mod.BuffType("MutantNibble")] = true;
            player.buffImmune[mod.BuffType("OceanicMaul")] = true;
            player.GetModPlayer<FargoPlayer>().MutantAntibodies = true;
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.2f);
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;
        }
    }
}
