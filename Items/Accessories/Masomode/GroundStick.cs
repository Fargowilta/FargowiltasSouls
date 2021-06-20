using Terraria;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class GroundStick : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Remote Control");
            Tooltip.SetDefault(@"Grants immunity to Lightning Rod
Your attacks have a small chance to inflict Lightning Rod
Two friendly probes fight by your side
'A defeated foe's segment with an antenna glued on'");
            DisplayName.AddTranslation(GameCulture.Chinese, "遥控装置");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫避雷针
攻击小概率造成避雷针效果
召唤2个友善的探测器为你而战
'被击败敌人的残片,上面粘着天线'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("LightningRod")] = true;
            player.GetModPlayer<FargoPlayer>().GroundStick = true;
            if (player.GetToggleValue("MasoProbe"))
                player.AddBuff(mod.BuffType("Probes"), 2);
        }
    }
}
