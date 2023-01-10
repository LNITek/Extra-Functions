using System;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Extra Math Functions To Fill Every Need.
    /// </summary>
    public static class ExMath
    {
        /// <summary>
        /// Advansed Rounding.
        /// </summary>
        /// <param name="Value">The Value That You Want To Round.</param>
        /// <param name="Decimals">To What Decimal Space To Round At. (Not 0)</param>
        /// <returns></returns>
        public static decimal Round(decimal Value, int Decimals = 1)
        {
            //Var
            decimal rAns = 0, rNum;
            //Code
            if (Decimals == 0)
                return rAns;
            Decimals = 10 * Decimals;
            rNum = Math.Floor(Value);
            rAns = (Value - rNum) * Decimals;
            rAns = Math.Round(rAns) / Decimals;
            rAns += rNum;
            return rAns;
        }

        /// <summary>
        /// Advansed Rounding.
        /// </summary>
        /// <param name="Value">The Value That You Want To Round.</param>
        /// <param name="Decimals">To What Decimal Space To Round At. (Not 0)</param>
        /// <returns></returns>
        public static double Round(double Value, int Decimals = 1)
        {
            //Var
            double rAns = 0, rNum;
            //Code
            if (Decimals == 0)
                return rAns;
            Decimals = 10 * Decimals;
            rNum = Math.Floor(Value);
            rAns = (Value - rNum) * Decimals;
            rAns = Math.Round(rAns) / Decimals;
            rAns += rNum;
            return rAns;
        }
    }
}
