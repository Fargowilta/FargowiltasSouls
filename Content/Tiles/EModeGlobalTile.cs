using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Tiles
{
    public class EModeGlobalTile : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (type == TileID.LihzahrdBrick && Framing.GetTileSafely(i, j).WallType == WallID.LihzahrdBrickUnsafe)
            {
                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && Main.LocalPlayer.Distance(new Vector2(i * 16 + 8, j * 16 + 8)) < 3000)
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<LihzahrdCurseBuff>(), 10);
            }

            if (type == TileID.LihzahrdAltar && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost
                && Main.LocalPlayer.Distance(new Vector2(i * 16 + 8, j * 16 + 8)) < 3000
                && Collision.CanHit(new Vector2(i * 16 + 8, j * 16 + 8), 0, 0, Main.LocalPlayer.Center, 0, 0)
                && Framing.GetTileSafely(Main.LocalPlayer.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                if (!Main.LocalPlayer.HasBuff(ModContent.BuffType<LihzahrdBlessingBuff>()))
                {
                    Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Buffs.LihzahrdBlessingBuff.Message"), Color.Orange);
                    SoundEngine.PlaySound(SoundID.Item4, Main.LocalPlayer.Center);
                    for (int k = 0; k < 50; k++)
                    {
                        int d = Dust.NewDust(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, DustID.Torch, 0f, 0f, 0, default, Main.rand.NextFloat(3f, 6f));
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 9f;
                    }
                }
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LihzahrdBlessingBuff>(), 60 * 60 * 10 - 1); //10mins
            }
        }

        private static bool CanBreakTileMaso(int i, int j, int type)
        {
            if ((type == TileID.Traps || type == TileID.PressurePlates) && Framing.GetTileSafely(i, j).WallType == WallID.LihzahrdBrickUnsafe)
            {
                int p = Player.FindClosest(new Vector2(i * 16 + 8, j * 16 + 8), 0, 0);
                if (p != -1)
                {
                    //if player INSIDE TEMPLE, but not cursed, its ok to break
                    Tile tile = Framing.GetTileSafely(Main.player[p].Center);
                    if (tile.WallType == WallID.LihzahrdBrickUnsafe && !Main.player[p].FargoSouls().LihzahrdCurse)
                        return true;
                }
                //if player outside temple, or player in temple but is cursed, dont break
                return false;
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.CanExplode(i, j, type);


            if (!CanBreakTileMaso(i, j, type))
                return false;


            return base.CanExplode(i, j, type);
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.CanKillTile(i, j, type, ref blockDamaged);


            if (!CanBreakTileMaso(i, j, type))
                return false;


            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (type == TileID.ShadowOrbs && Main.invasionType == 0 && !NPC.downedGoblins && WorldGen.shadowOrbSmashed)
            {
                int p = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                if (p != -1 && Main.player[p].statLifeMax2 >= 200)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Main.invasionDelay = 0;
                        Main.StartInvasion(1);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, p, -1f);
                    }
                }
            }
        }
    }
}