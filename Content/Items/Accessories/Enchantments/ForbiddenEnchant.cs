using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ForbiddenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Forbidden Enchantment");

            // Tooltip.SetDefault(tooltip);
        }

        public override Color nameColor => new(231, 178, 28);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            if (player.AddEffect<ForbiddenEffect>(item))
                Lighting.AddLight(player.Center, 0.8f, 0.7f, 0.2f);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AncientBattleArmorHat)
            .AddIngredient(ItemID.AncientBattleArmorShirt)
            .AddIngredient(ItemID.AncientBattleArmorPants)
            //sun mask/moon mask
            .AddIngredient(ItemID.DjinnsCurse)
            .AddIngredient(ItemID.SpiritFlame)
            .AddIngredient(ItemID.SkyFracture)
            //sky fracture
            //.AddIngredient(ItemID.RainbowRod);

            //recipe.AddRecipeGroup("FargowiltasSouls:AnyScorpion");
            //fennec fox pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class ForbiddenEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
        public override int ToggleItemType => ModContent.ItemType<ForbiddenEnchant>();
        public static void ActivateForbiddenStorm(Player player)
        {
            if (player.HasEffect<ForbiddenEffect>())
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (modPlayer.CanSummonForbiddenStorm)
                {
                    CommandForbiddenStorm(player);
                    modPlayer.CanSummonForbiddenStorm = false;
                }
            }
        }
        public static void CommandForbiddenStorm(Player Player)
        {
            List<int> list = new();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<ForbiddenTornado>() && projectile.owner == Player.whoAmI)
                {
                    list.Add(i);
                }
            }

            Vector2 center = Player.Center;
            Vector2 mouse = Main.MouseWorld;

            bool flag3 = false;
            float[] array = new float[10];
            Vector2 v = mouse - center;
            Collision.LaserScan(center, v.SafeNormalize(Vector2.Zero), 60f, v.Length(), array);
            float num = 0f;
            for (int j = 0; j < array.Length; j++)
            {
                if (array[j] > num)
                {
                    num = array[j];
                }
            }
            float[] array2 = array;
            for (int k = 0; k < array2.Length; k++)
            {
                float num2 = array2[k];
                if (Math.Abs(num2 - v.Length()) < 10f)
                {
                    flag3 = true;
                    break;
                }
            }
            if (list.Count <= 1)
            {
                Vector2 vector = center + v.SafeNormalize(Vector2.Zero) * num;
                Vector2 value2 = vector - center;
                if (value2.Length() > 0f)
                {
                    for (float num3 = 0f; num3 < value2.Length(); num3 += 15f)
                    {
                        Vector2 position = center + value2 * (num3 / value2.Length());
                        Dust dust = Main.dust[Dust.NewDust(position, 0, 0, DustID.Sandnado, 0f, 0f, 0, default, 1f)];
                        dust.position = position;
                        dust.fadeIn = 0.5f;
                        dust.scale = 0.7f;
                        dust.velocity *= 0.4f;
                        dust.noLight = true;
                    }
                }
                for (float num4 = 0f; num4 < 6.28318548f; num4 += 0.209439516f)
                {
                    Dust dust2 = Main.dust[Dust.NewDust(vector, 0, 0, DustID.Sandnado, 0f, 0f, 0, default, 1f)];
                    dust2.position = vector;
                    dust2.fadeIn = 1f;
                    dust2.scale = 0.3f;
                    dust2.noLight = true;
                }
            }

            //Main.NewText(" " + (list.Count <= 1) + " " + flag3 + " " + Player.CheckMana(20, true, false));

            bool flag = list.Count <= 1;
            flag &= flag3;



            if (flag)
            {
                flag = Player.CheckMana(20, true, false);
                if (flag)
                {
                    Player.manaRegenDelay = (int)Player.maxRegenDelay;
                }

                //Player.wingTime = Player.wingTimeMax;
            }
            if (!flag)
            {
                return;
            }
            foreach (int current in list)
            {
                Projectile projectile2 = Main.projectile[current];
                if (projectile2.ai[0] < 780f)
                {
                    projectile2.ai[0] = 780f + projectile2.ai[0] % 60f;
                    projectile2.netUpdate = true;
                }
            }

            int damage = (int)(20f * (1f + Player.GetDamage(DamageClass.Magic).Additive + Player.GetDamage(DamageClass.Summon).Additive - 2f));
            Projectile.NewProjectile(Player.GetSource_EffectItem<ForbiddenEffect>(), mouse, Vector2.Zero, ModContent.ProjectileType<ForbiddenTornado>(), damage, 0f, Main.myPlayer, 0f, 0f);
        }
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f)
            {
                Color color12 = player.GetImmuneAlphaPure(Lighting.GetColor((int)(drawInfo.Position.X + player.width * 0.5) / 16, (int)(drawInfo.Position.Y + player.height * 0.5) / 16, Color.White), drawInfo.shadow);
                Color color21 = Color.Lerp(color12, value2: Color.White, 0.7f);

                Texture2D texture2D2 = TextureAssets.Extra[74].Value;
                Texture2D texture = TextureAssets.GlowMask[217].Value;
                bool flag8 = !player.setForbiddenCooldownLocked;
                int num52 = (int)((player.miscCounter / 300f * 6.28318548f).ToRotationVector2().Y * 6f);
                float num53 = (player.miscCounter / 75f * 6.28318548f).ToRotationVector2().X * 4f;
                Color color22 = new Color(80, 70, 40, 0) * (num53 / 8f + 0.5f) * 0.8f;
                if (!flag8)
                {
                    num52 = 0;
                    num53 = 2f;
                    color22 = new Color(80, 70, 40, 0) * 0.3f;
                    color21 = color21.MultiplyRGB(new Color(0.5f, 0.5f, 1f));
                }
                Vector2 vector4 = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                vector4 += new Vector2((float)(-(float)player.direction * 10), (float)(-20 + num52));
                DrawData value = new(texture2D2, vector4, null, color21, player.bodyRotation, texture2D2.Size() / 2f, 1f, drawInfo.playerEffect, 0);

                int num6 = 0;
                if (player.dye[1] != null)
                {
                    num6 = player.dye[1].dye;
                }
                value.shader = num6;
                drawInfo.DrawDataCache.Add(value);
                for (float num54 = 0f; num54 < 4f; num54 += 1f)
                {
                    value = new DrawData(texture, vector4 + (num54 * 1.57079637f).ToRotationVector2() * num53, null, color22, player.bodyRotation, texture2D2.Size() / 2f, 1f, drawInfo.playerEffect, 0);
                    drawInfo.DrawDataCache.Add(value);
                }
            }
        }
    }
}
