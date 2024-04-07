﻿using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face)]
    public class IceQueensCrown : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Queen's Crown");
            /* Tooltip.SetDefault(@"Grants immunity to Hypothermia
Increases damage reduction by 5%
Graze attacks to gain a bomb that will freeze almost all enemies and projectiles
Press the Bomb key to use your freeze bomb
'The royal symbol of a defeated foe'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰雪女王的皇冠");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被打败的敌人的皇家象征'
            // 免疫冻结
            // 增加5%伤害减免
            // 召唤一个友善的超级圣诞雪灵");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.05f;
            player.buffImmune[ModContent.BuffType<HypothermiaBuff>()] = true;
            AddEffects(player, Item);
        }

        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.IceQueensCrown = true;
            if (player.AddEffect<IceQueenGraze>(item))
            {
                fargoPlayer.Graze = true;
                fargoPlayer.CirnoGraze = true;
            }
            player.AddEffect<MasoGrazeRing>(item);
            if (fargoPlayer.Graze && player.whoAmI == Main.myPlayer && player.HasEffect<MasoGrazeRing>() && player.ownedProjectileCounts[ModContent.ProjectileType<GrazeRing>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ModContent.ProjectileType<GrazeRing>(), 0, 0f, Main.myPlayer);
        }

        public const int CIRNO_GRAZE_THRESHOLD = 9999;
        public const int CIRNO_GRAZE_MAX = CIRNO_GRAZE_THRESHOLD + 9 * 60;

        public static void OnGraze(FargoSoulsPlayer fargoPlayer, int damage)
        {
            fargoPlayer.CirnoGrazeCounter += damage;
            if (fargoPlayer.CirnoGrazeCounter > CIRNO_GRAZE_MAX)
                fargoPlayer.CirnoGrazeCounter = CIRNO_GRAZE_MAX;
            if (fargoPlayer.CirnoGrazeCounter == CIRNO_GRAZE_MAX && fargoPlayer.Player.whoAmI == Main.myPlayer && fargoPlayer.Player.ownedProjectileCounts[ModContent.ProjectileType<CirnoBomb>()] < 1)
            {
                Projectile.NewProjectile(fargoPlayer.Player.GetSource_Misc(""), fargoPlayer.Player.Center, Vector2.Zero, ModContent.ProjectileType<CirnoBomb>(), 0, 0f, Main.myPlayer);
            }

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Graze") { Volume = 0.5f }, Main.LocalPlayer.Center);
            }

            Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
            const int max = 64; //make some indicator dusts
            bool capped = fargoPlayer.CirnoGrazeCounter > CIRNO_GRAZE_THRESHOLD;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = baseVel * 3f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                //changes color when maxed
                int d = Dust.NewDust(vector6 + vector7, 0, 0, capped ? DustID.IceTorch : 228, 0f, 0f, 0, default);
                Main.dust[d].scale = capped ? 1f : 0.75f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
    public class IceQueenGraze : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<IceQueensCrown>();

    }
}