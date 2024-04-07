using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SolarEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Solar Enchantment");
            /* Tooltip.SetDefault(
@"Solar shield allows you to dash through enemies
Solar shield is not depleted on hit, but has reduced damage reduction
Attacks may inflict the Solar Flare debuff
'Too hot to handle'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "日耀魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"允许你使用日耀护盾进行冲刺
            // 日耀护盾在击中敌人时不会被消耗，但会降低其伤害减免效果
            // 攻击有几率造成耀斑减益
            // '烫手魔石'");
        }

        public override Color nameColor => new(254, 158, 35);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Purple;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SolarEffect>(Item);
            player.AddEffect<SolarFlareEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SolarFlareHelmet)
            .AddIngredient(ItemID.SolarFlareBreastplate)
            .AddIngredient(ItemID.SolarFlareLeggings)
            //solar wings
            //.AddIngredient(ItemID.HelFire)
            //golem fist
            //xmas tree sword
            .AddIngredient(ItemID.DayBreak)
            .AddIngredient(ItemID.SolarEruption)
            .AddIngredient(ItemID.StarWrath) //terrarian

            .AddTile(TileID.LunarCraftingStation)
            .Register();

        }
    }
    public class SolarEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<SolarEnchant>();
        public static void AddDash(Player player)
        {
            Player Player = player;
            if (!Player.setSolar && !player.FargoSouls().TerrariaSoul) //nerf DR
            {
                Player.endurance -= 0.15f;
            }

            Player.AddBuff(BuffID.SolarShield3, 5, false);

            Player.setSolar = true;
            int solarCD = 240;
            if (++Player.solarCounter >= solarCD)
            {
                if (Player.solarShields > 0 && Player.solarShields < 3)
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (Player.buffType[i] >= BuffID.SolarShield1 && Player.buffType[i] <= BuffID.SolarShield2)
                        {
                            Player.DelBuff(i);
                        }
                    }
                }
                if (Player.solarShields < 3)
                {
                    Player.AddBuff(BuffID.SolarShield1 + Player.solarShields, 5, false);
                    for (int i = 0; i < 16; i++)
                    {
                        Dust dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, 0f, 0f, 100)];
                        dust.noGravity = true;
                        dust.scale = 1.7f;
                        dust.fadeIn = 0.5f;
                        dust.velocity *= 5f;
                    }
                    Player.solarCounter = 0;
                }
                else
                {
                    Player.solarCounter = solarCD;
                }
            }
            for (int i = Player.solarShields; i < 3; i++)
            {
                Player.solarShieldPos[i] = Vector2.Zero;
            }
            for (int i = 0; i < Player.solarShields; i++)
            {
                Player.solarShieldPos[i] += Player.solarShieldVel[i];
                Vector2 value = (Player.miscCounter / 100f * 6.28318548f + i * (6.28318548f / Player.solarShields)).ToRotationVector2() * 6f;
                value.X = Player.direction * 20;
                Player.solarShieldVel[i] = (value - Player.solarShieldPos[i]) * 0.2f;
            }
            if (Player.dashDelay >= 0)
            {
                Player.solarDashing = false;
                Player.solarDashConsumedFlare = false;
            }
            bool flag = Player.solarDashing && Player.dashDelay < 0;
            if (Player.solarShields > 0 || flag)
            {
                Player.dashType = 3;
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                modPlayer.FargoDash = DashManager.DashType.None;
                modPlayer.HasDash = true; //doesnt check this itself, so overrides most other dashes(?)
            }
        }
    }
    public class SolarFlareEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<SolarEnchant>();
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), 300);
        }
    }
}
