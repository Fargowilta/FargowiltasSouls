using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class BoneZone : SoulsItem
    {
        private int counter = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Bone Zone");
            Tooltip.SetDefault("Uses bones for ammo" +
                "\n33% chance to not consume ammo" +
                "\n'The shattered remains of a defeated foe..'");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "骸骨领域");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'被击败的敌人的残骸..'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 14;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item2;
            Item.value = 50000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Bonez>();
            Item.shootSpeed = 5.5f;
            Item.useAmmo = ItemID.Bone;
        }

        // Manually reposition the Item when held out
        public override Vector2? HoldoutOffset() => new Vector2(-30, 4);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int shoot;

            if (counter > 2)
            {
                shoot = ProjectileID.ClothiersCurse;
                counter = 0;
            }
            else
                shoot = ModContent.ProjectileType<Bonez>();

            Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, shoot, damage, knockback, player.whoAmI)].DamageType = DamageClass.Ranged;

            counter++;

            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(3);
    }
}