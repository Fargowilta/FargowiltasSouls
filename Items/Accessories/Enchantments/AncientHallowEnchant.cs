using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientHallowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ancient Hallowed Enchantment");
            Tooltip.SetDefault(
@"You gain a shield that can reflect projectiles
Summons a Terraprisma familiar that scales with minion damage
'Have you power enough to wield me?'");
        }

        protected override Color nameColor => new Color(150, 133, 100);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.AncientHallow");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AncientHallowEffect(player, Item);
        }

        public static void AncientHallowEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.AncientHallowEnchantActive = true;

            modPlayer.AddMinion(item, player.GetToggleValue("Hallowed"), ModContent.ProjectileType<HallowSword>(), 50, 2);

            //reflect proj
            if (player.GetToggleValue("HallowS") && !modPlayer.noDodge && !player.HasBuff(ModContent.BuffType<HallowCooldown>()))
            {
                const int focusRadius = 50;

                float num14 = Main.GlobalTimeWrappedHourly % 3f / 3f;
                Color fairyQueenWeaponsColor = GetFairyQueenWeaponsColor(0f, 0f, num14);

                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * focusRadius);
                    offset.Y += (float)(Math.Cos(angle) * focusRadius);
                    Dust dust = Main.dust[Dust.NewDust(
                        player.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.WhiteTorch, 0, 0, 100, fairyQueenWeaponsColor, 1f
                        )];
                    dust.velocity = player.velocity;
                    dust.noGravity = true;
                }

                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && Vector2.Distance(x.Center, player.Center) <= focusRadius + Math.Min(x.width, x.height) / 2 && ProjectileLoader.CanDamage(x) != false && ProjectileLoader.CanHitPlayer(x, player) && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
                {
                    if (x.aiStyle == ProjAIStyleID.FallingTile && x.velocity.X == 0)
                        return;

                    for (int i = 0; i < 5; i++)
                    {
                        int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.WhiteTorch, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, fairyQueenWeaponsColor, 3f);
                        Main.dust[dustId].noGravity = true;
                    }

                    // Set ownership
                    x.hostile = false;
                    x.friendly = true;
                    x.owner = player.whoAmI;

                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > player.Center.X)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }

                    // Don't know if this will help but here it is
                    x.netUpdate = true;

                    player.AddBuff(ModContent.BuffType<HallowCooldown>(), 600);
                });
            }
        }

        public static Color GetFairyQueenWeaponsColor(float alphaChannelMultiplier, float lerpToWhite, float rawHueOverride)
        {
            float num = rawHueOverride;

            float num2 = (num + 0.5f) % 1f;
            float saturation = 1f;
            float luminosity = 0.5f;

            Color color3 = Main.hslToRgb(num2, saturation, luminosity, byte.MaxValue);
            //color3 *= this.Opacity;
            if (lerpToWhite != 0f)
            {
                color3 = Color.Lerp(color3, Color.White, lerpToWhite);
            }
            color3.A = (byte)((float)color3.A * alphaChannelMultiplier);
            return color3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAncientHallowHead")
                .AddIngredient(ItemID.AncientHallowedPlateMail)
                .AddIngredient(ItemID.AncientHallowedGreaves)
                .AddIngredient(ItemID.SwordWhip) //durendal
                .AddIngredient(ItemID.BouncingShield) //sergent united
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
