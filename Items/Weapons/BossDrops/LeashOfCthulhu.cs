using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class LeashOfCthulhu : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Leash of Cthulhu");
            //Tooltip.SetDefault("'The mutilated carcass of a defeated foe...'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "克苏鲁连枷");
            //Tooltip.AddTranslation(GameCulture.Chinese, "'被打败的敌人的残尸...'");
        }

        public override void SetDefaults()
        {
            item.damage = 13;
            item.width = 30;
            item.height = 10;
            item.value = Item.sellPrice(0, 1);
            item.rare = ItemRarityID.Blue;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 25;
            item.useTime = 25;
            item.knockBack = 4f;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<LeashFlail>();
            item.shootSpeed = 25f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.autoReuse = true;
        }
    }
}