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
            DisplayName.SetDefault("Pure Heart");
            Tooltip.SetDefault(@"Grants immunity to Rotting and Bloodthirsty
Grants immunity to biome debuffs
20% increased movement speed and 20% increased max life
You spawn mini eaters to seek out enemies every few attacks
Creepers hover around you blocking some damage
A new Creeper appears every 15 seconds, and 5 can exist at once
Creeper respawn speed increases when not moving
'It pulses with vitality'");
            DisplayName.AddTranslation(GameCulture.Chinese, "纯净之心");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫腐败和嗜血减益
使你免疫环境减益
增加20%移动速度和20%最大生命值
每攻击几次便释放迷你吞噬者来搜寻并攻击敌人
飞眼怪徘徊在你周围并阻挡一部分伤害
每过15秒便生成一只新的飞眼怪，至多存在5只飞眼怪
站定不动时增加飞眼怪的生成速度
'它充满活力的跳动着'");
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
