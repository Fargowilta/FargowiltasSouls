using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class TwinRangs : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            // DisplayName.SetDefault("Twinrangs");
            /* Tooltip.SetDefault("Fire a different twinrang depending on mouse click" +
                "\n'The compressed forms of defeated foes..'"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "双子");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "被打败的敌人的压缩形态..");
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = 100000;
            Item.rare = ItemRarityID.Pink;
            Item.shootSpeed = 10;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {   
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Retirang>()] >= 3)
            {
                return false;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<Retirang>()] < 3)
            {   
                Item.shoot = ModContent.ProjectileType<Retirang>();
                Item.shootSpeed = 10;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<Spazmarang>();
                Item.shootSpeed = 15;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 0.75);
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}