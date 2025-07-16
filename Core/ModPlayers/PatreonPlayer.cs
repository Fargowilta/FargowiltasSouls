﻿using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Patreon.ParadoxWolf;
using FargowiltasSouls.Content.Patreon.Potato;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Core.ModPlayers
{
    public class PatreonPlayer : ModPlayer
    {
        public bool Gittle;
        public bool RoombaPet;

        public bool Sasha;
        public bool FishMinion;

        public bool CompOrb;
        public int CompOrbDrainCooldown;

        public bool ManliestDove;
        public bool DovePet;

        public bool Cat;
        public bool KingSlimeMinion;

        public bool WolfDashing;

        public bool PiranhaPlantMode;

        public bool JojoTheGamer;
        public bool PrimeMinion;

        public bool Eight3One;

        public bool Crimetroid;
        public bool ROB;

        public bool ChibiiRemii;

        public bool Northstrider;

        public bool RazorContainer;

        public bool TouhouBuff;

        public static readonly SoundStyle RazorContainerTink = new("FargowiltasSouls/Assets/Sounds/Accessories/RazorTink")  { PitchVariance = 0.25f };

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            string name = "PatreonSaves" + Player.name;
            var PatreonSaves = new List<string>();

            if (PiranhaPlantMode) PatreonSaves.Add("PiranhaPlantMode");

            tag.Add(name, PatreonSaves);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            string name = "PatreonSaves" + Player.name;
            IList<string> PatreonSaves = tag.GetList<string>(name);

            PiranhaPlantMode = PatreonSaves.Contains("PiranhaPlantMode");
        }

        public override void ResetEffects()
        {
            Gittle = false;
            RoombaPet = false;
            Sasha = false;
            FishMinion = false;
            CompOrb = false;
            ManliestDove = false;
            DovePet = false;
            Cat = false;
            KingSlimeMinion = false;
            WolfDashing = false;
            JojoTheGamer = false;
            Crimetroid = false;
            ROB = false;
            PrimeMinion = false;
            ChibiiRemii = false;
            Northstrider = false;
            RazorContainer = false;
            TouhouBuff = false;
        }

        public override void OnEnterWorld()
        {
            if (Gittle || Sasha || ManliestDove || Cat || JojoTheGamer || Northstrider || Eight3One)
            {
                string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.PatreonNameEffect");
                Main.NewText($"{text}, {Player.name}!");
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (CompOrbDrainCooldown > 0)
                CompOrbDrainCooldown -= 1;

            switch (Player.name)
            {
                case "iverhcamer":
                    Gittle = true;
                    Player.pickSpeed -= .15f;
                    //shine effect
                    Lighting.AddLight(Player.Center, 0.8f, 0.8f, 0);
                    break;
                case "Sasha":
                    Sasha = true;

                    Player.lavaImmune = true;
                    Player.fireWalk = true;
                    Player.buffImmune[BuffID.OnFire] = true;
                    Player.buffImmune[BuffID.CursedInferno] = true;
                    Player.buffImmune[BuffID.Burning] = true;
                    break;
                case "Dove":
                    ManliestDove = true;
                    break;
                case "cat":
                    Cat = true;

                    if (NPC.downedMoonlord)
                    {
                        Player.maxMinions += 4;
                    }
                    else if (Main.hardMode)
                    {
                        Player.maxMinions += 2;
                    }

                    Player.GetDamage(DamageClass.Summon) += Player.maxMinions * 0.5f;
                    break;
                case "VirtualDefender":
                    JojoTheGamer = true;
                    break;
                case "Northstrider":
                    Northstrider = true;

                    Player.wingsLogic = 3;
                    Player.wings = 3;

                    Player.wingTimeMax = 100;
                    Player.wingAccRunSpeed = 9f;
                    Player.wingRunAccelerationMult = 9f;
                    break;
                case "Eight3One":
                    Eight3One = true;
                    break;
            }

            if (CompOrb && Player.itemAnimation > 0)
            {
                Player.manaRegenDelay = Player.maxRegenDelay;
            }
        }
        public static void AddDash_Eight3One(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.FargoDash = DashManager.DashType.Crystal;
            modPlayer.CrystalAssassinDiagonal = true;
            modPlayer.HasDash = true;
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CompOrb && CompOrbDrainCooldown <= 0 && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                CompOrbDrainCooldown = 15;
                if (Player.CheckMana(10, true, false))
                    Player.manaRegenDelay = Player.maxRegenDelay;
            }
            if (Eight3One && Main.rand.NextBool(20))
                target.AddBuff(ModContent.BuffType<LightningRodBuff>(), 60);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CompOrb && CompOrbDrainCooldown <= 0 && proj.DamageType != DamageClass.Magic && proj.DamageType != DamageClass.Summon)
            {
                CompOrbDrainCooldown = 15;
                if (Player.CheckMana(10, true, false))
                    Player.manaRegenDelay = Player.maxRegenDelay;
            }
            if (Eight3One && Main.rand.NextBool(20))
                target.AddBuff(ModContent.BuffType<LightningRodBuff>(), 60);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Gittle)
            {
                if (Main.rand.NextBool(10))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (Vector2.Distance(target.Center, npc.Center) < 50)
                        {
                            npc.AddBuff(BuffID.Venom, 300);
                        }
                    }
                }

                if (ModLoader.TryGetMod("CalamityMod", out Mod _))
                {
                    target.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
                }
            }
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CompOrb && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                modifiers.FinalDamage *= 1.17f;

                if (Player.manaSick)
                    modifiers.FinalDamage *= 1f - Player.manaSickReduction;

                /*for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }*/
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CompOrb && proj.DamageType != DamageClass.Magic && proj.DamageType != DamageClass.Summon)
            {
                modifiers.FinalDamage *= 1.17f;

                if (Player.manaSick)
                    modifiers.FinalDamage *= 1f - Player.manaSickReduction;

                /*for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }*/
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByEither();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHitByEither();
        }

        private void OnHitByEither()
        {
            if (PiranhaPlantMode)
            {
                int index = Main.rand.Next(FargowiltasSouls.DebuffIDs.Count);
                Player.AddBuff(FargowiltasSouls.DebuffIDs[index], 180);
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (SoulConfig.Instance.PatreonWolf)
            {
                if (damageSource.TryGetCausingEntity(out Entity entity) && entity is NPC npc && npc.active && npc.type == NPCID.Wolf)
                {
                    Item.NewItem(Player.GetSource_Death(), Player.Hitbox, ModContent.ItemType<ParadoxWolfSoul>());
                }
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            base.HideDrawLayers(drawInfo);

            if (WolfDashing) //dont draw player during dash
                drawInfo.DrawDataCache.Clear();

            //HashSet<int> layersToRemove = new HashSet<int>();
            //for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            //{
            //    if (JojoTheGamer && drawInfo.DrawDataCache[i] == PlayerLayer.Skin)
            //    {
            //        layersToRemove.Add(i);
            //    }
            //}
            //foreach (int i in layersToRemove)
            //{
            //    drawInfo.DrawDataCache.RemoveAt(i);
            //}

            //alternative for jojo changes? idk
            //Terraria.DataStructures.PlayerDrawLayers.Skin.Hide();
        }

        public override void FrameEffects()
        {
            //if (JojoTheGamer)
            //{
            //    Player.legs = Mod.GetEquipSlot("BetaLeg", EquipType.Legs);
            //    Player.body = Mod.GetEquipSlot("BetaBody", EquipType.Body);
            //    Player.head = Mod.GetEquipSlot("BetaHead", EquipType.Head);
            //}
        }

        private int razorCD = 0;

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (RazorContainer && --razorCD <= 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile projectile = Main.projectile[i];

                    if (projectile.TypeAlive<RazorBlade>() && hitbox.Distance(projectile.Center) < 100)
                    {
                        if (Player.whoAmI == projectile.owner)
                        {
                            Vector2 velocity = Vector2.Normalize((Main.MouseWorld - Player.Center)) * 30;

                            for (int j = 0; j < 3; j++)
                            {
                                Vector2 pos = projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2, projectile.height / 2);
                                Particle p = new SparkParticle(pos, Vector2.Normalize(pos - projectile.Center) * Main.rand.NextFloat(2, 5), Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat()), 0.2f, 20);
                                p.Spawn();
                            }
                            SoundEngine.PlaySound(RazorContainerTink with { Volume = 0.25f }, projectile.Center);
                            projectile.velocity = velocity;
                            projectile.ai[0] = 1;
                            projectile.ai[1] = 0;
                            projectile.ResetLocalNPCHitImmunity();
                            projectile.netUpdate = true;
                            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
                        }
                    }
                }

                razorCD = 30;

            }


        }
    }
}