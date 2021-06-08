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
            DisplayName.SetDefault("Fish Stick");
            Tooltip.SetDefault("'The carcass of a defeated foe shoved violently on a stick..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "鱼杖");
            Tooltip.AddTranslation(GameCulture.Chinese, "'一位被打败的敌人的尸体，被粗暴地用棍子串起来..'");
        }

        public override void SetDefaults()
        {
            item.damage = 66;
            item.ranged = true;
            //item.mana = 10;
            item.width = 24;
            item.height = 24;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item1;
            item.value = 50000;
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FishStickProj>();
            item.shootSpeed = 35f;
            item.noUseGraphic = true;
        }
    }
}
