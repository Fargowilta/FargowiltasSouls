using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NecroEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Necro Enchantment");
            Tooltip.SetDefault(
@"Slain enemies may drop a pile of bones
Touch a pile of bones to spawn a friendly Dungeon Guardian
Damage scales with the defeated enemy's max HP
Bosses will drop bones every 10% of their HP lost
Effects of Bone Glove
'Welcome to the bone zone'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死灵魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"击杀敌人时有几率爆出一摞骨头
            // 拾取骨头时有几率生成一个地牢守卫
            // 地牢守卫的伤害取决于被击杀的敌人的最大生命值
            // Boss每损失10%生命值便会掉落骨头
            // '欢迎来到骸骨领域'");
        }

        protected override Color nameColor => new Color(86, 86, 67);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Necro");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            NecroEffect(player, this.Item);
        }

        public static void NecroEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            modPlayer.NecroEnchantActive = true;

            //whack way of doing it but ok vanilla
            if (player.GetToggleValue("NecroGlove"))
                player.boneGloveItem = item;

            if (modPlayer.NecroCD != 0)
                modPlayer.NecroCD--;
        }

        public static void NecroSpawnGraveEnemy(NPC npc, Player player, FargoSoulsPlayer modPlayer)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NecroGrave>()] < 15)
            {
                int damage = npc.lifeMax / 3;
                if (damage > 0)
                    Projectile.NewProjectile(player.GetSource_Misc(""), npc.Bottom, new Vector2(0, -4), ModContent.ProjectileType<NecroGrave>(), 0, 0, player.whoAmI, damage);

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
                    Projectile.NewProjectile(player.GetSource_Misc(""), npc.Bottom, new Vector2(0, -4), ModContent.ProjectileType<NecroGrave>(), 0, 0, player.whoAmI, dam);
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NecroHelmet)
                .AddIngredient(ItemID.NecroBreastplate)
                .AddIngredient(ItemID.NecroGreaves)
                .AddIngredient(ItemID.BoneGlove)
                .AddIngredient(ItemID.BoneWhip)
                .AddIngredient(ItemID.StillLife)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
