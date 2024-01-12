using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;
using System;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class BeetleEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Beetle Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "甲虫魔石");

            string tooltip =
@"Beetles increase your damage and melee speed
When hit, beetles instead protect you from damage for 10 seconds
Beetle buffs capped at level two
'The unseen life of dung courses through your veins'";
            // Tooltip.SetDefault(tooltip);

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "甲虫魔石");

            //             string tooltip =
            // @"Beetles protect you from damage, up to 15% damage reduction only
            // Increases flight time by 25%
            // 'The unseen life of dung courses through your veins'";
            //             Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"甲虫会保护你，减免下次受到的伤害，至多减免15%下次受到的伤害
            // 延长25%飞行时间
            // '你的血管里流淌着看不见的粪便生命'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new(109, 92, 133);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<BeetleEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHelmet)
            .AddRecipeGroup("FargowiltasSouls:AnyBeetle")
            .AddIngredient(ItemID.BeetleLeggings)
            .AddIngredient(ItemID.BeetleWings)
            .AddIngredient(ItemID.BeeWings)
            .AddIngredient(ItemID.ButterflyWings)
            //.AddIngredient(ItemID.MothronWings);
            //breaker blade
            //amarok
            //beetle minecart

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class BeetleEffect : AccessoryEffect
    {
        public override bool HasToggle => false;
        public override Header ToggleHeader => null;

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.beetleOffense && damageClass != DamageClass.Melee)
            {
                player.beetleCounter += hitInfo.Damage;
            }
        }

        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            BeetleFields fields = player.GetEffectFields<BeetleFields>();
            fields.BeetleEnchantDefenseTimer = 600;

            //AFTER THIS DAMAGE, transfer all offense beetles to endurance instead
            //doesnt really work rn, only trasnfers 1 beetle, but tbh its more balanced this way
            if (player.whoAmI == Main.myPlayer)
            {
                int strongestBuff = -1;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (player.buffTime[i] > 0)
                    {
                        if (player.buffType[i] == BuffID.BeetleMight1 || player.buffType[i] == BuffID.BeetleMight2 || player.buffType[i] == BuffID.BeetleMight3)
                        {
                            if (strongestBuff < player.buffType[i])
                                strongestBuff = player.buffType[i];

                            player.DelBuff(i);
                            i -= 1;
                        }
                    }
                }

                if (strongestBuff != -1)
                {
                    int offset = BuffID.BeetleMight1 - strongestBuff;
                    player.AddBuff(BuffID.BeetleEndurance1 + offset, 5, false);
                }
            }
        }

        public override void PostUpdateEquips(Player player)
        {
            //don't let this stack
            if (player.beetleDefense || player.beetleOffense)
                return;

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            BeetleFields fields = player.GetEffectFields<BeetleFields>();

            if (fields.BeetleEnchantDefenseTimer > 0) //do defensive beetle things
            {
                player.beetleDefense = true;
                player.beetleCounter += 1f;
                int cap = modPlayer.TerrariaSoul ? 3 : 2;
                int time = 180 * 3 / cap;
                if (player.beetleCounter >= time)
                {
                    if (player.beetleOrbs > 0 && player.beetleOrbs < cap)
                    {
                        for (int k = 0; k < Player.MaxBuffs; k++)
                        {
                            if (player.buffType[k] >= 95 && player.buffType[k] <= 96)
                            {
                                player.DelBuff(k);
                            }
                        }
                    }
                    if (player.beetleOrbs < cap)
                    {
                        player.AddBuff(BuffID.BeetleEndurance1 + player.beetleOrbs, 5, false);
                        player.beetleCounter = 0f;
                    }
                    else
                    {
                        player.beetleCounter = time;
                    }
                }
            }
            else
            {
                player.beetleOffense = true;

                player.beetleCounter -= 3f;
                player.beetleCounter = player.beetleCounter - player.beetleCountdown / 10f;

                player.beetleCountdown += 1;

                if (player.beetleCounter < 0)
                    player.beetleCounter = 0;

                int beetles = 0;
                int num2 = 400;
                int num3 = 1200;
                int num4 = 4600;

                if (player.beetleCounter > num2 + num3 + num4 + num3)
                    player.beetleCounter = num2 + num3 + num4 + num3;
                if (modPlayer.TerrariaSoul && player.beetleCounter > num2 + num3 + num4)
                {
                    player.AddBuff(BuffID.BeetleMight3, 5, false);
                    beetles = 3;
                }
                else
                {
                    player.buffImmune[BuffID.BeetleMight3] = true;

                    if (player.beetleCounter > num2 + num3)
                    {
                        player.AddBuff(BuffID.BeetleMight2, 5, false);
                        beetles = 2;
                    }
                    else if (player.beetleCounter > num2)
                    {
                        player.AddBuff(BuffID.BeetleMight1, 5, false);
                        beetles = 1;
                    }
                }

                if (beetles < player.beetleOrbs)
                    player.beetleCountdown = 0;
                else if (beetles > player.beetleOrbs)
                    player.beetleCounter = player.beetleCounter + 200f;

                float damage = beetles * 0.10f;
                player.GetDamage(DamageClass.Generic) += damage;
                player.GetDamage(DamageClass.Melee) -= damage; //offset the actual vanilla beetle buff

                if (beetles != player.beetleOrbs && player.beetleOrbs > 0)
                {
                    for (int b = 0; b < Player.MaxBuffs; ++b)
                    {
                        if (player.buffType[b] >= 98 && player.buffType[b] <= 100 && player.buffType[b] != 97 + beetles)
                            player.DelBuff(b);
                    }
                }
            }

            //vanilla code for beetle visuals
            if (!player.beetleDefense && !player.beetleOffense)
            {
                player.beetleCounter = 0f;
            }
            else
            {
                player.beetleFrameCounter++;
                if (player.beetleFrameCounter >= 1)
                {
                    player.beetleFrameCounter = 0;
                    player.beetleFrame++;
                    if (player.beetleFrame > 2)
                    {
                        player.beetleFrame = 0;
                    }
                }
                for (int l = player.beetleOrbs; l < 3; l++)
                {
                    player.beetlePos[l].X = 0f;
                    player.beetlePos[l].Y = 0f;
                }
                for (int m = 0; m < player.beetleOrbs; m++)
                {
                    player.beetlePos[m] += player.beetleVel[m];
                    Vector2[] expr_6EcCp0 = player.beetleVel;
                    int expr_6EcCp1 = m;
                    expr_6EcCp0[expr_6EcCp1].X = expr_6EcCp0[expr_6EcCp1].X + Main.rand.Next(-100, 101) * 0.005f;
                    Vector2[] expr71ACp0 = player.beetleVel;
                    int expr71ACp1 = m;
                    expr71ACp0[expr71ACp1].Y = expr71ACp0[expr71ACp1].Y + Main.rand.Next(-100, 101) * 0.005f;
                    float num6 = player.beetlePos[m].X;
                    float num7 = player.beetlePos[m].Y;
                    float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 100f)
                    {
                        num8 = 20f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num9 = 10;
                        player.beetleVel[m].X = (player.beetleVel[m].X * (num9 - 1) + num6) / num9;
                        player.beetleVel[m].Y = (player.beetleVel[m].Y * (num9 - 1) + num7) / num9;
                    }
                    else if (num8 > 30f)
                    {
                        num8 = 10f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num10 = 20;
                        player.beetleVel[m].X = (player.beetleVel[m].X * (num10 - 1) + num6) / num10;
                        player.beetleVel[m].Y = (player.beetleVel[m].Y * (num10 - 1) + num7) / num10;
                    }
                    num6 = player.beetleVel[m].X;
                    num7 = player.beetleVel[m].Y;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 2f)
                    {
                        player.beetleVel[m] *= 0.9f;
                    }
                    player.beetlePos[m] -= player.velocity * 0.25f;
                }
            }

            if (fields.BeetleEnchantDefenseTimer > 0)
                fields.BeetleEnchantDefenseTimer--;
        }
    }

    public class BeetleFields : EffectFields
    {
        public int BeetleEnchantDefenseTimer;

        public override void UpdateDead()
        {
            BeetleEnchantDefenseTimer = 0;
        }
    }
}
