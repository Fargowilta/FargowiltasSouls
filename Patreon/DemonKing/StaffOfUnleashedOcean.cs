using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DemonKing
{
    public class StaffOfUnleashedOcean : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Staff of Unleashed Ocean");
            Tooltip.SetDefault("Summons Duke Fishron to fight for you\nNeeds 2 minion slots\n'Now channel your rage against them!'");
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 2;
        }

        public override void SetDefaults()
        {
            Item.damage = 328;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Zombie20;
            Item.shoot = ModContent.ProjectileType<DukeFishronMinion>();
            Item.shootSpeed = 10f;
            Item.buffType = ModContent.BuffType<DukeFishronBuff>();
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 25);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, default, velocity);
            return false;
        }
    }
}