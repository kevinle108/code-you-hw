using System;
using System.Collections.Generic;

class RiverCrossingSolver
{
    enum Side { Left, Right }

    class State
    {
        public Side Farmer, Fox, Goose, Grain;
        public State Previous;
        public string Move;

        public State(Side farmer, Side fox, Side goose, Side grain, State prev = null, string move = "")
        {
            Farmer = farmer; Fox = fox; Goose = goose; Grain = grain;
            Previous = prev; Move = move;
        }

        public bool IsGoal() =>
            Farmer == Side.Right && Fox == Side.Right && Goose == Side.Right && Grain == Side.Right;

        public override bool Equals(object obj)
        {
            if (obj is State s)
                return Farmer == s.Farmer && Fox == s.Fox && Goose == s.Goose && Grain == s.Grain;
            return false;
        }

        public override int GetHashCode() =>
            (int)Farmer | ((int)Fox << 2) | ((int)Goose << 4) | ((int)Grain << 6);

        public bool IsValid()
        {
            // If farmer is not with goose, fox and goose can't be together
            if (Farmer != Goose && Fox == Goose) return false;
            // If farmer is not with goose, goose and grain can't be together
            if (Farmer != Goose && Goose == Grain) return false;
            return true;
        }
    }

    static IEnumerable<State> GetNextStates(State s)
    {
        var nextSide = s.Farmer == Side.Left ? Side.Right : Side.Left;

        // Farmer crosses alone
        yield return new State(nextSide, s.Fox, s.Goose, s.Grain, s, "Farmer crosses alone");

        // Farmer takes Fox
        if (s.Farmer == s.Fox)
            yield return new State(nextSide, nextSide, s.Goose, s.Grain, s, "Farmer takes Fox");

        // Farmer takes Goose
        if (s.Farmer == s.Goose)
            yield return new State(nextSide, s.Fox, nextSide, s.Grain, s, "Farmer takes Goose");

        // Farmer takes Grain
        if (s.Farmer == s.Grain)
            yield return new State(nextSide, s.Fox, s.Goose, nextSide, s, "Farmer takes Grain");
    }

    public static void Solve()
    {
        var start = new State(Side.Left, Side.Left, Side.Left, Side.Left);
        var queue = new Queue<State>();
        var visited = new HashSet<State>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!current.IsValid() || visited.Contains(current)) continue;
            visited.Add(current);

            if (current.IsGoal())
            {
                PrintSolution(current);
                return;
            }

            foreach (var next in GetNextStates(current))
            {
                if (next.IsValid() && !visited.Contains(next))
                    queue.Enqueue(next);
            }
        }
        Console.WriteLine("No solution found.");
    }

    static void PrintSolution(State goal)
    {
        var path = new Stack<State>();
        for (var s = goal; s != null; s = s.Previous)
            path.Push(s);

        int step = 0;
        State prev = null;
        while (path.Count > 0)
        {
            var s = path.Pop();
            if (prev != null)
                Console.WriteLine($"Step {step++}: {s.Move}");
            Console.WriteLine($"   Farmer: {s.Farmer}, Fox: {s.Fox}, Goose: {s.Goose}, Grain: {s.Grain}");
            prev = s;
        }
    }

    static void Main()
    {
        Solve();
    }
}