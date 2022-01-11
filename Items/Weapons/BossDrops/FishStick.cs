using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class FishStick : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fish Stick");
            //Tooltip.SetDefault("'The carcass of a defeated foe shoved violently on a stick..'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "鱼杖");
            //Tooltip.AddTranslation(GameCulture.Chinese, "'一个被打败的敌人的尸体,用棍子粗暴地串起来..'");
        }

        public override void SetDefaults()
        {
            item.damage = 77;
            item.ranged = true;
            //item.mana = 10;
            item.width = 24;
            item.height = 24;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(0, 6);
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FishStickProj>();
            item.shootSpeed = 35f;
            item.noUseGraphic = true;
        }
    }
}
