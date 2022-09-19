using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class GroundStick : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Remote Control");
            Tooltip.SetDefault(@"Grants immunity to Lightning Rod
Electric and ray attacks supercharge you and do halved damage if not already supercharged
While supercharged, you have increased movement speed, attack speed, and inflict Electrified and Lightning Rod
Two friendly probes fight by your side and will supercharge with you
'A defeated foe's segment with an antenna glued on'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "遥控装置");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被击败敌人的残片,上面粘着天线'
            // 免疫避雷针
            // 攻击小概率造成避雷针效果
            // 召唤2个友善的探测器为你而战");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().GroundStick = true;
            if (player.GetToggleValue("MasoProbe"))
                player.AddBuff(ModContent.BuffType<Probes>(), 2);
        }
    }
}