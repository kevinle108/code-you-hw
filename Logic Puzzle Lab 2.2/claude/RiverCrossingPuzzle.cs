using System;
using System.Collections.Generic;
using System.Linq;

public class RiverCrossingPuzzle
{
    private const string Fox = "fox";
    private const string Goose = "goose";
    private const string Grain = "grain";

    public struct State : IEquatable<State>
    {
        public HashSet<string> LeftBank { get; }
        public HashSet<string> RightBank { get; }
        public string BoatLocation { get; }

        public State(HashSet<string> left, HashSet<string> right, string boat)
        {
            LeftBank = new HashSet<string>(left);
            RightBank = new HashSet<string>(right);
            BoatLocation = boat;
        }

        public override bool Equals(object obj) => obj is State state && Equals(state);

        public bool Equals(State other)
        {
            return LeftBank.SetEquals(other.LeftBank) &&
                   RightBank.SetEquals(other.RightBank) &&
                   BoatLocation == other.BoatLocation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                string.Concat(LeftBank.OrderBy(x => x)),
                string.Concat(RightBank.OrderBy(x => x)),
                BoatLocation
            );
        }
    }

    private State _startState;
    private State _goalState;

    public RiverCrossingPuzzle()
    {
        var items = new HashSet<string> { Fox, Goose, Grain };
        _startState = new State(items, new HashSet<string>(), "left");
        _goalState = new State(new HashSet<string>(), items, "right");
    }

    private bool IsValidState(HashSet<string> left, HashSet<string> right, bool isStartState = false)
    {
        // Allow the initial state even though fox and goose are together
        if (isStartState)
            return true;

        // Fox and goose alone = bad
        if (left.Contains(Fox) && left.Contains(Goose) && !left.Contains(Grain))
            return false;
        if (right.Contains(Fox) && right.Contains(Goose) && !right.Contains(Grain))
            return false;

        // Goose and grain alone = bad
        if (left.Contains(Goose) && left.Contains(Grain) && !left.Contains(Fox))
            return false;
        if (right.Contains(Goose) && right.Contains(Grain) && !right.Contains(Fox))
            return false;

        return true;
    }

    private List<State> GetNextStates(State current)
    {
        var nextStates = new List<State>();

        if (current.BoatLocation == "left")
        {
            // Take each item from left to right
            foreach (var item in current.LeftBank.ToList())
            {
                var newLeft = new HashSet<string>(current.LeftBank);
                var newRight = new HashSet<string>(current.RightBank);
                newLeft.Remove(item);
                newRight.Add(item);

                // Check if remaining items on left are safe
                bool leftSafe = !(newLeft.Count == 2 && 
                    ((newLeft.Contains(Fox) && newLeft.Contains(Goose)) ||
                     (newLeft.Contains(Goose) && newLeft.Contains(Grain))));

                Console.WriteLine($"  Trying to move {item}: Left would be {FormatItems(newLeft)}, Right would be {FormatItems(newRight)} - leftSafe={leftSafe}");

                if (leftSafe)
                    nextStates.Add(new State(newLeft, newRight, "right"));
            }
        }
        else // Boat on right
        {
            // Take each item from right to left
            foreach (var item in current.RightBank.ToList())
            {
                var newLeft = new HashSet<string>(current.LeftBank);
                var newRight = new HashSet<string>(current.RightBank);
                newLeft.Add(item);
                newRight.Remove(item);

                // Check if remaining items are safe
                bool rightSafe = !(newRight.Count == 2 && 
                    ((newRight.Contains(Fox) && newRight.Contains(Goose)) ||
                     (newRight.Contains(Goose) && newRight.Contains(Grain))));

                bool leftSafe = !(newLeft.Count == 2 && 
                    ((newLeft.Contains(Fox) && newLeft.Contains(Goose)) ||
                     (newLeft.Contains(Goose) && newLeft.Contains(Grain))));

                Console.WriteLine($"  Trying to move {item}: Left would be {FormatItems(newLeft)}, Right would be {FormatItems(newRight)} - rightSafe={rightSafe}, leftSafe={leftSafe}");

                if (rightSafe && leftSafe)
                    nextStates.Add(new State(newLeft, newRight, "left"));
            }
        }

        return nextStates;
    }

    public List<State> Solve()
    {
        var queue = new Queue<(State state, List<State> path)>();
        var visited = new Dictionary<State, int>(); // State -> minimum depth where we've seen it

        queue.Enqueue((_startState, new List<State> { _startState }));
        visited[_startState] = 0;

        Console.WriteLine("Starting state:");
        Console.WriteLine($"  Left: {FormatItems(_startState.LeftBank)}, Right: {FormatItems(_startState.RightBank)}, Boat: {_startState.BoatLocation}");
        Console.WriteLine($"Goal state:");
        Console.WriteLine($"  Left: {FormatItems(_goalState.LeftBank)}, Right: {FormatItems(_goalState.RightBank)}, Boat: {_goalState.BoatLocation}");
        Console.WriteLine();

        int statesExplored = 0;

        while (queue.Count > 0)
        {
            var (currentState, path) = queue.Dequeue();
            statesExplored++;

            if (statesExplored % 100 == 0)
                Console.WriteLine($"Explored {statesExplored} states...");

            if (path.Count > 20) // Reasonable depth limit
                continue;

            var nextStates = GetNextStates(currentState);

            if (currentState.Equals(_goalState))
            {
                PrintSolution(path);
                return path;
            }

            foreach (var nextState in nextStates)
            {
                int nextDepth = path.Count + 1;
                
                // Allow revisiting states if depth is still reasonable
                if (!visited.ContainsKey(nextState) || visited[nextState] + 4 < nextDepth)
                {
                    visited[nextState] = Math.Min(visited.GetValueOrDefault(nextState, int.MaxValue), nextDepth);
                    var newPath = new List<State>(path);
                    newPath.Add(nextState);
                    queue.Enqueue((nextState, newPath));
                }
            }
        }

        Console.WriteLine($"\nTotal states explored: {statesExplored}");
        return new List<State>();
    }

    private static void PrintSolution(List<State> path)
    {
        Console.WriteLine("River Crossing Puzzle Solution\n");
        Console.WriteLine(new string('=', 70));

        for (int i = 0; i < path.Count; i++)
        {
            var state = path[i];
            Console.WriteLine($"Move {i}:");
            Console.WriteLine($"  Left Bank:  {FormatItems(state.LeftBank)}");
            Console.WriteLine($"  Right Bank: {FormatItems(state.RightBank)}");
            Console.WriteLine($"  Boat at:    {state.BoatLocation.ToUpper()}");
            Console.WriteLine();
        }
    }

    private static string FormatItems(HashSet<string> items)
    {
        return items.Count == 0 ? "Empty" : string.Join(", ", items.OrderBy(x => x));
    }

    public static void Main()
    {
        var puzzle = new RiverCrossingPuzzle();
        var solution = puzzle.Solve();

        if (solution.Count > 0)
        {
            PrintSolution(solution);
            Console.WriteLine($"Solution found in {solution.Count - 1} moves!");
        }
        else
        {
            Console.WriteLine("No solution found.");
        }
    }
}