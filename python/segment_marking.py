
# Tile states
EMPTY = '0'
BLOCK = 'B'
MARKED = 'M'

# Segment
all_segments = []

def display_grid(grid):
    for row in grid:
        for col in row:
            print(col, end="")
        print()

def searchAdjacent(grid, row, col, segment):
    visited = [row, col] in segment
    if grid[row][col] == EMPTY or visited:
        return

    segment.append([row, col])
    
    searchAdjacent(grid, row, col-1, segment) # Search left
    searchAdjacent(grid, row, col+1, segment) # Search right
    searchAdjacent(grid, row-1, col, segment) # Search up
    searchAdjacent(grid, row+1, col, segment) # Search down
        

GRID_HEIGHT = 20
GRID_WIDTH = 10
grid = [ [EMPTY for x in range(GRID_WIDTH)] for _ in range(GRID_HEIGHT) ]

grid[2][3] = BLOCK
grid[2][4] = BLOCK
grid[1][3] = BLOCK

grid[3][8] = BLOCK
grid[2][8] = BLOCK
grid[1][8] = BLOCK

grid[5][1] = BLOCK
grid[5][2] = BLOCK
grid[5][3] = BLOCK
grid[6][3] = BLOCK

# Search for segments
for row in range(GRID_HEIGHT):
    for col in range(GRID_WIDTH):

        # Skip this tile if it is already in all_segments
        do_skip = False
        for s in all_segments:
            if [row, col] in s:
                do_skip = True
                break
        
        if not do_skip:
            segment = []
            searchAdjacent(grid, row, col, segment)

            if segment:
                all_segments.append(segment)
        else:
            continue

# Display grid (before markers)
display_grid(grid)
print()
print()

# Mark all segments with a distinct number
seg_marker = 1
for segment in all_segments:
    for blocks in segment:
        row = blocks[0]
        col = blocks[1]
        grid[row][col] = seg_marker
    seg_marker += 1 

# Display grid (after markers)
display_grid(grid)
