using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class NecromanticBrew : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necromantic Brew");
            Tooltip.SetDefault(@"Grants immunity to Lethargic
Increases speed when dashing by 50%
When dashing, take 75% less contact damage and reflect 100% of contact damage on attacker
Summons 2 Skeletron arms to whack enemies
'The bone-growing solution of a defeated foe'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死灵密酿");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"被击败敌人的促进骨生长的溶液
            // 免疫昏昏欲睡
            // 召唤2个骷髅王手臂重击敌人
            // 可能会吸引宝宝骷髅头");

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
            player.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().NecromanticBrewItem = Item;
            if (player.GetToggleValue("MasoSkele"))
                player.AddBuff(ModContent.BuffType<SkeletronArms>(), 2);
        }
    }
}