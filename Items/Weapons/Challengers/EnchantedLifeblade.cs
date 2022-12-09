using FargowiltasSouls.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class EnchantedLifeblade : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Lifeblade");
            Tooltip.SetDefault("A living blade that will attack your mouse position");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
            public override void SetDefaults()
            {
                Item.width = 80;
                Item.height = 80;
                Item.damage = 60;
                Item.knockBack = 3f;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useAnimation = 45; //adjust
                Item.useTime = 45; //adjust
                Item.DamageType = DamageClass.Melee;
                Item.autoReuse = true;
                Item.noUseGraphic = true;
                Item.noMelee = true;
                //Item.channel = true;

                Item.rare = ItemRarityID.Pink;
                Item.value = Item.sellPrice(0, 25, 0, 0);
                Item.shoot = ModContent.ProjectileType<EnchantedLifebladeProjectile>(); 
                Item.shootSpeed = 2.1f; 
            }
            //player.SetCompositeFrontArm(params); for moving the arm

            public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
            {
                return base.Shoot(player, source, position, velocity, type, damage, knockback);
            }
        }
    }