def display_grid(grid):
    for row in grid:
        for i in range(len(row)):
            print(row[i], end="")
        print()
    
    print("\n========")