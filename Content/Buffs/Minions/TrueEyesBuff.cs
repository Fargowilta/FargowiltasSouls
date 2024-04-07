using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class TrueEyesBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("True Eyes of Cthulhu");
            // Description.SetDefault("The eyes of Cthulhu will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·克苏鲁之眼");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之眼将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().TrueEyes = true;

            if (player.whoAmI == Main.myPlayer)
            {
                const int damage = 60;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeL>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeL>(), damage, 3f, player.whoAmI, -1f);

                if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeR>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeR>(), damage, 3f, player.whoAmI, -1f);

                if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeS>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeS>(), damage, 3f, player.whoAmI, -1f);
            }
        }
    }
}