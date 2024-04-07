using FargowiltasSouls.Content.Buffs.Souls;
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
    public class ShadewoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Shadewood Enchantment");
            /* Tooltip.SetDefault(
@"You have an aura of Bleeding
Enemies struck while Bleeding spew damaging blood
'Surprisingly clean'"); */
        }

        public override Color nameColor => new(88, 104, 118);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ShadewoodEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadewoodHelmet)
                .AddIngredient(ItemID.ShadewoodBreastplate)
                .AddIngredient(ItemID.ShadewoodGreaves)
                .AddIngredient(ItemID.ViciousMushroom)
                .AddIngredient(ItemID.BloodOrange)
                .AddIngredient(ItemID.DeadlandComesAlive)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class ShadewoodEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShadewoodEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.whoAmI != Main.myPlayer)
                return;
            bool forceEffect = modPlayer.ForceEffect<ShadewoodEnchant>();
            int dist = forceEffect ? 400 : 200;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                {
                    Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npc, player.Center);
                    if (player.Distance(npcComparePoint) < dist && (forceEffect || Collision.CanHitLine(player.Center, 0, 0, npcComparePoint, 0, 0)))
                        npc.AddBuff(ModContent.BuffType<SuperBleedBuff>(), 120);
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * dist);
                offset.Y += (float)(Math.Cos(angle) * dist);
                Vector2 spawnPos = player.Center + offset - new Vector2(4, 4);
                if (forceEffect || Collision.CanHitLine(player.Left, 0, 0, spawnPos, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, spawnPos, 0, 0))
                {
                    Dust dust = Main.dust[Dust.NewDust(
                        spawnPos, 0, 0,
                        DustID.Blood, 0, 0, 100, Color.White, 1f
                        )];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }

            if (modPlayer.ShadewoodCD > 0)
            {
                modPlayer.ShadewoodCD--;
            }
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            ShadewoodProc(player, target, projectile);
        }
        public static void ShadewoodProc(Player player, NPC target, Projectile projectile)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect<ShadewoodEnchant>();
            int dmg = 12;

            if (forceEffect)
                dmg *= 3;

            if (target.HasBuff(ModContent.BuffType<SuperBleedBuff>()) && modPlayer.ShadewoodCD == 0 && (projectile == null || projectile.type != ModContent.ProjectileType<SuperBlood>()) && player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.NewProjectile(player.GetSource_Misc(""), target.Center.X, target.Center.Y - 20, 0f + Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), ModContent.ProjectileType<SuperBlood>(), FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 0f, Main.myPlayer);
                }

                if (forceEffect)
                {
                    target.AddBuff(BuffID.Ichor, 30);
                }
            }
        }
    }
}
