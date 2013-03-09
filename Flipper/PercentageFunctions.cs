using System;
using System.Collections.Generic;
using System.Linq;

namespace Flipper
{
    public class PercentageFunctions
    {
        private static Random Randomizer = new Random(191872349861231);

        public static Func<string, Feature, bool> DiceRoll = (user, feature) => { return (Randomizer.Next() % 100) <= feature.Percentage; };

        public static Func<string, Feature, bool> UserNameHash = (user, feature) => { return (user.GetHashCode() % 100) <= feature.Percentage; };

    }
}
