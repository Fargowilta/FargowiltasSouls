using FargowiltasSouls.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
10% increased movement speed, 10% increased max life, increased turnaround traction
You spawn mini eaters to seek out enemies every few attacks
Creepers hover around you blocking some damage
A new Creeper appears every 15 seconds, and 5 can exist at once
Creeper respawn speed increases when not moving
'It pulses with vitality'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "纯净之心");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"它充满活力地跳动着'
            // 免疫腐败和嗜血
            // 免疫地形Debuff
            // 增加20%移动速度和最大生命值
            // 每隔几次攻击就会产生一个迷你噬魂者追踪敌人
            // 爬行者徘徊在周围来阻挡伤害
            // 每15秒生成一个新的爬行者,最多同时存在5个");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            fargoPlayer.PureHeart = true;

            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Rotting>()] = true;
            player.moveSpeed += 0.1f;
            fargoPlayer.DarkenedHeartItem = Item;
            if (fargoPlayer.DarkenedHeartCD > 0)
                fargoPlayer.DarkenedHeartCD--;

            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Bloodthirsty>()] = true;
            player.statLifeMax2 += player.statLifeMax / 10;
            fargoPlayer.GuttedHeart = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<DarkenedHeart>())
            .AddIngredient(ModContent.ItemType<GuttedHeart>())
            //.AddIngredient(ModContent.ItemType<VolatileEnergy>(), 20);
            .AddIngredient(ItemID.PurificationPowder, 30)
            .AddIngredient(ItemID.GreenSolution, 50)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}