using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class MutantEye : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Eye");
            Tooltip.SetDefault(@"Grants immunity to Mutant Fang
25% increased graze bonus critical damage cap
Upgrades Sparkling Adoration hearts to love rays
Increases critical damage gained per graze
Increases Spectral Abominationn respawn rate and damage
Reduces Abominable Rebirth duration
Press the Mutant Bomb key to unleash a wave of spheres and destroy most hostile projectiles
Mutant Bomb has a 60 second cooldown
'Only a little suspicious'");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变之眼");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫突变毒牙减益
擦弹所增加的暴击值上限增加25%
将闪光之崇的伤害性心升级为爱之射线
擦弹时获得的暴击率增加
增加幽灵憎恶显现的频率和伤害
缩短憎恶手杖触发免死效果后无法回复生命值的时间
按下'突变炸弹'后会释放一波球体并摧毁大多数来犯的敌对弹幕
突变炸弹有60秒冷却时间
'有点可疑'");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 18));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(1);
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

            player.buffImmune[mod.BuffType("MutantFang")] = true;

            fargoPlayer.MutantEye = true;
            if (!hideVisual)
                fargoPlayer.MutantEyeVisual = true;

            if (fargoPlayer.MutantEyeCD > 0)
            {
                fargoPlayer.MutantEyeCD--;

                if (fargoPlayer.MutantEyeCD == 0)
                {
                    Main.PlaySound(SoundID.Item4, player.Center);

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
                && player.ownedProjectileCounts[mod.ProjectileType("PhantasmalRing2")] <= 0)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("PhantasmalRing2"), 0, 0f, Main.myPlayer);
            }

            if (fargoPlayer.CyclonicFinCD > 0)
                fargoPlayer.CyclonicFinCD--;
        }
    }
}
