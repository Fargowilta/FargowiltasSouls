using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class TrueEyes : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("True Eyes of Cthulhu");
            //Description.SetDefault("The eyes of Cthulhu will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "真·克苏鲁之眼");
            //Description.AddTranslation(GameCulture.Chinese, "克苏鲁之眼将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().TrueEyes = true;

            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("TrueEyeL")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("TrueEyeL"), 0, 3f, player.whoAmI, -1f);

                if (player.ownedProjectileCounts[mod.ProjectileType("TrueEyeR")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("TrueEyeR"), 0, 3f, player.whoAmI, -1f);

                if (player.ownedProjectileCounts[mod.ProjectileType("TrueEyeS")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("TrueEyeS"), 0, 3f, player.whoAmI, -1f);
            }
        }
    }
}