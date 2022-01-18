using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using FargowiltasSouls.Items;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    public class DeviousAestheticus : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Devious Aestheticus");
            Tooltip.SetDefault(
@"Shot spread scales with up to 6 empty minion slots
'If you're seeing this, You've been in a coma for 20 years, I don't know where this message will be, but please wake up'");*/
        }

        public override void SetDefaults()
        {
            item.damage = 222;
            item.summon = true;
            item.mana = 10;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 1;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DevRocket>();
            item.shootSpeed = 12f;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
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
                Vector2 baseSpeed = new Vector2(speedX, speedY).RotatedBy(spread * (-modifier / 2 + 0.5f)); //half offset for v spread
                for (int i = 0; i < modifier; i++)
                    Projectile.NewProjectile(player.Center, baseSpeed.RotatedBy(spread * (i + Main.rand.NextFloat(-0.5f, 0.5f))), type, damage, knockback, player.whoAmI);
            }
            else
            {
                Vector2 baseSpeed = new Vector2(speedX, speedY);
                int max = (int)modifier / 2;
                for (int i = -max; i <= max; i++)
                    Projectile.NewProjectile(player.Center, baseSpeed.RotatedBy(spread * (i + Main.rand.NextFloat(-0.5f, 0.5f))), type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }
}
