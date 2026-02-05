// Simple BFS solver + console visualizer for Fox-Goose-Grain river crossing
using System;
using System.Collections.Generic;
using System.Linq;

static class Program
{
    static readonly string[] ITEMS = { "farmer", "fox", "goose", "grain" };
    const int FARMER = 1 << 0;
    const int FOX    = 1 << 1;
    const int GOOSE  = 1 << 2;
    const int GRAIN  = 1 << 3;
    const int START  = 0;           // all bits 0 = left side
    const int GOAL   = FARMER|FOX|GOOSE|GRAIN; // all on right

    static void Main()
    {
        var path = BFS(START, GOAL);
        if (path == null)
        {
            Console.WriteLine("No solution found.");
            return;
        }

        Console.WriteLine($"Found solution in {path.Count - 1} moves.\n");
        Animate(path);
    }

    static bool IsSafe(int state)
    {
        // If farmer is NOT on a side, check that side for forbidden pairs.
        bool farmerOnRight = (state & FARMER) != 0;

        // left side is unguarded when farmer is on right
        if (farmerOnRight)
        {
            bool fox = (state & FOX) == 0;
            bool goose = (state & GOOSE) == 0;
            bool grain = (state & GRAIN) == 0;
            if ((fox && goose) || (goose && grain)) return false;
        }

        // right side is unguarded when farmer is on left
        if (!farmerOnRight)
        {
            bool fox = (state & FOX) != 0;
            bool goose = (state & GOOSE) != 0;
            bool grain = (state & GRAIN) != 0;
            if ((fox && goose) || (goose && grain)) return false;
        }

        return true;
    }

    static List<int>? BFS(int start, int goal)
    {
        var q = new Queue<int>();
        var parent = new Dictionary<int,int>();
        q.Enqueue(start);
        parent[start] = -1;
        while (q.Count > 0)
        {
            var s = q.Dequeue();
            if (s == goal)
            {
                var path = new List<int>();
                int cur = s;
                while (cur != -1)
                {
                    path.Add(cur);
                    cur = parent[cur];
                }
                path.Reverse();
                return path;
            }
            foreach (var n in Neighbors(s))
            {
                if (!parent.ContainsKey(n))
                {
                    parent[n] = s;
                    q.Enqueue(n);
                }
            }
        }
        return null;
    }

    static IEnumerable<int> Neighbors(int state)
    {
        bool farmerOnRight = (state & FARMER) != 0;
        // farmer crosses alone
        int s = state ^ FARMER;
        if (IsSafe(s)) yield return s;
        // farmer takes one item that's on same side
        foreach (var item in new[] { FOX, GOOSE, GRAIN })
        {
            if (((state & item) != 0) == farmerOnRight) // same side as farmer
            {
                int ns = state ^ FARMER ^ item;
                if (IsSafe(ns)) yield return ns;
            }
        }
    }

    static string DescribeMove(int a, int b)
    {
        var moved = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            int bit = 1 << i;
            if (((a & bit) != 0) != ((b & bit) != 0))
                moved.Add(ITEMS[i]);
        }
        return moved.Count > 0 ? string.Join(" & ", moved) : "no move";
    }

    static void PrintState(int s)
    {
        var left = new List<string>();
        var right = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            if ((s & (1 << i)) == 0) left.Add(ITEMS[i]); else right.Add(ITEMS[i]);
        }
        Console.WriteLine($"Left : {(left.Count > 0 ? string.Join(", ", left) : "—")}");
        Console.WriteLine($"Right: {(right.Count > 0 ? string.Join(", ", right) : "—")}");
        Console.WriteLine(new string('-', 30));
    }

    static void Animate(List<int> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Console.WriteLine($"Step {i}:");
            PrintState(path[i]);
            if (i + 1 < path.Count)
            {
                Console.WriteLine("Move: " + DescribeMove(path[i], path[i+1]));
                Console.Write("Press Enter for next move...");
                Console.ReadLine();
            }
        }
    }
}