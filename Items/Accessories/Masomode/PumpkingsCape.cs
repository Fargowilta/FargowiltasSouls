using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Front, EquipType.Back)]
    public class PumpkingsCape : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpking's Cape");
            Tooltip.SetDefault(@"Grants immunity to Living Wasteland
Increases damage and critical strike chance by 5%
Your critical strikes inflict Rotting
You may periodically fire additional attacks depending on weapon type
'Somehow, it's the right size'");
            DisplayName.AddTranslation(GameCulture.Chinese, "南瓜王的披肩");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫人型废土减益
增加5%伤害和暴击率
攻击造成暴击时会造成腐败减益
根据手持武器的类型定期发动额外攻击
'不知为何，它的大小对你来说正好合适'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.AllDamageUp(0.05f);
            fargoPlayer.AllCritUp(5);
            fargoPlayer.PumpkingsCape = true;
            fargoPlayer.AdditionalAttacks = true;
            player.buffImmune[mod.BuffType("LivingWasteland")] = true;
        }
    }
}
