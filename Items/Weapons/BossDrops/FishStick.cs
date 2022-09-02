using FargowiltasSouls.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class FishStick : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Fish Stick");
            Tooltip.SetDefault("Right click to throw a sharknado stick\n'The carcass of a defeated foe shoved violently on a stick..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "鱼杖");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'一个被打败的敌人的尸体,用棍子粗暴地串起来..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 77;
            Item.DamageType = DamageClass.Ranged;
            //Item.mana = 10;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 6);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FishStickProj>();
            Item.shootSpeed = 35f;
            Item.noUseGraphic = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.shoot = player.altFunctionUse == 2
                ? ModContent.ProjectileType<FishStickProjTornado>()
                : ModContent.ProjectileType<FishStickProj>();

            return base.CanUseItem(player);
        }
    }
}