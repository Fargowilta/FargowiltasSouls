using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class CrystallineCongregation : SoulsItem
    {
        private int delay = 0;
        private bool lastLMouse = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystalline Congregation");
            // Tooltip.SetDefault("Summon up to 20 crystal balls near your mouse cursor that home towards you \n Release the button to send them towards your cursor");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.DamageType = DamageClass.Magic;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item101;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrystallineCongregationProj>();
            Item.shootSpeed = 1f;
            Item.noMelee = true;
            Item.mana = 7;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0f);
        }
        public override void HoldItem(Player player)
        {
            if (lastLMouse && !Main.mouseLeft && delay == 0)
                delay = 70;
            if (delay > 0)
                delay--;
            if (delay == 1)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ChargeSound"), player.Center);
                }
                //dust
                double spread = 2 * Math.PI / 36;
                for (int i = 0; i < 36; i++)
                {
                    Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                    int index2 = Dust.NewDust(player.Center, 0, 0, DustID.PurpleCrystalShard, velocity.X, velocity.Y, 100);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                }
            }
            lastLMouse = Main.mouseLeft;
            base.HoldItem(player);
        }
        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] >= 30)
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.UseSound = SoundID.Item43 with
                {
                    Volume = 0.25f,
                    MaxInstances = 10
                };
            }
            else
            {
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.UseSound = SoundID.Item101;
            }
            return delay <= 0 && base.CanUseItem(player);

        }
        public override bool CanShoot(Player player) //different from CanUseItem because here you hold weapon out, and use mana
        {
            return player.ownedProjectileCounts[Item.shoot] < 30 && base.CanShoot(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int spread = 35;
            Vector2 pos = Main.MouseWorld + new Vector2(Main.rand.Next(-spread, spread), Main.rand.Next(-spread, spread));
            Projectile.NewProjectile(source, pos.X, pos.Y, 0f, 0f, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<LifelightBag>(2).AddTile(TileID.Solidifier).Register();
        }
    }
}
