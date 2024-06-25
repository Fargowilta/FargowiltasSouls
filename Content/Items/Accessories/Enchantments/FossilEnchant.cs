using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class FossilEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Fossil Enchantment");
            /* Tooltip.SetDefault(
@"If you reach zero HP you will revive with 50 HP and spawn several bones
You will also spawn a few bones on every hit
Collect the bones to heal for 20 HP each
'Beyond a forgotten age'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "化石魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"受到致死伤害时会以1生命值重生并爆出几根骨头
            // 你攻击敌人时也会扔出骨头
            // 每根骨头会回复15点生命值
            // '被遗忘已久的记忆'");
        }

        public override Color nameColor => new(140, 92, 59);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FossilEffect>(Item);
            player.AddEffect<FossilBones>(Item);
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
    public class FossilEffect : AccessoryEffect
    {

        public override Header ToggleHeader => null;
        public override bool IgnoresMutantPresence => true;
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            player.immune = true;
            player.immuneTime = Math.Max(player.immuneTime, 60);
        }
        public static void FossilRevive(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool spiritForce = modPlayer.ForceEffects.Contains(ModContent.ItemType<SpiritForce>());

            void Revive(int healAmount, int reviveCooldown)
            {
                player.statLife = healAmount;
                player.HealEffect(healAmount);

                player.immune = true;
                player.immuneTime = 120;
                player.hurtCooldowns[0] = 120;
                player.hurtCooldowns[1] = 120;

                int max = player.buffType.Length;
                for (int i = 0; i < max; i++)
                {
                    int timeLeft = player.buffTime[i];
                    if (timeLeft <= 0)
                        continue;

                    int buffType = player.buffType[i];
                    if (buffType <= 0)
                        continue;

                    if (timeLeft > 5
                        && Main.debuff[buffType]
                        && !Main.buffNoTimeDisplay[buffType]
                        && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                    {
                        player.DelBuff(i);

                        i--;
                        max--; //just in case, to prevent being stuck here forever
                    }
                }

                string text = Language.GetTextValue($"Mods.{FargowiltasSouls.Instance.Name}.Message.Revived");
                CombatText.NewText(player.Hitbox, Color.SandyBrown, text, true);
                Main.NewText(text, Color.SandyBrown);

                player.AddBuff(ModContent.BuffType<FossilReviveCDBuff>(), reviveCooldown);
            };

            if (modPlayer.Eternity)
            {
                Revive(player.statLifeMax2 / 2 > 300 ? player.statLifeMax2 / 2 : 300, 10800);
                //FargoSoulsUtil.XWay(30, player.GetSource_Misc("FossilEnchant"), player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
            }
            else if (modPlayer.TerrariaSoul)
            {
                Revive(300, 14400);
                //FargoSoulsUtil.XWay(25, player.GetSource_Misc("FossilEnchant"), player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
            }
            else
            {
                bool forceEffect = modPlayer.ForceEffect<FossilEnchant>();
                Revive(forceEffect ? 200 : 50, 18000);
                FargoSoulsUtil.XWay(forceEffect ? 20 : 10, player.GetSource_Misc("FossilEnchant"), player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
            }
        }
    }
    public class FossilBones : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
        public override int ToggleItemType => ModContent.ItemType<FossilEnchant>();
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            //spawn bones
            int damageCopy = info.Damage;
            for (int i = 0; i < 5; i++)
            {
                if (damageCopy < 30)
                    break;
                damageCopy -= 30;

                float velX = Main.rand.Next(-5, 6) * 3f;
                float velY = Main.rand.Next(-5, 6) * 3f;
                Projectile.NewProjectile(GetSource_EffectItem(player), player.position.X + velX, player.position.Y + velY, velX, velY, ModContent.ProjectileType<FossilBone>(), 0, 0f, player.whoAmI);
            }
        }
    }
}
