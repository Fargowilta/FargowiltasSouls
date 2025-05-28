using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class OrichalcumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(235, 50, 145);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<OrichalcumEffect>(Item);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyOriHead")
            .AddIngredient(ItemID.OrichalcumBreastplate)
            .AddIngredient(ItemID.OrichalcumLeggings)
            if (Main.remixWorld && !(Main.getGoodWorld && Main.zenithWorld)) //gfb should remain as is
            {
                .AddIngredient(ItemID.UnholyTrident)
            }
            else
            {
                .AddIngredient(ItemID.FlowerofFire)
            }
            .AddIngredient(ItemID.FlowerofFrost)
            .AddIngredient(ItemID.CursedFlames)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class OrichalcumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<OrichalcumEnchant>();

        public override bool ExtraAttackEffect => true;

        public static float OriDotModifier(NPC npc, FargoSoulsPlayer modPlayer)
        {
            float multiplier = 2.5f;

            if (modPlayer.Player.ForceEffect<OrichalcumEffect>())
            {
                multiplier = 3.5f;
            }
            return multiplier;
        }

        public override void PostUpdateEquips(Player player)
        {
            if (!HasEffectEnchant(player))
                return;
            player.onHitPetal = true;
        }

        public override void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.type == ProjectileID.FlowerPetal)
            {
                target.AddBuff(ModContent.BuffType<Content.Buffs.Souls.OriPoisonBuff>(), 300);
                target.immune[proj.owner] = 2;
            }
        }
    }
}
