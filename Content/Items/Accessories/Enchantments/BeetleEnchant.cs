using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;
using System;
using FargowiltasSouls.Content.Items.Accessories.Souls;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class BeetleEnchant : BaseEnchant
    {

        public override Color nameColor => new(109, 92, 133);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            //don't let this stack

            if (player.beetleDefense || player.beetleOffense)
                return;

            player.AddEffect<BeetleEffect>(item);
            if (!player.HasEffect<BeetleEffect>())
                return;

            Player Player = player;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.BeetleEnchantDefenseTimer > 0) //do defensive beetle things
            {
                Player.beetleDefense = true;
                Player.beetleCounter += 1f;
                int cap = modPlayer.TerrariaSoul ? 3 : 2;
                int time = 180 * 3 / cap;
                if (Player.beetleCounter >= time)
                {
                    if (Player.beetleOrbs > 0 && Player.beetleOrbs < cap)
                    {
                        for (int k = 0; k < Player.MaxBuffs; k++)
                        {
                            if (Player.buffType[k] >= 95 && Player.buffType[k] <= 96)
                            {
                                Player.DelBuff(k);
                            }
                        }
                    }
                    if (Player.beetleOrbs < cap)
                    {
                        Player.AddBuff(BuffID.BeetleEndurance1 + Player.beetleOrbs, 5, false);
                        Player.beetleCounter = 0f;
                    }
                    else
                    {
                        Player.beetleCounter = time;
                    }
                }
            }
            else
            {
                Player.beetleOffense = true;

                Player.beetleCounter -= 3f;
                Player.beetleCounter = Player.beetleCounter - Player.beetleCountdown / 10f;

                Player.beetleCountdown += 1;

                if (Player.beetleCounter < 0)
                    Player.beetleCounter = 0;

                int beetles = 0;
                int num2 = 400;
                int num3 = 1200;
                int num4 = 4600;

                if (Player.beetleCounter > num2 + num3 + num4 + num3)
                    Player.beetleCounter = num2 + num3 + num4 + num3;
                if (modPlayer.TerrariaSoul && Player.beetleCounter > num2 + num3 + num4)
                {
                    Player.AddBuff(BuffID.BeetleMight3, 5, false);
                    beetles = 3;
                }
                else
                {
                    Player.buffImmune[BuffID.BeetleMight3] = true;

                    if (Player.beetleCounter > num2 + num3)
                    {
                        Player.AddBuff(BuffID.BeetleMight2, 5, false);
                        beetles = 2;
                    }
                    else if (Player.beetleCounter > num2)
                    {
                        Player.AddBuff(BuffID.BeetleMight1, 5, false);
                        beetles = 1;
                    }
                }

                if (beetles < Player.beetleOrbs)
                    Player.beetleCountdown = 0;
                else if (beetles > Player.beetleOrbs)
                    Player.beetleCounter = Player.beetleCounter + 200f;

                float damage = beetles * 0.10f;
                Player.GetDamage(DamageClass.Generic) += damage;
                Player.GetDamage(DamageClass.Melee) -= damage; //offset the actual vanilla beetle buff

                if (beetles != Player.beetleOrbs && Player.beetleOrbs > 0)
                {
                    for (int b = 0; b < Player.MaxBuffs; ++b)
                    {
                        if (Player.buffType[b] >= 98 && Player.buffType[b] <= 100 && Player.buffType[b] != 97 + beetles)
                            Player.DelBuff(b);
                    }
                }
            }

            //vanilla code for beetle visuals
            if (!Player.beetleDefense && !Player.beetleOffense)
            {
                Player.beetleCounter = 0f;
            }
            else
            {
                Player.beetleFrameCounter++;
                if (Player.beetleFrameCounter >= 1)
                {
                    Player.beetleFrameCounter = 0;
                    Player.beetleFrame++;
                    if (Player.beetleFrame > 2)
                    {
                        Player.beetleFrame = 0;
                    }
                }
                for (int l = Player.beetleOrbs; l < 3; l++)
                {
                    Player.beetlePos[l].X = 0f;
                    Player.beetlePos[l].Y = 0f;
                }
                for (int m = 0; m < Player.beetleOrbs; m++)
                {
                    Player.beetlePos[m] += Player.beetleVel[m];
                    Vector2[] expr_6EcCp0 = Player.beetleVel;
                    int expr_6EcCp1 = m;
                    expr_6EcCp0[expr_6EcCp1].X = expr_6EcCp0[expr_6EcCp1].X + Main.rand.Next(-100, 101) * 0.005f;
                    Vector2[] expr71ACp0 = Player.beetleVel;
                    int expr71ACp1 = m;
                    expr71ACp0[expr71ACp1].Y = expr71ACp0[expr71ACp1].Y + Main.rand.Next(-100, 101) * 0.005f;
                    float num6 = Player.beetlePos[m].X;
                    float num7 = Player.beetlePos[m].Y;
                    float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 100f)
                    {
                        num8 = 20f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num9 = 10;
                        Player.beetleVel[m].X = (Player.beetleVel[m].X * (num9 - 1) + num6) / num9;
                        Player.beetleVel[m].Y = (Player.beetleVel[m].Y * (num9 - 1) + num7) / num9;
                    }
                    else if (num8 > 30f)
                    {
                        num8 = 10f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num10 = 20;
                        Player.beetleVel[m].X = (Player.beetleVel[m].X * (num10 - 1) + num6) / num10;
                        Player.beetleVel[m].Y = (Player.beetleVel[m].Y * (num10 - 1) + num7) / num10;
                    }
                    num6 = Player.beetleVel[m].X;
                    num7 = Player.beetleVel[m].Y;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 2f)
                    {
                        Player.beetleVel[m] *= 0.9f;
                    }
                    Player.beetlePos[m] -= Player.velocity * 0.25f;
                }
            }
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
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<BeetleEnchant>();

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.beetleOffense && damageClass != DamageClass.Melee)
            {
                player.beetleCounter += hitInfo.Damage;
            }
        }

        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.BeetleEnchantDefenseTimer = 600;

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

    }

}
