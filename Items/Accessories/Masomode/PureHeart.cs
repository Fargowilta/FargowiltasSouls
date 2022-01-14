using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class PureHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Pure Heart");
            Tooltip.SetDefault(@"Grants immunity to Rotting and Bloodthirsty
Grants immunity to biome debuffs
20% increased movement speed and 20% increased max life
You spawn mini eaters to seek out enemies every few attacks
Creepers hover around you blocking some damage
A new Creeper appears every 15 seconds, and 5 can exist at once
Creeper respawn speed increases when not moving
'It pulses with vitality'");
            DisplayName.AddTranslation(GameCulture.Chinese, "纯净之心");
            Tooltip.AddTranslation(GameCulture.Chinese, @"它充满活力地跳动着'
免疫腐败和嗜血
免疫地形Debuff
增加20%移动速度和最大生命值
每隔几次攻击就会产生一个迷你噬魂者追踪敌人
爬行者徘徊在周围来阻挡伤害
每15秒生成一个新的爬行者,最多同时存在5个");*/
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
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.PureHeart = true;
            player.statLifeMax2 += player.statLifeMax / 5;
            player.buffImmune[mod.BuffType("Rotting")] = true;
            player.moveSpeed += 0.2f;
            fargoPlayer.CorruptHeart = true;
            if (fargoPlayer.CorruptHeartCD > 0)
                fargoPlayer.CorruptHeartCD--;

            player.buffImmune[mod.BuffType("Bloodthirsty")] = true;
            fargoPlayer.GuttedHeart = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("CorruptHeart"));
            recipe.AddIngredient(mod.ItemType("GuttedHeart"));
            //recipe.AddIngredient(mod.ItemType("VolatileEnergy"), 20);
            recipe.AddIngredient(ItemID.PurificationPowder, 30);
            recipe.AddIngredient(ItemID.GreenSolution, 50);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 5);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}