# Simple BFS solver + ASCII visualizer for Fox-Goose-Grain river crossing

from collections import deque

ITEMS = ["farmer", "fox", "goose", "grain"]

def is_safe(state):
    # state: tuple of 4 booleans (False=left, True=right)
    farmer_side = state[0]
    # If farmer not on a side, check pairs that can't be left alone
    for side in (False, True):
        if farmer_side != side:
            fox = state[1] == side
            goose = state[2] == side
            grain = state[3] == side
            # fox with goose alone OR goose with grain alone -> unsafe
            if (fox and goose) or (goose and grain):
                return False
    return True

def neighbors(state):
    results = []
    farmer_side = state[0]
    # farmer crosses alone
    new_state = list(state)
    new_state[0] = not farmer_side
    if is_safe(tuple(new_state)):
        results.append(tuple(new_state))
    # farmer crosses with one item that's on the same side
    for i in (1,2,3):
        if state[i] == farmer_side:
            new_state = list(state)
            new_state[0] = not farmer_side
            new_state[i] = not farmer_side
            if is_safe(tuple(new_state)):
                results.append(tuple(new_state))
    return results

def bfs(start=(False,False,False,False), goal=(True,True,True,True)):
    q = deque([start])
    parent = {start: None}
    while q:
        s = q.popleft()
        if s == goal:
            path = []
            while s:
                path.append(s)
                s = parent[s]
            return list(reversed(path))
        for n in neighbors(s):
            if n not in parent:
                parent[n] = s
                q.append(n)
    return None

def describe_move(a, b):
    moved = []
    for i in range(4):
        if a[i] != b[i]:
            moved.append(ITEMS[i])
    return " & ".join(moved) or "no move"

def print_state(s):
    left = [ITEMS[i] for i in range(4) if not s[i]]
    right = [ITEMS[i] for i in range(4) if s[i]]
    print(f"Left : {', '.join(left) if left else '—'}")
    print(f"Right: {', '.join(right) if right else '—'}")
    print("-" * 30)

def animate(path):
    for i, s in enumerate(path):
        print(f"Step {i}:")
        print_state(s)
        if i+1 < len(path):
            print("Move:", describe_move(s, path[i+1]))
            input("Press Enter for next move...")

if __name__ == "__main__":
    path = bfs()
    if not path:
        print("No solution found.")
    else:
        print(f"Found solution in {len(path)-1} moves.\n")
        animate(path)