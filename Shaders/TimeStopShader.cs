using FargowiltasSouls.Buffs.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace FargowiltasSouls.Shaders
{
    public class TimeStopShader : ScreenShaderData
    {
        public TimeStopShader(Ref<Effect> shader, string passName) : base(shader, passName)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Filters.Scene["FargowiltasSouls:Invert"].IsActive())
            {
                FargoSoulsPlayer modPlayer = Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>();
                int d = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<TimeFrozen>());
                if ((!modPlayer.FreezeTime || modPlayer.freezeLength < 60)
                    && (d == -1 || Main.LocalPlayer.buffTime[d] < 60))
                {
                    Filters.Scene.Deactivate("FargowiltasSouls:Invert");
                }
            }
        }

        public override void Apply()
        {
            base.Apply();
        }
    }
}