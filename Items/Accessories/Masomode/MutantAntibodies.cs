using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
Increases damage for your current weapon class by 20%, but decreases life regeneration
Submerging in water refreshes flight time and gives you improved speed and increased max flight time
'Healthy drug recommended by 0 out of 10 doctors'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变抗体");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'推荐健康药物指数: 0/10'
            // 免疫潮湿,野性咬噬和突变啃啄和海洋重击
            // 免疫大部分由水造成的Debuff
            // 骑乘猪鲨坐骑时获得潮湿状态
            // 增加20%伤害");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[ModContent.BuffType<MutantNibble>()] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaul>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().MutantAntibodies = true;

            DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
            player.GetDamage(damageClass) += 0.2f;

            player.rabid = true;
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;
        }
    }
}