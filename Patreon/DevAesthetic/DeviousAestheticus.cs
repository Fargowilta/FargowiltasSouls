using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles;
using System.Collections.Generic;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    public class DeviousAestheticus : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devious Aestheticus");
            Tooltip.SetDefault(
@"Shot spread scales with up to 6 empty minion slots
'If you're seeing this, You've been in a coma for 20 years, I don't know where this message will be, but please wake up'");
        }

        public override void SetDefaults()
        {
            item.damage = 222;
            item.summon = true;
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
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage / 4.0 * 1.1);

            float modifier = player.maxMinions - player.slotsMinions + 1;
            if (modifier < 1)
                modifier = 1;
            if (modifier > 7)
                modifier = 7;
            
            if (modifier % 2 == 0)
            {
                float spread = MathHelper.ToRadians(80 / 3.5f);
                Vector2 baseSpeed = new Vector2(speedX, speedY).RotatedBy(spread * (-modifier / 2 + 0.5f)); //half offset for v spread
                for (int i = 0; i < modifier; i++)
                    Projectile.NewProjectile(player.Center, baseSpeed.RotatedBy(spread * i), type, damage, knockback, player.whoAmI);
            }
            else
            {
                int p = Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), type, damage, knockback, player.whoAmI);
                if (p != Main.maxProjectiles)
                {
                    float spread = MathHelper.ToRadians(80f / 7 * modifier * 2);
                    FargoGlobalProjectile.SplitProj(Main.projectile[p], (int)modifier, spread, 1);
                }
            }

            return false;
        }
    }
}
