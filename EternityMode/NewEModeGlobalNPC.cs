using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class NewEModeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public List<EModeNPCBehaviour> EModeNpcBehaviours = new List<EModeNPCBehaviour>();

        public bool FirstTick = true;
        public bool enteredSetDefaults;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            InitBehaviourList(npc);

            if (!Main.dedServ)
            {
                bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.EternityMode;
                if (recolor || FargowiltasSouls.Instance.LoadedNewSprites)
                {
                    FargowiltasSouls.Instance.LoadedNewSprites = true;
                    foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                    {
                        behaviour.LoadSprites(npc, recolor);
                    }
                }
            }

            if (FargoSoulsWorld.EternityMode) //needs to be like this to avoid bestiary/npcloot issues and crashes
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.SetDefaults(npc);
                }
            }
        }

        private void InitBehaviourList(NPC npc)
        {
            // TODO Try caching this again? Last attempt caused major fails
            IEnumerable<EModeNPCBehaviour> behaviours = EModeNPCBehaviour.AllEModeNpcBehaviours
                .Where(m => m.Matcher.Satisfies(npc.type));

            // To make sure they're always in the same order
            // TODO is ordering needed? Do they always have the same order?
            behaviours.OrderBy(m => m.GetType().FullName, StringComparer.InvariantCulture);

            EModeNpcBehaviours = behaviours.Select(m => m.NewInstance()).ToList();
        }

        #region Behaviour Hooks
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.OnSpawn(npc, source);
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (!FargoSoulsWorld.EternityMode)
                return true;

            bool result = true;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                if (FirstTick)
                    behaviour.OnFirstTick(npc);

                result &= behaviour.PreAI(npc);
            }

            FirstTick = false;

            return result;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!FargoSoulsWorld.EternityMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.AI(npc);
            }
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);

            if (!FargoSoulsWorld.EternityMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.PostAI(npc);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.ModifyNPCLoot(npc, npcLoot);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.OnKill(npc);
                }
            }
        }

        public override bool SpecialOnKill(NPC npc)
        {
            bool result = base.SpecialOnKill(npc);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.SpecialOnKill(npc);
                }
            }

            return result;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            bool result = true;

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.CanHitPlayer(npc, target, ref CooldownSlot);
                }
            }

            return result;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            bool? result = base.CanBeHitByItem(npc, player, item);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.CanBeHitByItem(npc, player, item);
                }
            }

            return result;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            bool? result = base.CanBeHitByProjectile(npc, projectile);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.CanBeHitByProjectile(npc, projectile);
                }
            }

            return result;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            base.ModifyHitPlayer(npc, target, ref damage, ref crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.ModifyHitPlayer(npc, target, ref damage, ref crit);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.OnHitPlayer(npc, target, damage, crit);
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(npc, target, damage, knockback, crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.OnHitNPC(npc, target, damage, knockback, crit);
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
                }
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            base.OnHitByItem(npc, player, item, damage, knockback, crit);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.OnHitByItem(npc, player, item, damage, knockback, crit);
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.OnHitByProjectile(npc, projectile, damage, knockback, crit);
                }
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool result = base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
                }
            }

            return result;
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            base.HitEffect(npc, hitDirection, damage);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.HitEffect(npc, hitDirection, damage);
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            base.UpdateLifeRegen(npc, ref damage);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.UpdateLifeRegen(npc, ref damage);
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            bool result = base.CheckDead(npc);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.CheckDead(npc);
                }
            }

            return result;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            Color? result = base.GetAlpha(npc, drawColor);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result = behaviour.GetAlpha(npc, drawColor);
                }
            }

            return result;
        }

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            bool? result = base.DrawHealthBar(npc, hbPosition, ref scale, ref position);

            if (FargoSoulsWorld.EternityMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.DrawHealthBar(npc, hbPosition, ref scale, ref position);
                }
            }

            return result;
        }

        public void NetSync(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = FargowiltasSouls.Instance.GetPacket();
            packet.Write((byte)FargowiltasSouls.PacketID.SyncEModeNPC);
            packet.Write(npc.whoAmI);
            packet.Write(npc.type);

            int bytesLength = 0;
            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                bytesLength += behaviour.GetBytesNeeded();
            packet.Write(bytesLength);

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NetSend(packet);
            }

            packet.Send();
        }

        public void NetRecieve(BinaryReader reader)
        {
            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NetRecieve(reader);
            }
        }
        #endregion
    }
}
