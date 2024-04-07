using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class TheSmallSting : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HiveStaff>();
            // DisplayName.SetDefault("The Small Sting");
            /* Tooltip.SetDefault("Uses darts for ammo" +
                "\n50% chance to not consume ammo" +
                "\nStingers will stick to enemies, hitting the same spot again will deal extra damage" +
                "\n'Repurposed from the abdomen of a defeated foe..'"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.crit = 0;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = 50000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SmallStinger>();
            Item.useAmmo = AmmoID.Dart;
            Item.UseSound = SoundID.Item97;
            Item.shootSpeed = 40f;
            Item.width = 44;
            Item.height = 16;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
            float multiplier = 1; //markiplier
            if (player.strongBees)
            {
                multiplier += 0.1f;
            }
            damage = (int)(damage * multiplier);
            knockback = (int)(knockback * multiplier);
        }

        // Remove the Crit Chance line because of a custom crit method
        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => tooltips.Remove(tooltips.FirstOrDefault(line => line.Name == "CritChance" && line.Mod == "Terraria"));

        //make them hold it different
        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

    }
}