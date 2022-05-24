using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SqueakyToy : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squeaky Toy");
            Tooltip.SetDefault(@"Grants immunity to Squeaky Toy and Guilty
Attacks have a chance to squeak and deal 1 damage to you
'The beloved toy of a defeated foe...?'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "吱吱响的玩具");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被打败的敌人心爱的玩具...?
            // 免疫吱吱响的玩具和净化
            // 敌人攻击概率发出吱吱声,并只造成1点伤害");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToy>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Guilty>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().SqueakyAcc = true;
        }
    }
}