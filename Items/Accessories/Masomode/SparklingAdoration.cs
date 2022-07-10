using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SparklingAdoration : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Adoration");
            Tooltip.SetDefault(@"Grants immunity to Lovestruck and Fake Hearts
Graze attacks to gain up to 25% increased critical damage
Critical damage bonus decreases over time and is fully lost on hit
Your attacks periodically summon life-draining hearts
'With all of your emotion!'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Lovestruck>()] = true;

            if (player.GetToggleValue("MasoGraze", false))
                fargoPlayer.Graze = true;

            fargoPlayer.DevianttHeartItem = Item;

            if (fargoPlayer.Graze && player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoGrazeRing", false) && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.GrazeRing>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GrazeRing>(), 0, 0f, Main.myPlayer);
        }

        public static void OnGraze(FargoSoulsPlayer fargoPlayer, int damage)
        {
            double grazeCap = 0.25;
            if (fargoPlayer.MutantEyeItem != null)
                grazeCap += 0.25;

            double grazeGain = 0.0125;
            if (fargoPlayer.AbomWandItem != null)
                grazeGain *= 2;

            fargoPlayer.GrazeBonus += grazeGain;
            if (fargoPlayer.GrazeBonus > grazeCap)
            {
                fargoPlayer.GrazeBonus = grazeCap;
                if (fargoPlayer.StyxSet)
                    fargoPlayer.StyxMeter += FargoSoulsUtil.HighestDamageTypeScaling(Main.LocalPlayer, damage) * 4; //as if gaining the damage, times SOU crit
            }
            fargoPlayer.GrazeCounter = -1; //reset counter whenever successful graze

            if (fargoPlayer.NekomiSet)
            {
                fargoPlayer.NekomiTimer = Math.Clamp(fargoPlayer.NekomiTimer + 60, 0, 420);
            }

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Graze") { Volume = 0.5f }, Main.LocalPlayer.Center);
            }

            Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
            const int max = 64; //make some indicator dusts
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = baseVel * 3f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                //changes color when bonus is maxed
                int d = Dust.NewDust(vector6 + vector7, 0, 0, fargoPlayer.GrazeBonus >= grazeCap ? 86 : 228, 0f, 0f, 0, default(Color));
                Main.dust[d].scale = fargoPlayer.GrazeBonus >= grazeCap ? 1f : 0.75f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
}