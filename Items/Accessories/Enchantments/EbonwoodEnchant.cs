using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class EbonwoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ebonwood Enchantment");
            Tooltip.SetDefault(
@"You are surrounded by an aura of Shadowflame
Any projectiles that would deal less than 10 damage to you are destroyed
'Untapped potential'");
            //in force damage theshold increased to 25 AND any npc that has less than 200 HP is instantly killed in the aura
        }

        protected override Color nameColor => new Color(100, 90, 141);
        public override string wizardEffect => "Damage threshold increased to 25, additonally kills any npcs with less than 200 HP";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            EbonwoodEffect(player);
        }

        public static void EbonwoodEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (!player.GetToggleValue("Ebon") || player.whoAmI != Main.myPlayer)
                return;

            int dist = modPlayer.WoodForce ? 400 : 200;
            int damageThreshold = modPlayer.WoodForce ? 25 : 10;
            float defenseFactor = 0.5f;

            if (Main.masterMode)
            {
                defenseFactor = 1f;
            }
            else if (Main.expertMode)
            {
                defenseFactor = 0.75f;
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active && proj.hostile && proj.damage > 0 && proj.Distance(player.Center) < dist)
                {
                    int dealtDamage = (int)(proj.damage - player.statDefense * defenseFactor);

                    if (dealtDamage <= damageThreshold)
                    {
                        proj.Kill();
                    }

                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage && npc.Distance(player.Center) < dist)
                {
                    if (modPlayer.WoodForce && npc.life < 200 && !npc.boss && !(npc.realLife > -1 && Main.npc[npc.realLife].active && Main.npc[npc.realLife].boss))
                        npc.StrikeNPC(npc.life, 0f, 0);

                    if (modPlayer.WoodForce || Collision.CanHitLine(player.Left, 0, 0, npc.Center, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, npc.Center, 0, 0))
                    {
                        npc.AddBuff(BuffID.ShadowFlame, 10);
                    }
                }
            }

            //dust
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
                        DustID.Shadowflame, 0, 0, 100, Color.White, 1f
                        )];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EbonwoodHelmet)
            .AddIngredient(ItemID.EbonwoodBreastplate)
            .AddIngredient(ItemID.EbonwoodGreaves)
            .AddIngredient(ItemID.VileMushroom)
            .AddIngredient(ItemID.BlackCurrant)
            .AddIngredient(ItemID.LightlessChasms)
            

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
