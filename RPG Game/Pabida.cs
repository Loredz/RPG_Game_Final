using System;

namespace RPG_Game
{
    /// <summary>
    /// Pabida: A show-off character who loves to be the center of attention.
    /// Demonstrates Inheritance and Polymorphism.
    /// </summary>
    public class Pabida : ClassFighter
    {
        private static Random rng = new Random();
        private readonly TimeSpan skillCooldown = TimeSpan.FromSeconds(10); // 10-second cooldown
        private const int skillManaCost = 50;

        public Pabida(string name) : base(name, 100, 100) { } // 100 HP, 100 Mana

        /// <summary>
        /// Overrides Attack with a random damage between 5 and 10.
        /// </summary>
        public override int Attack()
        {
            return rng.Next(5, 11); // 5 to 10 inclusive
        }

        /// <summary>
        /// Pabida's skill: High damage attack that shows off their power.
        /// </summary>
        public override int UseSkill()
        {
            if (!IsSkillReady || Mana < 90)
            {
                // Should not be called if not ready or mana is less than 90, but handle defensively
                return 0; 
            }
            int damage = rng.Next(25, 31); // Fixed skill damage: 25-30
            ConsumeMana(Mana); // Drain all mana
            StartSkillCooldown(skillCooldown);
            return damage;
        }
    }
} 