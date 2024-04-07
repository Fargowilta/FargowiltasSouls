using FargowiltasSouls.Content.Projectiles.Masomode;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class MarkedforDeathBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Marked for Death");
            // Description.SetDefault("Just don't get hit");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死亡标记");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "别被打到");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().DeathMarked = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<DeathSkull>()] < 1)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex),
                    player.Center - 200f * Vector2.UnitY, Vector2.Zero,
                    ModContent.ProjectileType<DeathSkull>(), 0, 0f, player.whoAmI);
        }
    }
}
