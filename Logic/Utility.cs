//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

namespace IQFit.Logic
{
    public static class Utility
    {
        // Generate a randomised sequence of the integers 0, 1, ..., n - 1,
        // using the Fisher-Yates Shuffling Algorithm.
        public static int[] GenerateOrdering(Random rand, int n)
        {
            int[] ordering = new int[n];
            for (int i = 0; i < n; i++)
            {
                ordering[i] = i;
            }
            for (int i = 0; i < n - 1; i++)
            {
                int j = rand.Next(i, n);
                (ordering[i], ordering[j]) = (ordering[j], ordering[i]);
            }
            return ordering;
        }
    }
}
