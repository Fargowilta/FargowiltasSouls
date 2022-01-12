using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class PungentEyeball : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Pungent Eyeball");
            //Description.SetDefault("The pungent eyeball will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "尖刻眼球");
            //Description.AddTranslation(GameCulture.Chinese, "尖刻眼球将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().PungentEyeballMinion = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[mod.ProjectileType("PungentEyeball")] < 1)
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("PungentEyeball"), 0, 0f, player.whoAmI);
        }
    }
}