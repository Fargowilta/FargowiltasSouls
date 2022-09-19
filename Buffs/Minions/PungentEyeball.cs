using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class PungentEyeball : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pungent Eyeball");
            Description.SetDefault("The pungent eyeball will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "尖刻眼球");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "尖刻眼球将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().PungentEyeballMinion = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.PungentEyeball>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.PungentEyeball>(), 50, 0f, player.whoAmI);
        }
    }
}