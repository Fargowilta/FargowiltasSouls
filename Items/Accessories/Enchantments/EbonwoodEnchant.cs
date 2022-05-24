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
@"You have an aura of Shadowflame
'Untapped potential'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "乌木魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"一圈暗影焰光环环绕着你
            // '未开发的潜力'");
        }

        protected override Color nameColor => new Color(100, 90, 141);

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

            int dist = modPlayer.WoodForce ? 350 : 250;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && npc.Distance(player.Center) < dist && (modPlayer.WoodForce || Collision.CanHitLine(player.Left, 0, 0, npc.Center, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, npc.Center, 0, 0)))
                {
                    npc.AddBuff(BuffID.ShadowFlame, 15);

                    if (modPlayer.WoodForce)
                    {
                        npc.AddBuff(BuffID.CursedInferno, 15);
                    }
                }

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
            .AddIngredient(ItemID.EbonwoodSword)
            .AddIngredient(ItemID.VileMushroom)
            .AddIngredient(ItemID.BlackCurrant)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
