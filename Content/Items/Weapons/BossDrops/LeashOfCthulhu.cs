using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class LeashOfCthulhu : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Leash of Cthulhu");
            // Tooltip.SetDefault("'The mutilated carcass of a defeated foe...'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁连枷");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'被打败的敌人的残尸...'");
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.knockBack = 4f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<LeashFlail>();
            Item.shootSpeed = 25f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }
    }
}