using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrystalAssassinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Crystal Assassin Enchantment");
            Tooltip.SetDefault(
@"Allows the ability to dash
Use Ninja hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
Effects of Volatile Gel
'Deceptively fragile'");
        }

        protected override Color nameColor => new Color(36, 157, 207);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            NinjaEnchant.NinjaEffect(player);
            CrystalAssassinEffect(player, Item);
        }

        public static void CrystalAssassinEffect(Player player, Item item)
        {
            if (player.GetToggleValue("CrystalDash", false))
                player.dashType = 5;
            if (player.GetToggleValue("CrystalGelatin"))
                VolatileGelatin(player, item);
        }

        public static void VolatileGelatin(Player player, Item sourceItem)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }
            player.volatileGelatinCounter++;
            if (player.volatileGelatinCounter > 50)
            {
                player.volatileGelatinCounter = 0;
                int damage = 65;
                float knockBack = 7f;
                float num = 640f;
                NPC npc = null;
                for (int i = 0; i < 200; i++)
                {
                    NPC npc2 = Main.npc[i];
                    if (npc2 != null && npc2.active && npc2.CanBeChasedBy(player, false) && Collision.CanHit(player, npc2))
                    {
                        float num2 = Vector2.Distance(npc2.Center, player.Center);
                        if (num2 < num)
                        {
                            num = num2;
                            npc = npc2;
                        }
                    }
                }
                if (npc != null)
                {
                    Vector2 vector = npc.Center - player.Center;
                    vector = vector.SafeNormalize(Vector2.Zero) * 6f;
                    vector.Y -= 2f;
                    Projectile.NewProjectile(player.GetSource_Accessory(sourceItem), player.Center.X, player.Center.Y, vector.X, vector.Y, 937, damage, knockBack, player.whoAmI, 0f, 0f);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalNinjaHelmet)
                .AddIngredient(ItemID.CrystalNinjaChestplate)
                .AddIngredient(ItemID.CrystalNinjaLeggings)
                .AddIngredient(ModContent.ItemType<NinjaEnchant>())
                .AddIngredient(ItemID.VolatileGelatin)
                .AddIngredient(ItemID.FlyingKnife)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
