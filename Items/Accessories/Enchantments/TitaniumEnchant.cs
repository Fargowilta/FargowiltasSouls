using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Projectiles;
using System;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Titanium Enchantment");
            Tooltip.SetDefault(
@"Attacking generates a defensive barrier of up to 20 titanium shards
When you reach the maximum amount, you gain resistance to most debuffs on hit and 50% damage resistance against contact damage and projectiles in close range
This has a cooldown of 10 seconds during which you cannot gain shards
'The power of titanium in the palm of your hand'");
        }

        protected override Color nameColor => new Color(130, 140, 136);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Titanium");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TitaniumEffect(player, Item);
        }

        public static void TitaniumEffect(Player player, Item item)
        {
            if (player.GetToggleValue("Titanium"))
            {
                player.GetModPlayer<FargoSoulsPlayer>().TitaniumEnchantItem = item;
            }
        }

        public static void TryTitaniumDR(FargoSoulsPlayer modPlayer, Entity attacker)
        {
            if (!modPlayer.TitaniumDRBuff)
                return;

            Player player = modPlayer.Player;

            bool canUseDR = attacker is NPC ||
                (attacker is Projectile projectile && projectile.GetSourceNPC() is NPC sourceNPC 
                && player.Distance(sourceNPC.Center) < Math.Max(sourceNPC.width, sourceNPC.height) + 16 * 8);

            if (canUseDR)
            {
                float diff = 1f - player.endurance;
                diff *= modPlayer.EarthForce ? 0.75f : 0.5f;
                player.endurance += diff;
            }
        }

        public static void TitaniumShards(FargoSoulsPlayer modPlayer, Player player)
        {
			if (modPlayer.TitaniumCD)
				return;
			
            player.AddBuff(306, 600, true, false);
            if (player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] < 20)
            {
                int damage = 50;
                if (modPlayer.EarthForce)
                {
                    damage = FargoSoulsUtil.HighestDamageTypeScaling(player, damage);
                }

                Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.TitaniumEnchantItem), player.Center, Vector2.Zero, ProjectileID.TitaniumStormShard /*ModContent.ProjectileType<TitaniumShard>()*/, damage, 15f, player.whoAmI, 0f, 0f);
            }
            else
            {
				if (!player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()))
				{
					//dust ring
					for (int j = 0; j < 20; j++)
					{
						Vector2 vector6 = Vector2.UnitY * 5f;
						vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
						Vector2 vector7 = vector6 - player.Center;
						int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Titanium);
						Main.dust[d].noGravity = true;
						Main.dust[d].scale = 1.5f;
						Main.dust[d].velocity = vector7;
					}
				}

				int buffDuration = 240;
				player.AddBuff(ModContent.BuffType<TitaniumDRBuff>(), buffDuration);
			}
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
                .AddIngredient(ItemID.TitaniumBreastplate)
                .AddIngredient(ItemID.TitaniumLeggings)
                .AddIngredient(ItemID.Chik)
                .AddIngredient(ItemID.CrystalStorm)
                .AddIngredient(ItemID.CrystalVileShard)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
