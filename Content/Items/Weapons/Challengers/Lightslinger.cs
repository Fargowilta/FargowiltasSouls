using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class Lightslinger : SoulsItem
    {
        const int ReqShots = 40;
        int ShotType = ModContent.ProjectileType<LightslingerShot>();

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightslinger");
            // Tooltip.SetDefault("Converts bullets to hallowed shots of light\nAfter 40 shots, press Right Click to fire a lightbomb\n25% chance to not consume ammo");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 76;
            Item.height = 48;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightslingerShot>();
            Item.shootSpeed = 12f;

            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(4);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ShotType;
            if (player.altFunctionUse == 2)
                damage *= 10;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-27f, -12f);
        }

        public override bool AltFunctionUse(Player player) => player.FargoSouls().LightslingerHitShots >= ReqShots;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                ShotType = ModContent.ProjectileType<LightslingerBomb>();
                Item.shootSpeed = 12f;
                Item.UseSound = SoundID.Item91;
                Item.useAnimation = 30;
                Item.useTime = 30;

            }
            else
            {
                Item.UseSound = SoundID.Item12;
                ShotType = ModContent.ProjectileType<LightslingerShot>();
                Item.shootSpeed = 16f;
                Item.useAnimation = 6;
                Item.useTime = 6;
            }
            return base.CanUseItem(player);
        }
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.FargoSouls().LightslingerHitShots = 0;
            }
            else
            {
                if (++player.FargoSouls().LightslingerHitShots >= ReqShots && player.whoAmI == Main.myPlayer)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ChargeSound"), player.Center);
            }

            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<LifelightBag>(2).AddTile(TileID.Solidifier).Register();
        }
    }
}