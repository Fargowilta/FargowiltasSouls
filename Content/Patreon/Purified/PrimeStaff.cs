using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Purified
{
    public class PrimeStaff : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Prime Staff");
            // Tooltip.SetDefault("Summons Skeletron Prime to fight for you\n'Using expert hacking skills (turning it off and on again), you've reprogrammed a terror of the night!'");
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<PrimeMinionProj>();
            Item.shootSpeed = 10f;
            Item.buffType = ModContent.BuffType<PrimeMinionBuff>();
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<PrimeMinionBuff>(), 2);
            Vector2 spawnPos = Main.MouseWorld;
            float usedminionslots = 0;
            var minions = Main.projectile.Where(x => x.minionSlots > 0 && x.owner == player.whoAmI && x.active);
            foreach (Projectile minion in minions)
                usedminionslots += minion.minionSlots;
            if (usedminionslots < player.maxMinions)
            {
                if (player.ownedProjectileCounts[type] == 0) //only spawn brain minion itself when the player doesnt have any, and if minion slots aren't maxxed out
                {
                    FargoSoulsUtil.NewSummonProjectile(source, spawnPos, Vector2.Zero, type, Item.damage, knockback, player.whoAmI);
                }

                if (++counter >= 4)
                    counter = 0;
                var limbType = counter switch
                {
                    0 => ModContent.ProjectileType<PrimeMinionVice>(),
                    1 => ModContent.ProjectileType<PrimeMinionSaw>(),
                    2 => ModContent.ProjectileType<PrimeMinionLaserGun>(),
                    _ => ModContent.ProjectileType<PrimeMinionCannon>(),
                };
                FargoSoulsUtil.NewSummonProjectile(source, spawnPos, Main.rand.NextVector2Circular(10, 10), limbType, Item.damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
