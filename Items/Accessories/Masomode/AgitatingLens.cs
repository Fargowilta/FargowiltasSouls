using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class AgitatingLens : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Agitating Lens");
            Tooltip.SetDefault("Grants immunity to Berserked" +
                "\n10% increased damage when below half HP" +
                "\nWhile dashing or running quickly you will create a trail of demon scythes" +
                "\n'The irritable remnant of a defeated foe'");

            DisplayName.AddTranslation(GameCulture.Chinese, "躁动晶状体");
            Tooltip.AddTranslation(GameCulture.Chinese, "使你狂暴免疫减益" +
                "\n生命值低于50%时增加10%伤害" +
                "\n冲刺或奔跑时会在身后留下一串恶魔镰刀" +
                "\n'被打败的敌人的躁动残渣'");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("Berserked")] = true;

            if (player.statLife < player.statLifeMax2 / 2)
                player.GetModPlayer<FargoPlayer>().AllDamageUp(.10f);

            player.GetModPlayer<FargoPlayer>().AgitatingLens = true;
        }
    }
}
