using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShadewoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shadewood Enchantment");
            Tooltip.SetDefault(
@"You have an aura of Bleeding
Enemies struck while Bleeding spew damaging blood
'Surprisingly clean'");
        }

        protected override Color nameColor => new Color(88, 104, 118);
        public override string wizardEffect => "Aura size and blood damage increased, additionally applies Ichor";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ShadewoodEffect(player, Item);
        }

        public static void ShadewoodEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (!player.GetToggleValue("Shade") || player.whoAmI != Main.myPlayer)
                return;

            modPlayer.ShadewoodEnchantItem = item;

            int dist = modPlayer.WoodForce ? 400 : 200;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && npc.Distance(player.Center) < dist && (modPlayer.WoodForce || Collision.CanHitLine(player.Left, 0, 0, npc.Center, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, npc.Center, 0, 0)))
                    npc.AddBuff(ModContent.BuffType<SuperBleed>(), 120);

                npc.netUpdate = true;
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * dist);
                offset.Y += (float)(Math.Cos(angle) * dist);
                Vector2 spawnPos = player.Center + offset - new Vector2(4, 4);
                if (modPlayer.WoodForce || Collision.CanHitLine(player.Left, 0, 0, spawnPos, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, spawnPos, 0, 0))
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

        public static void ShadewoodProc(FargoSoulsPlayer modPlayer, NPC target, Projectile projectile)
        {
            Player player = modPlayer.Player;
            bool trueMelee = (projectile == null || projectile.aiStyle == 19);
            int dmg = 20;

            if (modPlayer.WoodForce)
            {
                dmg *= 3;
            }

            if (target.HasBuff(ModContent.BuffType<SuperBleed>()) && (trueMelee || modPlayer.ShadewoodCD == 0) && (projectile == null || projectile.type != ModContent.ProjectileType<SuperBlood>()) && player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.NewProjectile(player.GetSource_Misc(""), target.Center.X, target.Center.Y - 20, 0f + Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), ModContent.ProjectileType<SuperBlood>(), FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 0f, Main.myPlayer);
                }

                if (modPlayer.WoodForce)
                {
                    target.AddBuff(BuffID.Ichor, 30);
                }

                //true melee attack does not go on cooldown
                if (!trueMelee)
                {
                    modPlayer.ShadewoodCD = 30;
                }
            }
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
}
