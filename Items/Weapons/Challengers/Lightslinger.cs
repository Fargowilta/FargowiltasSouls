using FargowiltasSouls.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class Lightslinger : SoulsItem
    {
        int ShotType = ModContent.ProjectileType<LightslingerShot>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightslinger");
            Tooltip.SetDefault("Converts bullets to hallowed shots of light\nAfter 25 shots, press Right Click to fire a lightbomb\n25% chance to not consume ammo");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 76;
            Item.height = 48;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 25);
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
                damage *= 5;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-27f, -12f);
        }

        public override bool AltFunctionUse(Player player) => player.GetModPlayer<FargoSoulsPlayer>().LightslingerHitShots >= 20 ? true : false;
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
                player.GetModPlayer<FargoSoulsPlayer>().LightslingerHitShots = 0;
            }
            else
            {
                if (++player.GetModPlayer<FargoSoulsPlayer>().LightslingerHitShots >= 25)
                    SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
            }

            return base.UseItem(player);
        }
    }
}