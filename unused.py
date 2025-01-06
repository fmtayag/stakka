def move_T(grid, pos, move):
    # TODO: Add 'rotation' state (as it is, this only checks movement validity for one rotation state)
    # TODO: Add adjacency check

    # - Set the origin point
    ox = pos[0] # Yes, this is intentional
    oy = pos[1]
    newx = ox+move[0]
    newy = oy+move[1]

    # def is_blocked(grid): # Recursive adjacency check
    #     # Check if new position will go outside grid
    #     outside_grid = (
    #         newx < 0 or newx > GRID_WIDTH-1 or # Horizontal check
    #         newy > GRID_HEIGHT-1
    #     )
    #     collides_block = (
    #         grid[newy][newx] == PLACED or # ORIGIN
    #         grid[newy+1][newx  ] == PLACED or
    #         grid[newy  ][newx-1] == PLACED or
    #         grid[newy  ][newx+1] == PLACED
    #     )

    #     if outside_grid or collides_block:
    #         return True
    #     return False

    # if is_blocked(grid):
    if False:
        # Is blocked by walls, return original grid
        return grid, pos
    else:
        # # Empty the current tiles ### NO LONGER NECESSARY AFTER ADDING REFRESH GRID
        # grid[oy][ox] = AIR # Origin block
        # grid[oy+1][ox  ] = AIR
        # grid[oy  ][ox-1] = AIR
        # grid[oy  ][ox+1] = AIR

        # -- Populate the current tiles
        grid[newy  ][newx  ] = BLOCK # Origin block
        grid[newy+1][newx  ] = BLOCK
        grid[newy  ][newx-1] = BLOCK
        grid[newy  ][newx+1] = BLOCK

    new_pos = [pos[0]+move[0], pos[1]+move[1]]
    return grid, new_pos
