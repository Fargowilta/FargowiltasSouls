using FargowiltasSouls.Projectiles.BossWeapons;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class FleshHand : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Flesh Hand");
            Tooltip.SetDefault("'The enslaved minions of a defeated foe..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "血肉之手");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'战败敌人的仆从..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.NPCDeath13;
            Item.value = 50000;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Hungry>();
            Item.shootSpeed = 20f;
            Item.noUseGraphic = true;
        }
    }
}