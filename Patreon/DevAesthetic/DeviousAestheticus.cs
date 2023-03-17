using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    public class DeviousAestheticus : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Devious Aestheticus");
            Tooltip.SetDefault(
@"Shot spread scales with up to 6 empty minion slots
'If you're seeing this, You've been in a coma for 20 years, I don't know where this message will be, but please wake up'");
        }

        public override void SetDefaults()
        {
            Item.damage = 366;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DevRocket>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            damage = (int)(damage / 4.0 * 1.3);

            float minionSlotsUsed = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && !Main.projectile[i].hostile && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].minionSlots > 0)
                    minionSlotsUsed += Main.projectile[i].minionSlots;
            }

            float modifier = player.maxMinions - minionSlotsUsed;
            if (modifier < 1)
                modifier = 1;
            if (modifier > 7)
                modifier = 7;

            float spread = MathHelper.ToRadians(60f / 3.5f);
            if (modifier % 2 == 0)
            {
                Vector2 baseSpeed = velocity.RotatedBy(spread * (-modifier / 2 + 0.5f)); //half offset for v spread
                for (int i = 0; i < modifier; i++)
                    Projectile.NewProjectile(source, position, baseSpeed.RotatedBy(spread * (i + Main.rand.NextFloat(-0.5f, 0.5f))), type, damage, knockback, player.whoAmI);
            }
            else
            {
                Vector2 baseSpeed = velocity;
                int max = (int)modifier / 2;
                for (int i = -max; i <= max; i++)
                    Projectile.NewProjectile(source, position, baseSpeed.RotatedBy(spread * (i + Main.rand.NextFloat(-0.5f, 0.5f))), type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }
}
