using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Projectiles.JungleMimic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Misc
{
    public class OvergrownKey : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Overgrown Key");
            // Tooltip.SetDefault("Summons a Jungle Mimic to fight for you\nNeeds 2 minion slots");
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 2;
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 47;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 14f;
            Item.width = 36;
            Item.height = 16;
            Item.UseSound = SoundID.Item77;
            Item.useAnimation = 37;
            Item.useTime = 37;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 8);
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.LightRed;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<JungleMimicSummon>();
            Item.buffType = ModContent.BuffType<JungleMimicSummonBuff>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }
    }
}