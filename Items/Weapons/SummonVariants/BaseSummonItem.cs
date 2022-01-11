using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SummonVariants
{
    public abstract class BaseSummonItem : SoulsItem
    {
        public abstract int Type
        {
            get;
        }

        public override string Texture => $"Terraria/Item_{Type}";

        public override void SetDefaults()
        {
            item.CloneDefaults(Type);
            item.melee = false;
            item.ranged = false;
            item.magic = false;
            item.summon = true;

            //item.mana = 0;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int num = Item.NewItem(player.getRect(), Type, prefixGiven: item.prefix);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, number: num, number2: 1f);
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].minion = true;
            Main.projectile[proj].melee = false;
            Main.projectile[proj].ranged = false;
            Main.projectile[proj].magic = false;
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "help", Language.GetTextValue("Mods.FargowiltasSouls.SummonVariants.Convert"));
            tooltips.Add(line);
        }
    }
}