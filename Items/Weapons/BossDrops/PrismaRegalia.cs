using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class PrismaRegalia : SoulsItem
    {

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Prisma Regalia");
            Tooltip.SetDefault("Hold to charge for more damage \nWhile fully charged, dash and strike towards the mouse\n'The something of a foe something something...'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "");
        }

        public override void SetDefaults()
        {
            Item.mana = 0;
            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.width = 156;
            Item.height = 156;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 5);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<PrismaRegaliaProj>();
            Item.shootSpeed = 4f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
            
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.CanUseItem(player);
        }
    }
}