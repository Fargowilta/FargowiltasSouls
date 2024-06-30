using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class SpiritForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<FossilEnchant>(),
                ModContent.ItemType<ForbiddenEnchant>(),
                ModContent.ItemType<HallowEnchant>(),
                ModContent.ItemType<AncientHallowEnchant>(),
                ModContent.ItemType<TikiEnchant>(),
                ModContent.ItemType<SpectreEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            player.AddEffect<SpiritTornadoEffect>(Item);
            player.AddEffect<FossilEffect>(Item);
            player.AddEffect<HallowEffect>(Item);
            TikiEnchant.AddEffects(player, Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile<CrucibleCosmosSheet>();
            recipe.Register();
        }
    }
    public class SpiritTornadoEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
        public override int ToggleItemType => ModContent.ItemType<SpiritForce>();
        
        public static void ActivateSpiritStorm(Player player)
        {
            if (player.HasEffect<SpiritTornadoEffect>())
            {
                CommandSpiritStorm(player);
            }
        }
        public static void CommandSpiritStorm(Player Player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<SpiritTornado>() && projectile.owner == Player.whoAmI)
                {
                    projectile.Kill();
                }
            }

            int damage = (int)(125f * (1f + Player.GetDamage(DamageClass.Magic).Additive + Player.GetDamage(DamageClass.Summon).Additive - 2f));
            Projectile.NewProjectile(Player.GetSource_EffectItem<SpiritTornadoEffect>(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SpiritTornado>(), damage, 0f, Main.myPlayer, 0f, 0f);
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
