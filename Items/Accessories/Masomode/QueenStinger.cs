using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Utilities;
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
                "\nIncreases armor penetration by 10" +
                "\nYour attacks inflict Poisoned and spray honey that increases your life regeneration" +
                "\nBees and weak Hornets become friendly" +
                "\n'Ripped right off of a defeated foe'");

            DisplayName.AddTranslation(GameCulture.Chinese, "女王的毒刺");
            Tooltip.AddTranslation(GameCulture.Chinese, "使你免疫感染减益" +
                "\n增加10点护甲穿透" +
                "\n攻击会造成中毒减益并喷出会增加你的生命恢复速度的蜂蜜" +
                "\n使蜜蜂和弱小的黄蜂变得友好" +
                "\n'从一位被打败的敌人的身上撕下来的'");
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
            //player.honey = true;
            player.armorPenetration += 10;
            player.buffImmune[mod.BuffType("Infested")] = true;

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
            player.GetModPlayer<FargoPlayer>().QueenStinger = true;
        }
    }
}
