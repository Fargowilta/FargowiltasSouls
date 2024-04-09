using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace FargowiltasSouls.Content.Sky
{
    public class MoonLordSky : CustomSky
    {
        private bool isActive = false;

        public override void Update(GameTime gameTime)
        {
            int vulState = -1;
            int vulTimer = 0;
            bool bossAlive = false;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore))
            {
                vulState = Main.npc[EModeGlobalNPC.moonBoss].GetGlobalNPC<MoonLordCore>().VulnerabilityState;
                vulTimer = (int)Main.npc[EModeGlobalNPC.moonBoss].GetGlobalNPC<MoonLordCore>().VulnerabilityTimer;
                bossAlive = true;
            }

            if (!Main.dedServ && vulTimer % 30 == 0)
            {
                bool HandleScene(string name, int neededState)
                {
                    if (Filters.Scene[$"FargowiltasSouls:{name}"].IsActive())
                    {
                        if (vulState != neededState)
                            Filters.Scene.Deactivate($"FargowiltasSouls:{name}");
                        return false;
                    }
                    return true;
                }

                if (HandleScene("Solar", 0) & HandleScene("Vortex", 1)
                    & HandleScene("Nebula", 2) & HandleScene("Stardust", 3) & !bossAlive)
                {
                    Deactivate();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {

        }

        public override float GetCloudAlpha()
        {
            return base.GetCloudAlpha();
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override Color OnTileColor(Color inColor)
        {
            return base.OnTileColor(inColor);
        }
    }
}