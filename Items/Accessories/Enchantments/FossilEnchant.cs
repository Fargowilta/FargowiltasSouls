using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class FossilEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Fossil Enchantment");
            Tooltip.SetDefault(
@"If you reach zero HP you will revive with 50 HP and spawn several bones
You will also spawn a few bones on every hit
Collect the bones to heal for 20 HP each
'Beyond a forgotten age'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "化石魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"受到致死伤害时会以1生命值重生并爆出几根骨头
            // 你攻击敌人时也会扔出骨头
            // 每根骨头会回复15点生命值
            // '被遗忘已久的记忆'");
        }

        protected override Color nameColor => new Color(140, 92, 59);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FossilEffect(player, Item);
        }

        public static void FossilEffect(Player player, Item item)
        {
            //bone zone
            player.GetModPlayer<FargoSoulsPlayer>().FossilEnchantItem = item;
        }

        public static void FossilHurt(FargoSoulsPlayer modPlayer, int damage)
        {
            Player player = modPlayer.Player;

            player.immune = true;
            player.immuneTime = 60;

            if (player.GetToggleValue("Fossil"))
            {
                //spawn bones
                int damageCopy = damage;
                for (int i = 0; i < 5; i++)
                {
                    if (damageCopy < 30)
                        break;
                    damageCopy -= 30;

                    float velX = Main.rand.Next(-5, 6) * 3f;
                    float velY = Main.rand.Next(-5, 6) * 3f;
                    Projectile.NewProjectile(player.GetSource_Misc(""), player.position.X + velX, player.position.Y + velY, velX, velY, ModContent.ProjectileType<FossilBone>(), 0, 0f, player.whoAmI);
                }
            }
        }

        public static void FossilRevive(FargoSoulsPlayer modPlayer)
        {
            Player player = modPlayer.Player;

            void Revive(int healAmount, int reviveCooldown)
            {
                player.statLife = healAmount;
                player.HealEffect(healAmount);

                player.immune = true;
                player.immuneTime = 120;
                player.hurtCooldowns[0] = 120;
                player.hurtCooldowns[1] = 120;

                string text = Language.GetTextValue($"Mods.{FargowiltasSouls.Instance.Name}.Message.Revived");
                CombatText.NewText(player.Hitbox, Color.SandyBrown, text, true);
                Main.NewText(text, Color.SandyBrown);

                player.AddBuff(ModContent.BuffType<FossilReviveCD>(), reviveCooldown);
            };

            if (modPlayer.Eternity)
            {
                Revive(player.statLifeMax2 / 2 > 200 ? player.statLifeMax2 / 2 : 200, 10800);
                FargoSoulsUtil.XWay(30, player.GetSource_Accessory(modPlayer.FossilEnchantItem), player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
            }
            else if (modPlayer.TerrariaSoul)
            {
                Revive(200, 14400);
                FargoSoulsUtil.XWay(25, player.GetSource_Accessory(modPlayer.FossilEnchantItem), player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
            }
            else
            {
                Revive(modPlayer.SpiritForce ? 200 : 50, 18000);
                FargoSoulsUtil.XWay(modPlayer.SpiritForce ? 20 : 10, player.GetSource_Accessory(modPlayer.FossilEnchantItem), player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FossilHelm)
                .AddIngredient(ItemID.FossilShirt)
                .AddIngredient(ItemID.FossilPants)
                .AddIngredient(ItemID.BoneDagger, 100)
                .AddIngredient(ItemID.AmberStaff)
                .AddIngredient(ItemID.AntlionClaw)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
