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
            /*DisplayName.SetDefault("Pumpking's Cape");
            Tooltip.SetDefault(@"Grants immunity to Living Wasteland
Increases damage and critical strike chance by 5%
Your critical strikes inflict Rotting
You may periodically fire additional attacks depending on weapon type
'Somehow, it's the right size'");
            DisplayName.AddTranslation(GameCulture.Chinese, "南瓜王的披肩");
            Tooltip.AddTranslation(GameCulture.Chinese, @"'不知怎么的,它的尺寸正好合适'
免疫人形废土
增加5%伤害和暴击率
暴击造成腐败
根据武器类型定期发动额外的攻击");*/
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