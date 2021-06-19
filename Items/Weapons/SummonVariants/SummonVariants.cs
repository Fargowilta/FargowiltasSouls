using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SummonVariants
{
    public class BatScepterSummon : BaseSummonItem
    {
        public override int Type => ItemID.BatScepter;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Summons bats to attack your enemies");
            Item.staff[item.type] = true;
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤蝙蝠去攻击你的敌人");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int amount = Main.rand.Next(1, 4);
            for (int i = 0; i < amount; i++)
            {
                speedX += Main.rand.Next(-35, 36) * 0.05f;
                speedY += Main.rand.Next(-35, 36) * 0.05f;
                int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                Main.projectile[proj].minion = true;
                Main.projectile[proj].magic = false;
            }

            return false;
        }
    }

    public class WaspGunSummon : BaseSummonItem
    {
        public override int Type => ItemID.WaspGun;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int amount = Main.rand.Next(2, 5);
            for (int i = 0; i <= 2; i++)
            {
                if (Main.rand.NextBool(5))
                {
                    amount++;
                }
            }

            for (int i = 0; i < amount; i++)
            {
                speedX += Main.rand.Next(-35, 36) * 0.02f;
                speedY += Main.rand.Next(-35, 36) * 0.02f;
                int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                Main.projectile[proj].minion = true;
                Main.projectile[proj].magic = false;
            }

            return false;
        }
    }

    public class PiranhaGunSummon : BaseSummonItem
    {
        public override int Type => ItemID.PiranhaGun;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Latches on to enemies for continuous damage");
        }
    }

    public class NimbusRodSummon : BaseSummonItem
    {
        public override int Type => ItemID.NimbusRod;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Summons a cloud to rain down on your foes");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].ai[0] = Main.MouseWorld.X;
            Main.projectile[proj].ai[1] = Main.MouseWorld.Y;

            Main.projectile[proj].minion = true;
            Main.projectile[proj].magic = false;
            return false;
        }
    }

    public class CrimsonRodSummon : BaseSummonItem
    {
        public override int Type => ItemID.CrimsonRod;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Summons a cloud to rain blood on your foes");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].ai[0] = Main.MouseWorld.X;
            Main.projectile[proj].ai[1] = Main.MouseWorld.Y;

            Main.projectile[proj].minion = true;
            Main.projectile[proj].magic = false;

            return false;
        }
    }

    public class BeeGunSummon : BaseSummonItem
    {
        public override int Type => ItemID.BeeGun;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Shoots bees that will chase your enemy");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int amount = Main.rand.Next(1, 4);
            for (int i = 0; i <= 2; i++)
            {
                if (Main.rand.NextBool(6))
                {
                    amount++;
                }
            }

            if (player.strongBees && Main.rand.NextBool(3))
            {
                amount++;
            }

            for (int i = 0; i < amount; i++)
            {
                speedX += Main.rand.Next(-35, 36) * 0.02f;
                speedY += Main.rand.Next(-35, 36) * 0.02f;
                int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, player.beeType(), player.beeDamage(damage), player.beeKB(knockBack), player.whoAmI);
                Main.projectile[proj].minion = true;
                Main.projectile[proj].magic = false;
            }

            return false;
        }
    }

    //magnet sphere

    //spirit lamp thing

    //book of skulls

    //scourge

    //hellwing

    //sword fish

    //rockfish

    //obsidian swordfish

    //toxicarp

    //purple clubberfish

    //crystal serpent

    //phantom phoenix

    //flying knife

    //golem fist whip?? pog

    //reaver shark

    //sawtooth shark

    //KITES
}
