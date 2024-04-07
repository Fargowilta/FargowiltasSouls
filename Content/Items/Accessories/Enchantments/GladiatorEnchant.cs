using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class GladiatorEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Gladiator Enchantment");
            /* Tooltip.SetDefault(
@"Spears will rain down on struck enemies
Grants knockback immunity when you are facing the attack
'Are you not entertained?'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "角斗士魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"长矛将倾泄在被攻击的敌人身上
            // '难道你不高兴吗？'");
        }

        public override Color nameColor => new(156, 146, 78);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<GladiatorBanner>(Item);
            player.AddEffect<GladiatorSpears>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GladiatorHelmet)
                .AddIngredient(ItemID.GladiatorBreastplate)
                .AddIngredient(ItemID.GladiatorLeggings)
                .AddIngredient(ItemID.Spear)
                .AddIngredient(ItemID.Gladius)
                .AddIngredient(ItemID.BoneJavelin, 300)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class GladiatorBanner : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<GladiatorEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.GladiatorCD > 0)
            {
                modPlayer.GladiatorCD--;
            }
        }

        public static void ActivateGladiatorBanner(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.whoAmI == Main.myPlayer && player.HasEffect<GladiatorBanner>())
            {
                int GladiatorStandard = ModContent.ProjectileType<GladiatorStandard>();
                if (player.ownedProjectileCounts[GladiatorStandard] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_Misc(""), player.Top, Vector2.UnitY * 25, GladiatorStandard, modPlayer.ForceEffect<GladiatorEnchant>() ? 300 : 100, 3f, player.whoAmI);
                }
            }
        }
    }
    public class GladiatorSpears : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<GladiatorEnchant>();

        public override bool ExtraAttackEffect => true;
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.whoAmI == Main.myPlayer && modPlayer.GladiatorCD <= 0 && (projectile == null || projectile.type != ModContent.ProjectileType<GladiatorJavelin>()))
            {
                bool buff = player.HasBuff<GladiatorBuff>();
                int spearDamage = baseDamage / (buff ? 3 : 5);

                if (spearDamage > 0)
                {
                    if (!modPlayer.TerrariaSoul)
                        spearDamage = Math.Min(spearDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, 300));
                    Item effectItem = EffectItem(player);
                    for (int i = 0; i < 3; i++)
                    {
                        const int arrivalTime = 15;
                        Vector2 spawn = new(target.Center.X + Main.rand.NextFloat(-300, 300), target.Center.Y - Main.rand.Next(600, 801));
                        Vector2 aim = target.Center + (target.velocity * arrivalTime * Main.rand.NextFloat(0.7f, 1.3f));
                        float speed = (aim - spawn).Length() / arrivalTime * Main.rand.NextFloat(0.8f, 1.2f);
                        //Vector2 speed = target.Center + target.velocity * i * 5 * Main.rand.NextFloat(0.5f, 1.5f) - spawn;
                        //speed.Normalize();
                        //speed *= 15f * Main.rand.NextFloat(0.8f, 1.2f);

                        Projectile.NewProjectile(player.GetSource_Accessory(effectItem), spawn, Vector2.Normalize(aim - spawn).RotatedByRandom(MathHelper.Pi / 20) * speed, ModContent.ProjectileType<GladiatorJavelin>(), spearDamage, 4f, Main.myPlayer);
                    }

                    modPlayer.GladiatorCD = modPlayer.ForceEffect<GladiatorEnchant>() ? 10 : 30;
                    modPlayer.GladiatorCD = buff ? modPlayer.GladiatorCD : (int)Math.Round(modPlayer.GladiatorCD * 1.5f);
                }
            }
        }
    }
}
