
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class SparklingAdoration : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sparkling Adoration");
            /* Tooltip.SetDefault(@"Grants immunity to Lovestruck and Fake Hearts
Graze attacks to gain up to x1.25 increased damage
Damage bonus decreases over time and is fully lost on hit
Your attacks periodically summon life-draining hearts
'With all of your emotion!'"); */
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
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.LovestruckBuff>()] = true;

            if (player.AddEffect<MasoGraze>(Item))
            {
                fargoPlayer.Graze = true;
                fargoPlayer.DeviGraze = true;
            }

            fargoPlayer.DevianttHeartItem = Item;
            player.AddEffect<DevianttHearts>(Item);

            player.AddEffect<MasoGrazeRing>(Item);
            if (fargoPlayer.Graze && player.whoAmI == Main.myPlayer && player.HasEffect<MasoGrazeRing>() && player.ownedProjectileCounts[ModContent.ProjectileType<GrazeRing>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GrazeRing>(), 0, 0f, Main.myPlayer);
        }

        public static void OnGraze(FargoSoulsPlayer fargoPlayer, int damage)
        {
            double grazeCap = 0.25;
            if (fargoPlayer.MutantEyeItem != null)
                grazeCap += 0.25;

            double grazeGain = 0.0125;
            grazeGain *= 0.75;
            if (fargoPlayer.AbomWandItem != null)
                grazeGain *= 2;

            fargoPlayer.DeviGrazeBonus += grazeGain;
            if (fargoPlayer.DeviGrazeBonus > grazeCap)
            {
                fargoPlayer.DeviGrazeBonus = grazeCap;
                if (fargoPlayer.StyxSet)
                    fargoPlayer.StyxMeter += FargoSoulsUtil.HighestDamageTypeScaling(Main.LocalPlayer, damage) * 4; //as if gaining the damage, times SOU crit
            }
            fargoPlayer.DeviGrazeCounter = -1; //reset counter whenever successful graze

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Graze") { Volume = 0.5f }, Main.LocalPlayer.Center);
            }

            Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
            const int max = 64; //make some indicator dusts
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = baseVel * 3f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                //changes color when bonus is maxed
                int d = Dust.NewDust(vector6 + vector7, 0, 0, fargoPlayer.DeviGrazeBonus >= grazeCap ? 86 : 228, 0f, 0f, 0, default);
                Main.dust[d].scale = fargoPlayer.DeviGrazeBonus >= grazeCap ? 1f : 0.75f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
    public class DevianttHearts : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SparklingAdoration>();
        public override bool ExtraAttackEffect => true;
    }
    public class MasoGraze : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SparklingAdoration>();
        public override bool IgnoresMutantPresence => true;
    }
}