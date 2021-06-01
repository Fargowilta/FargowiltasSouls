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
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫潮湿、野性咬噬、突变啃啄和海洋重击减益
使你免疫大多数浸没在水中时获得的减益
骑着可爱猪龙鱼坐骑时使你获得潮湿减益的效果
增加20%伤害
'10位医生中有0位推荐的健康药物'");
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
