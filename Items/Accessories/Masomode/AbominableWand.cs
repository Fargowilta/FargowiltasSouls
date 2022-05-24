using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class AbominableWand : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominable Wand");
            Tooltip.SetDefault(@"Grants immunity to Abominable Fang and Abominable Presence
Increased critical damage gained per Sparkling Adoration graze and halves heart cooldown
Spectral Abominationn periodically manifests to support your critical hits
With Styx armor, charges energy when graze is maxed
You can endure any attack and survive with 1 life
Endurance recovers when you reach full life again
'Seems like something's missing'");
            //Upgrades Cute Fishron to Cute Fishron EX");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "憎恶手杖");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"免疫憎恶毒牙
            // 擦弹增加暴击伤害的上限增加20%
            // 暴击时显现幽灵憎恶
            // 幽灵憎恶会造成突变啃啄,阻止敌人再生
            // 受到致命伤害时，保留1血不死
            // 该效果发动时，10秒钟内禁止回复血量
            // 该效果在回复到满血时才能够下一次发动
            // '看起来像是什么遗失了的东西'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 14));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 75);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Buffs.Boss.AbomFang>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Boss.AbomPresence>()] = true;

            player.GetModPlayer<FargoSoulsPlayer>().AbomWandItem = Item;
            if (player.GetModPlayer<FargoSoulsPlayer>().AbomWandCD > 0)
                player.GetModPlayer<FargoSoulsPlayer>().AbomWandCD--;
            /*if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CuteFishronRitual>()] < 1 && player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<CuteFishronRitual>(), 0, 0f, Main.myPlayer);
                player.MountFishronSpecialCounter = 300;
                player.GetDamage(DamageClass.Melee) += 0.15f;
                player.GetDamage(DamageClass.Ranged) += 0.15f;
                player.GetDamage(DamageClass.Magic) += 0.15f;
                player.GetDamage(DamageClass.Summon) += 0.15f;
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