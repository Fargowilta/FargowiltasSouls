using FargowiltasSouls.Content.Projectiles.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasSouls.Core.ModPlayers;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class NecroEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Necro Enchantment");
            /* Tooltip.SetDefault(
@"Slain enemies may drop a pile of bones
Touch a pile of bones to spawn a friendly Dungeon Guardian
Damage scales with the defeated enemy's max HP
Bosses will drop bones every 10% of their HP lost
Effects of Bone Glove
'Welcome to the bone zone'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死灵魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"击杀敌人时有几率爆出一摞骨头
            // 拾取骨头时有几率生成一个地牢守卫
            // 地牢守卫的伤害取决于被击杀的敌人的最大生命值
            // Boss每损失10%生命值便会掉落骨头
            // '欢迎来到骸骨领域'");
        }

        public override Color nameColor => new(86, 86, 67);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<NecroEffect>(Item);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NecroHelmet)
                .AddIngredient(ItemID.NecroBreastplate)
                .AddIngredient(ItemID.NecroGreaves)
                .AddIngredient(ItemID.BoneWand)
                .AddIngredient(ItemID.BoneWhip)
                .AddIngredient(ItemID.StillLife)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class NecroEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<NecroEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.NecroCD != 0)
                modPlayer.NecroCD--;
        }
        public static void NecroSpawnGraveEnemy(NPC npc, Player player, FargoSoulsPlayer modPlayer)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NecroGrave>()] < 15)
            {
                int damage = npc.lifeMax / 3;
                if (damage > 0)
                    Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<NecroEffect>()), npc.Bottom, new Vector2(0, -4), ModContent.ProjectileType<NecroGrave>(), 0, 0, player.whoAmI, damage);

                //if (modPlayer.ShadowForce || modPlayer.WizardEnchantActive)
                //{
                //    modPlayer.NecroCD = 15;
                //}
                //else
                //{
                //    modPlayer.NecroCD = 30;
                //}
            }
        }
        public static void NecroSpawnGraveBoss(FargoSoulsGlobalNPC globalNPC, NPC npc, Player player, int damage)
        {
            globalNPC.NecroDamage += damage;

            if (globalNPC.NecroDamage > npc.lifeMax / 10 && player.ownedProjectileCounts[ModContent.ProjectileType<NecroGrave>()] < 45)
            {
                globalNPC.NecroDamage = 0;

                int dam = npc.lifeMax / 25;
                if (dam > 0)
                    Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<NecroEffect>()), npc.Bottom, new Vector2(0, -4), ModContent.ProjectileType<NecroGrave>(), 0, 0, player.whoAmI, dam);
            }
        }
    }
}
