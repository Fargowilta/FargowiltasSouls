using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Neck)]
    public class SkullCharm : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Charm");
            Tooltip.SetDefault(@"Grants immunity to Dazed and Stunned
Increases damage taken and dealt by 10%
Enemies are less likely to target you
Makes armed and magic skeletons less hostile outside the Dungeon
'No longer in the zone'");
            DisplayName.AddTranslation(GameCulture.Chinese, "头骨挂坠");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫眩晕和错愕减益
增加10%受到的和造成的伤害
减少敌人以你为目标的几率
在地牢外减少武装和魔法骷髅对你的敌意
'不在这个区域了'");
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
            player.buffImmune[BuffID.Dazed] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Stunned>()] = true;
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.endurance -= 0.1f;
            player.aggro -= 400;
            player.GetModPlayer<FargoPlayer>().SkullCharm = true;
            if (!player.ZoneDungeon)
            {
                player.npcTypeNoAggro[NPCID.SkeletonSniper] = true;
                player.npcTypeNoAggro[NPCID.SkeletonCommando] = true;
                player.npcTypeNoAggro[NPCID.TacticalSkeleton] = true;
                player.npcTypeNoAggro[NPCID.DiabolistRed] = true;
                player.npcTypeNoAggro[NPCID.DiabolistWhite] = true;
                player.npcTypeNoAggro[NPCID.Necromancer] = true;
                player.npcTypeNoAggro[NPCID.NecromancerArmored] = true;
                player.npcTypeNoAggro[NPCID.RaggedCaster] = true;
                player.npcTypeNoAggro[NPCID.RaggedCasterOpenCoat] = true;
            }
        }
    }
}
