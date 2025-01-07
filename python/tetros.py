
import copy
from constants import BLOCK, GRID_HEIGHT, GRID_WIDTH, PLACED, Rotation
from grid_display import display_grid

class Tetro:
    def __init__(self, pos, rotation):
        self.position = pos 
        self.rotation = rotation

    def get_blocks(self):
        pass # Abstract method

    def rotate(self, grid):
        # -- 1. Set the new rotation
        new_rot = None
        match self.rotation:
            case Rotation.LEFT:
                new_rot = Rotation.TOP
            case Rotation.TOP:
                new_rot = Rotation.RIGHT
            case Rotation.RIGHT:
                new_rot = Rotation.BOTTOM
            case Rotation.BOTTOM:
                new_rot = Rotation.LEFT

        rotated_blocks = self.get_blocks(new_rot)
        is_blocked = False
        wallkick_dir = 0
        
        # 2. -- Check if the rotated blocks will hit a wall or a placed block
        for block in rotated_blocks:
            bx = self.position[0] + block[0]
            by = self.position[1] + block[1]
            # -- Check if block in new pos will hit wall
            if (
                bx < 0 or bx > GRID_WIDTH-1 or
                by < 0 or by > GRID_HEIGHT-1
            ):
                is_blocked = True

            # -- Check if block in new pos will hit already placed blocks
            within_vergrid = by > 0 and by < GRID_HEIGHT- 1
            within_horgrid = bx > 0 and bx < GRID_WIDTH - 1
            if (within_vergrid) and (within_horgrid):
                if grid[by][bx] == PLACED:
                    is_blocked = True
            else:
                is_blocked = True
                break
            
        # -- 3. Try Wall kick to right
        if is_blocked:
            # # -- DEBUG 1 --
            # newgrid = copy.deepcopy(grid)

            for kick in range(1, 3):
                for block in rotated_blocks:
                    wallk_right = self.position[0] + block[0] + kick
                    by = self.position[1] + block[1]

                    # -- DEBUG 1 --
                    # newgrid[by][wallk_right] = 3
                    # display_grid(newgrid)

                    # Check if block in rotated pos will collide with the ceiling
                    if by < 0:
                        is_blocked = True
                        break
                    
                    if wallk_right > 0 and wallk_right <= GRID_WIDTH-1:
                        if grid[by][wallk_right] == PLACED and by > 0:
                            wallkick_dir = 0
                            is_blocked = True
                            break
                        else:
                            wallkick_dir = kick
                            is_blocked = False
                    else:
                        is_blocked = True
                        break
                if not is_blocked:
                    break

        # -- 4. If still blocked, wall kick to left
        if is_blocked:
            # print("Still blocked, kicking to left...")
            
            for kick in range(1, 3):
                for block in rotated_blocks:
                    wallk_left = self.position[0] + block[0] - kick
                    by = self.position[1] + block[1]

                    # Check if block in rotated pos will collide with the ceiling
                    if by < 0:
                        is_blocked = True
                        break
                                
                    if wallk_left > 0 and wallk_left <= GRID_WIDTH-1:
                        if grid[by][wallk_left] == PLACED:
                            wallkick_dir = 0
                            is_blocked = True
                            break
                        else:
                            wallkick_dir = -kick
                            is_blocked = False
                    else:
                        wallkick_dir = 0
                        is_blocked = True 
                        break
                if not is_blocked:
                    break

        # -- 5. Set current rotation to new rotation if all is good
        if not is_blocked:
            # print("Not blocked...rotating")
            self.rotation = new_rot

            if wallkick_dir != 0 :
                self.position[0] += wallkick_dir
                print(self.position)

class TetroJ(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if rotation == Rotation.LEFT:
            return [
                [0, 0], 
                [-1, 0],
                [-2, 0],
                [0, 1]
            ]
        elif rotation == Rotation.TOP:
            return [
                [-1, 1],
                [-1, 0],
                [-1, -1],
                [-2, 1]
            ]
        elif rotation == Rotation.RIGHT:
            return [
                [-2, 1],
                [-1, 1],
                [0, 1],
                [-2, 0]
            ]
        elif rotation == Rotation.BOTTOM:
            return [
                [-1, -1], 
                [-1, 0],
                [-1, 1],
                [0, -1]
            ]
        
class TetroL(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if rotation == Rotation.LEFT:
            return [
                [0, 0], 
                [1, 0],
                [2, 0],
                [0, 1]
            ]
        elif rotation == Rotation.TOP:
            return [
                [0, -1],
                [1, -1],
                [1, 0],
                [1, 1]
            ]
        elif rotation == Rotation.RIGHT:
            return [
                [0, 1],
                [1, 1],
                [2, 1],
                [2, 0]
            ]
        elif rotation == Rotation.BOTTOM:
            return [
                [1, -1],
                [1, 0],
                [1, 1],
                [2, 1]
            ]

class TetroT(Tetro):
    def __init__(self, pos, rotation=Rotation.BOTTOM):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if rotation == Rotation.LEFT:
            return [
                [0, 0], 
                [-1, 0],
                [0, -1],
                [0, 1]
            ]
        elif rotation == Rotation.TOP:
            return [
                [0, 0], 
                [0, 1],
                [1, 1],
                [-1, 1]
            ]
        elif rotation == Rotation.RIGHT:
            return [
                [0, 0],
                [1, 0],
                [0, -1],
                [0, 1]
            ]
        elif rotation == Rotation.BOTTOM:
            return [
                [0, 0], 
                [0, 1],
                [1, 0],
                [-1, 0]
            ]
        
class TetroS(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if rotation == Rotation.LEFT or rotation == Rotation.RIGHT:
            return [
                [0, 0], 
                [1, 0],
                [0, 1],
                [-1, 1]
            ]
        elif rotation == Rotation.TOP or rotation == Rotation.BOTTOM:
            return [
                [0, 0],
                [-1, 0],
                [-1, -1],
                [0, 1]
            ]
        
class TetroZ(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if rotation == Rotation.LEFT or rotation == Rotation.RIGHT:
            return [
                [0, 0],
                [-1, 0],
                [0, 1],
                [1, 1],
            ]
        elif rotation == Rotation.TOP or rotation == Rotation.BOTTOM:
            return [
                [0, 0],
                [1, 0],
                [1, -1],
                [0, 1],
            ]

class TetroI(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if rotation == Rotation.LEFT or rotation == Rotation.RIGHT:
            return [
                [0, 0], 
                [1, 0],
                [-1, 0],
                [-2, 0]
            ]
        elif rotation == Rotation.TOP or rotation == Rotation.BOTTOM:
            return [
                [-1, -1],
                [-1, 0],
                [-1, 1],
                [-1, 2]
            ]

class TetroCircle(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)

    def get_blocks(self, rotation: Rotation):
        if (
            rotation == Rotation.LEFT or rotation == Rotation.RIGHT or 
            rotation == Rotation.TOP or rotation == rotation.BOTTOM
        ):
            return [
                [0, 0], 
                [1, 0],
                [1, 1],
                [0, 1]
            ]