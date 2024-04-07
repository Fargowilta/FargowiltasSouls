using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class DestroyerGun : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Destroyer Gun");
            // Tooltip.SetDefault("Becomes longer and faster with up to 3 empty minion slots\n'An old foe beaten into submission..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "毁灭者之枪");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'一个被迫屈服的老对手..'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.NPCDeath13;
            Item.value = 50000;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DestroyerHead>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            FargoSoulsUtil.NewSummonProjectile(source, position, velocity, type, Item.damage, knockback, player.whoAmI);
            return false;
        }
    }
}