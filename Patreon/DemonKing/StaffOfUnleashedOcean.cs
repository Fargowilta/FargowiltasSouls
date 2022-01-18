using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using FargowiltasSouls.Items;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.DemonKing
{
    public class StaffOfUnleashedOcean : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Staff of Unleashed Ocean");
            //Tooltip.SetDefault("Summons Duke Fishron to fight for you\nNeeds 2 minion slots\n'Now channel your rage against them!'");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 3;
        }

        public override void SetDefaults()
        {
            item.damage = 375;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4f;
            item.rare = 11;
            item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Zombie, 20);
            item.shoot = mod.ProjectileType("DukeFishronMinion");
            item.shootSpeed = 10f;
            item.buffType = mod.BuffType("DukeFishronBuff");
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 25);
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            Vector2 spawnPos = Main.MouseWorld;
            Projectile.NewProjectile(spawnPos, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, -1);
            return false;
        }
    }
}