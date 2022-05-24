using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SparklingAdoration : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Adoration");
            Tooltip.SetDefault(@"Grants immunity to Lovestruck and Fake Hearts
Graze projectiles to gain up to 25% increased critical damage
Critical damage bonus decreases over time and is fully lost on hit
This damage bonus can apply to summon damage even without a critical hit
Your attacks periodically summon life-draining hearts
'With all of your emotion!'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Lovestruck>()] = true;

            if (player.GetToggleValue("MasoGraze", false))
                fargoPlayer.Graze = true;

            fargoPlayer.DevianttHeartItem = Item;

            if (fargoPlayer.Graze && player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoGrazeRing", false) && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.GrazeRing>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GrazeRing>(), 0, 0f, Main.myPlayer);
        }
    }
}