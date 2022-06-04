using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class QueenStinger : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Queen's Stinger");
            Tooltip.SetDefault("Grants immunity to Infested" +
                "\nYour attacks inflict Poisoned and spray honey that increases your life regeneration" +
                "\nHoney buff increases your armor penetration by 10" +
                "\nBees and weak Hornets become friendly" +
                "\n'Ripped right off of a defeated foe'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "女王的毒刺");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'从一个被打败的敌人身上撕下来'" +
            //     "\n免疫感染" +
            //     "\n增加10点护甲穿透" +
            //     "\n攻击造成中毒效果" +
            //     "\n永久蜂蜜Buff效果" +
            //     "\n蜜蜂和虚弱黄蜂变得友好");

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
            player.buffImmune[Terraria.ModLoader.ModContent.BuffType<Buffs.Masomode.Infested>()] = true;

            // Bees
            player.npcTypeNoAggro[NPCID.Bee] = true;
            player.npcTypeNoAggro[NPCID.BeeSmall] = true;

            // Hornets
            player.npcTypeNoAggro[NPCID.Hornet] = true;
            player.npcTypeNoAggro[NPCID.HornetFatty] = true;
            player.npcTypeNoAggro[NPCID.HornetHoney] = true;
            player.npcTypeNoAggro[NPCID.HornetLeafy] = true;
            player.npcTypeNoAggro[NPCID.HornetSpikey] = true;
            player.npcTypeNoAggro[NPCID.HornetStingy] = true;

            // Stringer immune
            player.GetModPlayer<FargoSoulsPlayer>().QueenStingerItem = Item;
        }
    }
}