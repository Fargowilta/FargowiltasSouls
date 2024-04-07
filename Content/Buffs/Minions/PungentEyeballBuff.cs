using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class PungentEyeballBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pungent Eyeball");
            // Description.SetDefault("The pungent eyeball will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "尖刻眼球");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "尖刻眼球将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().PungentEyeballMinion = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<PungentEyeballMinion>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<PungentEyeballMinion>(), 50, 0f, player.whoAmI);
        }
    }
}