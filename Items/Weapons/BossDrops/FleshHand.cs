using Terraria.Audio;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class FleshHand : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Flesh Hand");
            //Tooltip.SetDefault("'The enslaved minions of a defeated foe..'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "血肉之手");
            //Tooltip.AddTranslation(GameCulture.Chinese, "'战败敌人的仆从..'");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.magic = true;
            item.mana = 20;
            item.width = 24;
            item.height = 24;
            item.useTime = 32;
            item.useAnimation = 32;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = new LegacySoundStyle(4, 13);
            item.value = 50000;
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("Hungry");
            item.shootSpeed = 20f;
            item.noUseGraphic = true;
        }
    }
}