using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class Dicer : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Dicer");
            // Tooltip.SetDefault("'A defeated foe's attack now on a string'");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "切肉器");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'一个被击败的敌人的攻击,用线拴着'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            ItemID.Sets.Yoyo[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<DicerYoyo>();
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.shootSpeed = 16f;
            Item.knockBack = 2.5f;
            Item.damage = 60;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void HoldItem(Player player) => player.stringColor = 5;
    }
}