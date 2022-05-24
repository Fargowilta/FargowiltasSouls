using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class AgitatingLens : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Agitating Lens");
            Tooltip.SetDefault("Grants immunity to Berserked" +
                "\nWhen below half HP, gain 10% increased damage, speed, and attack speed" +
                "\nWhile dashing or running quickly you will create a trail of demon scythes" +
                "\n'The irritable remnant of a defeated foe'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "躁动晶状体");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使你狂暴免疫减益" +
            //     "\n生命值低于50%时增加10%伤害" +
            //     "\n冲刺或奔跑时会在身后留下一串恶魔镰刀" +
            //     "\n'被打败的敌人的躁动残渣'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Berserked>()] = true;

            player.GetModPlayer<FargoSoulsPlayer>().AgitatingLensItem = Item;
        }
    }
}
