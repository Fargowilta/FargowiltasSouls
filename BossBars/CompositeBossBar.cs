using FargowiltasSouls.NPCs.Challengers;
using FargowiltasSouls.NPCs.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace FargowiltasSouls.BossBars
{
    public class CompositeBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];

            return base.GetIconTexture(ref iconFrame);
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            NPC npc = FargoSoulsUtil.NPCExists(info.npcIndexToAimAt);

            if (npc == null || !npc.active)
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            int life = npc.life;
            int lifeMax = npc.lifeMax;

            bool retval = true;

            if (npc.ModNPC is TrojanSquirrel trojanSquirrel)
            {
                if (trojanSquirrel.head != null)
                    life += trojanSquirrel.head.life;
                if (trojanSquirrel.arms != null)
                    life += trojanSquirrel.arms.life;

                lifeMax += trojanSquirrel.lifeMaxHead;
                lifeMax += trojanSquirrel.lifeMaxArms;
            }
            else if (npc.type == ModContent.NPCType<ShadowChampion>())
            {
                if (npc.ai[0] == -1)
                {
                    shieldPercent = 1;
                }
                else
                {
                    int ballCount = 0;
                    int untouchedBalls = 0;
                    foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<ShadowOrb>() && n.ai[0] == npc.whoAmI))
                    {
                        ballCount++;
                        if (!n.dontTakeDamage)
                            untouchedBalls++;
                    }
                    shieldPercent = Utils.Clamp((float)untouchedBalls / ballCount, 0f, 1f);
                }
            }
            else
            {
                retval = false;
            }

            lifePercent = Utils.Clamp((float)life / lifeMax, 0f, 1f);

            return retval;
        }
    }
}
