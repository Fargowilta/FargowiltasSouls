using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Pets
{
    public class ChibiDevi : ModProjectile
    {
        private Vector2 oldMouse;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibi Devi");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 44;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
            writer.Write(Projectile.direction);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();
            Projectile.direction = reader.ReadInt32();
        }

        private static bool haveDoneInitScramble;
        private static bool usePlayerDiedSpawnText;

        public override void OnSpawn(IEntitySource source)
        {
            if (!haveDoneInitScramble)
            {
                haveDoneInitScramble = true;

                for (int i = 0; i < TalkCounters.Length; i++)
                    TalkCounters[i] = Main.rand.Next(MaxThingsToSay[i]);

                TalkCDs[(int)TalkType.Idle] = MediumCD;
            }

            TryTalkWithCD(usePlayerDiedSpawnText ? TalkType.Respawn : TalkType.Spawn, ShortCD);

            if (Projectile.owner == Main.myPlayer)
                usePlayerDiedSpawnText = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.dead || player.ghost)
            {
                modPlayer.ChibiDevi = false;
            }
            if (modPlayer.ChibiDevi)
            {
                Projectile.timeLeft = 2;
            }

            DelegateMethods.v3_1 = new Vector3(1f, 0.5f, 0.9f) * 0.75f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 6f, 20f, DelegateMethods.CastLightOpen);
            Utils.PlotTileLine(Projectile.Left, Projectile.Right, 20f, DelegateMethods.CastLightOpen);

            bool asleep = Projectile.ai[0] == 1;
            if (asleep)
            {
                Projectile.tileCollide = true;
                Projectile.ignoreWater = false;

                Projectile.frameCounter = 0;
                Projectile.frame = Projectile.velocity.Y == 0 ? 5 : 4;

                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y += 0.3f;

                if (Projectile.owner == Main.myPlayer && Projectile.Distance(Main.MouseWorld) > 180)
                {
                    Projectile.ai[0] = 0;

                    TryTalkWithCD(TalkType.Wake, ShortCD);
                }
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.tileCollide = false;
                    Projectile.ignoreWater = true;

                    Projectile.direction = Projectile.Center.X < Main.MouseWorld.X ? 1 : -1;

                    float distance = 2500;
                    float possibleDist = Main.player[Projectile.owner].Distance(Main.MouseWorld) / 2 + 100;
                    if (distance < possibleDist)
                        distance = possibleDist;
                    if (Projectile.Distance(Main.player[Projectile.owner].Center) > distance && Projectile.Distance(Main.MouseWorld) > distance)
                    {
                        Projectile.Center = player.Center;
                        Projectile.velocity = Vector2.Zero;
                    }

                    if (Projectile.Distance(Main.MouseWorld) > 30)
                        Movement(Main.MouseWorld, 0.15f, 32f);

                    if (oldMouse == Main.MouseWorld)
                    {
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] > 600)
                        {
                            bool okToRest = !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);

                            if (okToRest)
                            {
                                okToRest = false;

                                Vector2 targetPos = new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height);
                                for (int i = 0; i < 10; i++) //collision check below self
                                {
                                    targetPos.Y += 16;
                                    Tile tile = Framing.GetTileSafely(targetPos); //if solid, ok
                                    if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType])
                                    {
                                        okToRest = true;
                                        break;
                                    }
                                }
                            }

                            if (okToRest) //not in solid tiles, but found tiles within a short distance below
                            {
                                Projectile.ai[0] = 1;
                                Projectile.ai[1] = 0;

                                TryTalkWithCD(TalkType.Sleep, ShortCD);
                            }
                            else //try again in a bit
                            {
                                Projectile.ai[1] = 540;
                            }
                        }
                    }
                    else
                    {
                        Projectile.ai[1] = 0;
                        oldMouse = Main.MouseWorld;
                    }
                }

                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 4)
                        Projectile.frame = 0;
                }
            }

            Projectile.spriteDirection = Projectile.direction;

            bool bossAlive = FargoSoulsUtil.AnyBossAlive();
            if (bossAlive)
            {
                TryTalkWithCD(TalkType.BossSpawn, ShortCD);

                if (Main.npc[FargoSoulsGlobalNPC.boss].life < Main.npc[FargoSoulsGlobalNPC.boss].lifeMax / 4)
                    TryTalkWithCD(TalkType.BossAlmostDead, MediumCD);
            }
            else
            {
                //only do idle talk when awake, not a boss fight, and not in danger
                if (asleep)
                    TalkCDs[(int)TalkType.Idle] = Math.Max(TalkCDs[(int)TalkType.Idle], 12 * 60);
                else if (player.statLife > player.statLifeMax2 / 2)
                    TryTalkWithCD(TalkType.Idle, MediumCD);

                //wont cheer in boss fight unless over 30 seconds
                const int timeRequirement = 30 * 60;
                TalkCDs[(int)TalkType.BossAlmostDead] = Math.Max(TalkCDs[(int)TalkType.BossAlmostDead], timeRequirement);
                TalkCDs[(int)TalkType.KillBoss] = Math.Max(TalkCDs[(int)TalkType.KillBoss], timeRequirement);
            }

            if (universalTalkCD > 0)
                universalTalkCD--;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < TalkCDs.Length; i++)
                {
                    //dont run these timer during a boss fight
                    if (bossAlive && (i == (int)TalkType.Idle || i == (int)TalkType.BossSpawn))
                        continue;

                    if (TalkCDs[i] > 0)
                        TalkCDs[i]--;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || player.ghost)
            {
                TryTalkWithCD(TalkType.PlayerDeath, MediumCD);
                if (Projectile.owner == Main.myPlayer)
                    usePlayerDiedSpawnText = true;
            }
            else
            {
                TryTalkWithCD(TalkType.ProjDeath, ShortCD);
            }
        }

        #region

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X *= 0.95f;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X *= 0.95f;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y *= 0.95f;
            }
            else
            {
                Projectile.velocity.Y -= speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y *= 0.95f;
            }
            if (Math.Abs(Projectile.velocity.X) > cap)
                Projectile.velocity.X = cap * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > cap)
                Projectile.velocity.Y = cap * Math.Sign(Projectile.velocity.Y);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        #endregion

        public override void Unload()
        {
            TalkCounters = null;
            TalkCDs = null;
        }

        public static int[] TalkCounters = new int[(int)TalkType.Count];
        public static int[] TalkCDs = new int[(int)TalkType.Count];

        int universalTalkCD;

        public enum TalkType
        {
            Spawn,
            Respawn,
            Idle,
            Sleep,
            Wake,
            ProjDeath,
            PlayerDeath,
            BossAlmostDead,
            KillBoss,
            BossSpawn,

            Count
        };
        private int[] MaxThingsToSay => new int[] {
            5, //Spawn
            7, //Respawn
            10, //Idle
            5, //Sleep
            5, //Wake
            4, //ProjDeath
            7, //PlayerDeath
            6, //BossAlmostDead
            7, //KillBoss
            8, //BossSpawn
            1 //Count
        };

        public int ShortCD => 600;
        public int MediumCD => Main.rand.Next(3600, 7200);

        public void TryTalkWithCD(TalkType talkType, int CD)
        {
            int talkInt = (int)talkType;

            if (TalkCDs[talkInt] > 0 || universalTalkCD > 0)
                return;

            TalkCounters[talkInt] = (TalkCounters[talkInt] + 1) % MaxThingsToSay[talkInt];
            TalkCDs[talkInt] = CD;
            universalTalkCD = 0;

            if (Projectile.owner == Main.myPlayer && ModContent.GetInstance<SoulConfig>().DeviChatter)
            {
                if (!Main.player[Projectile.owner].dead && !Main.player[Projectile.owner].ghost)
                    EmoteBubble.MakeLocalPlayerEmote(EmoteID.EmotionLove);
                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, Projectile.Center);

                string key = Enum.GetName(talkType);
                int actualSay = TalkCounters[talkInt] + 1;
                string text = Language.GetTextValue($"Mods.FargowiltasSouls.DeviChatter.{key}{actualSay}");

                Vector2 pos = Vector2.Lerp(Main.MouseWorld, Projectile.Center, 0.5f);
                pos = Vector2.Lerp(pos, Main.player[Projectile.owner].Center, 0.5f);

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = text,
                    DurationInFrames = 420,
                    Velocity = 7 * -Vector2.UnitY,
                    Color = Color.HotPink * 1.15f
                }, pos);

                //FargoSoulsUtil.HeartDust(
                //    Projectile.Center, 
                //    addedVel: 0.5f * new Vector2(7 * Projectile.direction, -7),
                //    spreadModifier: 0.5f,
                //    scaleModifier: 0.5f
                //);
            }
        }
    }

    public class DeviTalkGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.boss && Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().ChibiDevi)
            {
                Projectile p = Main.projectile.FirstOrDefault(p => p.active && p.owner == Main.myPlayer && p.type == ModContent.ProjectileType<ChibiDevi>());
                if (p != null && p.ModProjectile is ChibiDevi devi)
                    devi.TryTalkWithCD(ChibiDevi.TalkType.KillBoss, devi.MediumCD);
            }
        }
    }
}