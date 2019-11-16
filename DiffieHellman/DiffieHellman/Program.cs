using System;
using System.Collections.Generic;

namespace DiffieHellman
{
    class Program
    {
        static void Main(string[] args)
        {
            string ans = "";
            ans = Console.ReadLine();
            while (ans != "exit")
            {
                while (ans != "manual") // Run random value alogrithm
                {
                    DiffieHellman();
                    ans = Console.ReadLine();
                }

                if (ans == "manual") // Run algorithm with manaul input
                {
                    Console.Write("Enter p: ");
                    int p = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter g: ");
                    int g = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter Xa: ");
                    int a = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter Xb: ");
                    int b = Convert.ToInt32(Console.ReadLine());
                    DiffieHellman(p, g, a, b);
                }

                ans = Console.ReadLine();
            } 
        }
        
        static void DiffieHellman()
        {
            Random rand = new Random();

            int p = rand.Next(3, 30);
            while (!isPrime(p))
                p = rand.Next(3, 30); // Makes p is a random prime number

            List<int> roots = new List<int>(); // get a list of primitive roots
            foreach (int r in Roots(p)) // put hashed set of roots into a list to be indexed
                roots.Add(r);

            int g = roots[rand.Next(0, roots.Count)]; // pick a random root via index

            int alice_private_key = rand.Next(3, 30 / 2); // Get a random Xa
            int bob_private_key = rand.Next(3, 30 / 2); // Get a random Xb

            int alice_generated_key = (int)Math.Pow(g, alice_private_key) % p; // G ^ Xa mod p
            int bob_generated_key = (int)Math.Pow(g, bob_private_key) % p; // G ^ Xb mod p

            int alice_generated_secret_key = (int)Math.Pow(bob_generated_key, alice_private_key) % p; // Yb ^ Xa mod p
            int bob_generated_secret_key = (int)Math.Pow(alice_generated_key, bob_private_key) % p; ; // Ya ^ Xb mod p

            Console.WriteLine("P: " + p);
            Console.WriteLine("G: " + g);
            Console.WriteLine("Xa: " + alice_generated_key);
            Console.WriteLine("Xb: " + bob_generated_key);

            if (alice_generated_secret_key == bob_generated_secret_key)
                Console.WriteLine("K: " + alice_generated_secret_key);
            else
                Console.WriteLine("Keys are not identical");
        }

        static void DiffieHellman(int _p, int _g, int _a, int _b) // Manual input
        {
            Random rand = new Random();

            int p = _p; // Set p

            int g = _g; // Set g

            int alice_private_key = _a; // Set Xa
            int bob_private_key = _b; // Set Xb
          
            int alice_generated_key = (int)Math.Pow(g, alice_private_key) % p; // G ^ Xa mod p
            int bob_generated_key = (int)Math.Pow(g, bob_private_key) % p; // G ^ Xb mod p

            int alice_generated_secret_key = (int)Math.Pow(bob_generated_key, alice_private_key) % p; // Yb ^ Xa mod p
            int bob_generated_secret_key = (int)Math.Pow(alice_generated_key, bob_private_key) % p; ; // Ya ^ Xb mod p

            //Console.WriteLine("P: " + p);
            //Console.WriteLine("G: " + g);
            //Console.WriteLine("Xa: " + alice_generated_key);
            //Console.WriteLine("Xb: " + bob_generated_key);

            if (alice_generated_secret_key == bob_generated_secret_key)
                Console.WriteLine("K: " + alice_generated_secret_key);
            else
                Console.WriteLine("Keys are not identical");
        }

        static HashSet<int> Roots(int p)
        {
            HashSet<int> primitiveRoots = new HashSet<int>();
            if (isPrime(p)) // Check p is prime
            {
                int phi = p-1; // if p is prime phi = p-1
                List<int> coprime_group = Coprimes(phi); // Get a list of coprimes
    
                int root = findPrimitive(p); // Find a valid g
                foreach (int i in coprime_group)
                {
                    int primitive = (int)Math.Pow(root, i) % p; // Calculate the remaining primitive roots from powers of g
                    primitiveRoots.Add(primitive); // Add to set
                }
            }
            return primitiveRoots;
        }

        static int GCD(int a, int b)
        {
            if (a == 0)
                return b;
            return GCD(b % a, a); // Greatest Common Denominator
        }

        static List<int> Coprimes(int n)
        {
            List<int> coprimes = new List<int>();

            for (int i = 0; i <= n; i++)
            {
                if (GCD(n, i) == 1) // Find Coprimes of n up to n
                    coprimes.Add(i);
            }

            return coprimes;
        }

        public static HashSet<int> PrimeFactors(int n)
        {
            HashSet<int> factors = new HashSet<int>();
            while (n % 2 == 0) // only even prime is 2
            {
                factors.Add(2);
                n /= 2;
            }

            // only odd prime form here, can loop by 2 at a time
            for (int i = 3; i <= Math.Sqrt(n); i += 2)
            {
                while (n % i == 0)// While i divides n, add i and divide n 
                {
                    factors.Add(i);
                    n /= i;
                }
            }

            if (n > 2) factors.Add(n); // By this stage n is a factor to add to the list

            return factors;
        }

        static bool isPrime(int n)
        {
            if (n <= 1) return false; // 0 and 1 aren't prime
            if (n <= 3) return true; // 2 and 3 are prime
            if (n % 2 == 0) return false; // Even numbers aren't prime
            if (n % 3 == 0) return false; // Clears multiples of 3

            for (int i = 3; i <= (int)Math.Floor(Math.Sqrt(n)); i += 2)
                if (n % i == 0)
                    return false;

            return true;
        }

        static int Power(int x, int y, int p)
        {
            int res = 1; // x^1 = 1

            x = x % p; // if x is more than p, x mod p

            while (y > 0)
            {
                // If y is odd, multiply x with result 
                if (y % 2 == 1)
                    res = (res * x) % p;

                // y must be even
                y = y >> 1; // y = y/2 
                x = (x * x) % p;
            }
            return res;
        }
        
        static int findPrimitive(int n)
        {
            if (isPrime(n) == false) return -1;
           
            int phi = n - 1;

            HashSet<int> s = PrimeFactors(phi);

            // Check for every number from 2 to phi 
            for (int r = 2; r <= phi; r++)
            {
                bool flag = false;
                foreach (int a in s)
                {
                    // Check if r^((phi)/primefactors) mod n 
                    // is 1 or not 
                    if (Power(r, phi / (a), n) == 1)
                    {
                        flag = true;
                        break;
                    }
                }

                // If there was no power with value 1. 
                if (flag == false) return r;
            }

            // If no primitive root found 
            return -1;
        }
    }
}
