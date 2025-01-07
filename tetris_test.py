# ⬛⬜

import copy
import pygame, time
from pygame import Vector2 as Vec2

from constants import AIR, BLOCK, GRID_HEIGHT, GRID_WIDTH, PLACED
from tetros import Tetro, TetroCircle, TetroI, TetroJ, TetroL, TetroS, TetroT, TetroZ

pygame.init()

def refresh_grid(grid):
    for row in grid:
        for i in range(len(row)):
            if row[i] != PLACED:
                row[i] = AIR

def display_grid_visual(screen: pygame.Surface, grid):
    gridpos_x = screen.get_width() * 0.4
    gridpos_y = screen.get_height() * 0.2
    y = 0
    for row in grid:

        ver_grid = 0
        for i in range(len(row)):
            surf = pygame.Surface((16, 16))
            if row[i] == AIR:
                surf.fill("BLACK")
            if row[i] == BLOCK:
                surf.fill("WHITE")
            if row[i] == PLACED:
                surf.fill("RED")

            x = (i * 16) + (ver_grid * 4)

            # Blit block
            screen.blit(surf, (x+gridpos_x, y+gridpos_y))

            ver_grid += 1

        y += 18

def dbg_move(grid, tetro: Tetro, move):
    # TODO: Check if tetro is not blocked, if not then return the new grid
    newx = tetro.position[0] + move[0]
    newy = tetro.position[1] + move[1]
    newgrid = copy.deepcopy(grid) # Create copy of grid

    is_blocked = False
    for block in tetro.get_blocks(tetro.rotation):
        bx = block[0]
        by = block[1]

        # -- Check if block in new pos will hit wall
        if (
            newx + bx < 0 or newx + bx > GRID_WIDTH-1 or 
            newy + by < 0 or newy + by > GRID_HEIGHT-1
        ):
            is_blocked = True
            break

        # -- Check if block in new pos will hit already placed blocks
        try:
            if grid[newy+by][newx+bx] == PLACED:
                is_blocked = True
                break
        except:
            # This is a bit hacky but screw it
            is_blocked = True
            break

        # -- Place new block if all is good
        newgrid[newy+by][newx+bx] = BLOCK
        
    
    if not is_blocked:
        tetro.position[0] = newx 
        tetro.position[1] = newy

        return newgrid
    else:
        return grid


screen = pygame.display.set_mode((800, 600))
pygame.display.set_caption("Tetris Clone")
running = True
clock = pygame.time.Clock()
FPS = 60

new_pos = [0, 0]

# -- Create the grid with pre-placed blocks
grid = [ [AIR for i in range(GRID_WIDTH)] for i in range (GRID_HEIGHT) ]
grid[3][6] = PLACED # [2] is y [6] is x
grid[3][1] = PLACED
grid[5][2] = PLACED
grid[5][3] = PLACED

# dbg_tetro = TetroT([1, 4]) # Debug tetro 
dbg_tetro = TetroL([2, 2]) # Debug tetro 

while running:
    clock.tick(FPS)
    new_pos = [0, 0]

    # Get input
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_LEFT:
                new_pos[0] = -1
            elif event.key == pygame.K_RIGHT:
                new_pos[0] = 1
            elif event.key == pygame.K_UP: # TODO: FOR DEBUGGING ONLY
                new_pos[1] = -1 
            elif event.key == pygame.K_DOWN:
                new_pos[1] = 1
            elif event.key == pygame.K_r:
                if type(dbg_tetro) != TetroCircle:
                    dbg_tetro.rotate(grid)
    
    # Process
    refresh_grid(grid)
    grid = dbg_move(grid, dbg_tetro, new_pos)

    # Draw
    screen.fill("BLUE")
    display_grid_visual(screen, grid)

    pygame.display.update()

pygame.quit()
    
