using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs;
using Terraria.ID;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class OrichalcumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Orichalcum Enchantment");
            /* Tooltip.SetDefault(
@"Flower petals will cause extra damage to your target and inflict Orichalcum Poison
Damaging debuffs deal 2.5x damage
'Nature blesses you'"); */
        }

        protected override Color nameColor => new(235, 50, 145);
        

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
            .AddIngredient(ItemID.FlowerofFire)
            .AddIngredient(ItemID.FlowerofFrost)
            .AddIngredient(ItemID.CursedFlames)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class OrichalcumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override bool HasToggle => true;
        public override bool ExtraAttackEffect => true;

        public static void OriDotModifier(NPC npc, FargoSoulsPlayer modPlayer, ref int damage)
        {
            float multiplier = 2.5f;

            if (modPlayer.Player.ForceEffect<OrichalcumEffect>())
            {
                multiplier = 4f;
            }

            npc.lifeRegen = (int)(npc.lifeRegen * multiplier);
            damage = (int)(damage * multiplier);

            //half as effective if daybreak applied
            if (npc.daybreak)
            {
                npc.lifeRegen /= 2;
                damage /= 2;
            }
        }

        public override void PostUpdateEquips(Player player)
        {
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
