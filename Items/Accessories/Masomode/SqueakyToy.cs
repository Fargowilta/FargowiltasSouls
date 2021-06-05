using Terraria;
using Terraria.Localization;
using Terraria.ID;

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
            DisplayName.AddTranslation(GameCulture.Chinese, "吱吱响的玩具");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫吱吱作响的玩具和愧疚减益
你在受到伤害时有几率发出吱吱声，并使这次受到的伤害降至1点
'一位被打败的敌人的心爱的玩具...？'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("SqueakyToy")] = true;
            player.buffImmune[mod.BuffType("Guilty")] = true;
            player.GetModPlayer<FargoPlayer>().SqueakyAcc = true;
        }
    }
}
