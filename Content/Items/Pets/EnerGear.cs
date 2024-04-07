using FargowiltasSouls.Content.Buffs.Pets;
using FargowiltasSouls.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Pets
{
    public class EnerGear : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ener-Gear");
            // Tooltip.SetDefault("Summons a bite-size baron\n'100% rust-proof or your money back'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DukeFishronPetItem);
            Item.shoot = ModContent.ProjectileType<BiteSizeBaron>();
            Item.buffType = ModContent.BuffType<BiteSizeBaronBuff>();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            base.UseStyle(player, heldItemFrame);

            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}