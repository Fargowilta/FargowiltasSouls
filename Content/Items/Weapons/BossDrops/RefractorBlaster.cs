using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.FargoSoulsUtil;
using Microsoft.Xna.Framework.Input;
using Terraria.Audio;
using Steamworks;
using System;


namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class RefractorBlaster : SoulsItem
    {

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Refractor Blaster");
            // Tooltip.SetDefault("'Modified from the arm of a defeated foe..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "变轨激光炮");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'由一个被击败的敌人的手臂改装而来..'");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LaserRifle);
            Item.knockBack = 2.5f;
            Item.scale = 0.75f;
            Item.damage = 25;
            Item.useTime = 2;
            Item.useAnimation = 24;
            Item.shootSpeed = 15f;
            Item.value = 100000;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<PrimeLaser>();
            Item.UseSound = SoundID.Item12 with { Volume = 0.5f };
            Item.mana = 10;
            
            
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 1f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int NumProjectiles = 1; // The number of projectiles that this gun will shoot.
            
            for (int i = 0; i < NumProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(28 + Main.rand.NextFloat(1f, 35f)));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }
            for (int i = 0; i < NumProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(-28 + Main.rand.NextFloat(-1f, -35f)));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            return false; // Return false because we don't want tModLoader to shoot projectile
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}
