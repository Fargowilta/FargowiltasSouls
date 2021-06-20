using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
	public class MarkedforDeath : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Marked for Death");
			Description.SetDefault("Just don't get hit");
			Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "死亡标记");
            Description.AddTranslation(GameCulture.Chinese, "无伤尔");
		}

		public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().DeathMarked = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[mod.ProjectileType("DeathSkull")] < 1)
                Projectile.NewProjectile(player.Center - 200f * Vector2.UnitY, Vector2.Zero, mod.ProjectileType("DeathSkull"), 0, 0f, player.whoAmI);
        }
    }
}
