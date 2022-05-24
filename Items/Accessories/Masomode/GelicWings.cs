using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Wings)]
    public class GelicWings : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gelic Wings");
            Tooltip.SetDefault(@"Grants immunity to Flipped caused by the Underground Hallow
Allows the wearer to double jump
When you land after a jump, slime spikes shoot out to your sides
'Step on me'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "明胶羽翼");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"免疫由地下神圣之地造成的翻转减益
            // 可让持有者二连跳
            // 落地时向左右发射出史莱姆尖刺
            // “踩在我身上”");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(100);
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
            player.buffImmune[ModContent.BuffType<FlippedHallow>()] = true;
            player.hasJumpOption_Unicorn = true;
            player.GetModPlayer<FargoSoulsPlayer>().GelicWingsItem = Item;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 6.75f;
            acceleration = 0.185f;
        }
    }
}