using System.Collections.Immutable;

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
