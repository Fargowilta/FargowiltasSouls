using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class MutantEye : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Eye");
            Tooltip.SetDefault(@"Grants immunity to Mutant Fang and Mutant Presence
25% increased graze bonus critical damage cap
Upgrades Sparkling Adoration hearts to love rays
Increases critical damage gained per graze
Increases Spectral Abominationn respawn rate and damage
Press the Mutant Bomb key to unleash a wave of spheres and destroy most hostile projectiles
Mutant Bomb has a 60 second cooldown
'Only a little suspicious'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变者之眼");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'有点可疑'
            // 擦弹增加暴击伤害的上限增加50%
            // 每次擦弹增加暴击伤害的数值增加
            // 增加幽灵憎恶的重生频率和伤害
            // 减少憎恶手杖复活效果禁止回血的时间
            // 按下Mutant Bomb快捷键释放一波球并破坏多数敌对抛射物
            // Mutant Bomb有60秒的冷却");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 18));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(1);
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            player.buffImmune[ModContent.BuffType<Buffs.Boss.MutantFang>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Boss.MutantPresence>()] = true;

            fargoPlayer.MutantEyeItem = Item;
            if (!hideVisual)
                fargoPlayer.MutantEyeVisual = true;

            if (fargoPlayer.MutantEyeCD > 0)
            {
                fargoPlayer.MutantEyeCD--;

                if (fargoPlayer.MutantEyeCD == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item4, player.Center);

                    const int max = 50; //make some indicator dusts
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 8f;
                        vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                        Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, 229, 0f, 0f, 0, default(Color), 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = vector7;
                    }

                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 8f;
                    }
                }
            }

            if (player.whoAmI == Main.myPlayer && fargoPlayer.MutantEyeVisual && fargoPlayer.MutantEyeCD <= 0
                && player.ownedProjectileCounts[ModContent.ProjectileType<PhantasmalRing2>()] <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalRing2>(), 0, 0f, Main.myPlayer);
            }

            if (fargoPlayer.AbomWandCD > 0)
                fargoPlayer.AbomWandCD--;
        }
    }
}