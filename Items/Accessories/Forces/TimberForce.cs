using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class TimberForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Timber");

            Tooltip.SetDefault(
@"Turns certain critters into weapons
Attacks will periodically be accompanied by several snowballs
All grappling hooks shoot, pull, and retract 2.5x as fast
You have an aura of Shadowflame, Cursed Flames, and Bleeding
Double tap down to spawn a palm tree sentry that throws nuts at enemies
Projectiles may spawn a star when they hit something
'Extremely rigid'");
            DisplayName.AddTranslation(GameCulture.Chinese, "森林之力");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"将某些小动物转化为武器
攻击时定期释放几个雪球
所有钩爪的抛出速度、牵引速度和回收速度x2.5
召唤一个暗影焰、诅咒焰和鲜血光环环绕着你
双击“下”键会召唤一个会向敌人扔橡实的棕榈树哨兵
弹幕在击中敌人或物块时有几率生成一颗星星
“我超刚的”");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Purple;
            item.value = 600000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.WoodForce = true;
            //wood
            modPlayer.WoodEnchant = true;
            //boreal
            modPlayer.BorealEnchant = true;
            modPlayer.AdditionalAttacks = true;
            //mahogany
            modPlayer.MahoganyEnchant = true;

            //ebon
            modPlayer.EbonEffect();
            //shade
            modPlayer.ShadewoodEffect();

            //shade
            modPlayer.ShadeEnchant = true;
            //palm
            modPlayer.PalmEffect();
            //pearl
            modPlayer.PearlEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "WoodEnchant");
            recipe.AddIngredient(null, "BorealWoodEnchant");
            recipe.AddIngredient(null, "RichMahoganyEnchant");
            recipe.AddIngredient(null, "EbonwoodEnchant");
            recipe.AddIngredient(null, "ShadewoodEnchant");
            recipe.AddIngredient(null, "PalmWoodEnchant");
            recipe.AddIngredient(null, "PearlwoodEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
