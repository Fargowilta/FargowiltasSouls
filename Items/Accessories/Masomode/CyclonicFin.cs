﻿using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class CyclonicFin : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominable Wand");
            Tooltip.SetDefault(@"Grants immunity to Abominable Fang
Increased critical damage gained per Sparkling Adoration graze
Halves Sparkling Adoration heart cooldown
Spectral Abominationn periodically manifests to support your critical hits
Spectral Abominationn also inflicts Mutant Nibble
You can endure any attack and survive with 1 life
Once triggered, you cannot heal for 10 seconds
Endurance recovers when you reach full life again
'Seems like something's missing'");
            //Upgrades Cute Fishron to Cute Fishron EX");
            DisplayName.AddTranslation(GameCulture.Chinese, "憎恶手杖");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫憎恶毒牙减益
增加闪光之崇擦弹所增加的暴击值上限
减半闪光之崇召唤伤害性心的周期
暴击时显现幽灵憎恶
幽灵憎恶会造成突变啃啄减益
受到致死伤害时会为你保留1点生命值而非死亡
在免死效果触发后，在10秒内无法回复生命值
在免死效果触发后，在你的生命值回复至满之前，免死效果不会再次触发
'似乎少了点什么'");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 14));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 17);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("AbomFang")] = true;
            player.GetModPlayer<FargoPlayer>().CyclonicFin = true;
            if (player.GetModPlayer<FargoPlayer>().CyclonicFinCD > 0)
                player.GetModPlayer<FargoPlayer>().CyclonicFinCD--;
            /*if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("CuteFishronRitual")] < 1 && player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.MountedCenter, Vector2.Zero, mod.ProjectileType("CuteFishronRitual"), 0, 0f, Main.myPlayer);
                player.MountFishronSpecialCounter = 300;
                player.meleeDamage += 0.15f;
                player.rangedDamage += 0.15f;
                player.magicDamage += 0.15f;
                player.minionDamage += 0.15f;
                player.meleeCrit += 30;
                player.rangedCrit += 30;
                player.magicCrit += 30;
                player.statDefense += 30;
                player.lifeRegen += 3;
                player.lifeRegenCount += 3;
                player.lifeRegenTime += 3;
                if (player.controlLeft == player.controlRight)
                {
                    if (player.velocity.X != 0)
                        player.velocity.X -= player.mount.Acceleration * Math.Sign(player.velocity.X);
                    if (player.velocity.X != 0)
                        player.velocity.X -= player.mount.Acceleration * Math.Sign(player.velocity.X);
                }
                else if (player.controlLeft)
                {
                    player.velocity.X -= player.mount.Acceleration * 4f;
                    if (player.velocity.X < -16f)
                        player.velocity.X = -16f;
                    if (!player.controlUseItem)
                        player.direction = -1;
                }
                else if (player.controlRight)
                {
                    player.velocity.X += player.mount.Acceleration * 4f;
                    if (player.velocity.X > 16f)
                        player.velocity.X = 16f;
                    if (!player.controlUseItem)
                        player.direction = 1;
                }
                if (player.controlUp == player.controlDown)
                {
                    if (player.velocity.Y != 0)
                        player.velocity.Y -= player.mount.Acceleration * Math.Sign(player.velocity.Y);
                    if (player.velocity.Y != 0)
                        player.velocity.Y -= player.mount.Acceleration * Math.Sign(player.velocity.Y);
                }
                else if (player.controlUp)
                {
                    player.velocity.Y -= player.mount.Acceleration * 4f;
                    if (player.velocity.Y < -16f)
                        player.velocity.Y = -16f;
                }
                else if (player.controlDown)
                {
                    player.velocity.Y += player.mount.Acceleration * 4f;
                    if (player.velocity.Y > 16f)
                        player.velocity.Y = 16f;
                }
            }*/
        }
    }
}
