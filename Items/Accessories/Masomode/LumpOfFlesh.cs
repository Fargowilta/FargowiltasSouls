using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class LumpOfFlesh : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lump of Flesh");
            Tooltip.SetDefault(@"Grants immunity to Blackout, Obstructed, Dazed, and Stunned
Increases minion damage by 16% but slightly decreases defense
Increases your max number of minions by 2
Increases your max number of sentries by 2
The pungent eyeball charges energy to fire a laser as you attack
Enemies are less likely to target you
Makes armed and magic skeletons less hostile outside the Dungeon
'It's growing'");
            DisplayName.AddTranslation(GameCulture.Chinese, "肉团");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫黑视、遮蔽、眩晕和错愕减益
增加16%召唤伤害但微幅降低防御
+2最大召唤栏
+2最大哨兵栏
你在攻击时尖刻眼球会充能并发射激光
减少敌人以你为攻击目标的几率
在地牢外减少武装和魔法骷髅对你的敌意
'它在生长'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.buffImmune[BuffID.Dazed] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Stunned>()] = true;
            player.minionDamage += 0.16f;
            player.statDefense -= 6;
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
            player.maxMinions += 2;
            player.maxTurrets += 2;
            if (player.GetToggleValue("MasoPugent"))
                player.AddBuff(mod.BuffType("PungentEyeball"), 5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("PungentEyeball"));
            recipe.AddIngredient(mod.ItemType("SkullCharm"));
            recipe.AddIngredient(ItemID.SpectreBar, 10);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
