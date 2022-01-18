using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using FargowiltasSouls.Items;
using System.Linq;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Prime Staff");
            //Tooltip.SetDefault("Summons Skeletron Prime to fight for you\n'Using expert hacking skills (turning it off and on again), you've reprogrammed a terror of the night!'");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public int counter;

        public override void SetDefaults()
        {
            item.damage = 60;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.rare = 5;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<PrimeMinionProj>();
            item.shootSpeed = 10f;
            item.buffType = mod.BuffType("PrimeMinionBuff");
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 8);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
                    Projectile.NewProjectile(spawnPos, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                }

                if (++counter >= 4)
                    counter = 0;

                int limbType;
                switch (counter)
                {
                    case 0: limbType = ModContent.ProjectileType<PrimeMinionVice>(); break;
                    case 1: limbType = ModContent.ProjectileType<PrimeMinionSaw>(); break;
                    case 2: limbType = ModContent.ProjectileType<PrimeMinionLaserGun>(); break;
                    default: limbType = ModContent.ProjectileType<PrimeMinionCannon>(); break;
                }
                    
                Projectile.NewProjectile(spawnPos, Main.rand.NextVector2Circular(10, 10), limbType, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
