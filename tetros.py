
from constants import GRID_HEIGHT, GRID_WIDTH, PLACED, Rotation

class Tetro:
    def __init__(self, pos, rotation):
        self.position = pos 
        self.rotation = rotation

    def rotate(self, grid):
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

        # TODO: allow wall kicks
        blocks = self.get_blocks(new_rot)
        is_blocked = False
        for block in blocks:
            bx = self.position[0] + block[0]
            by = self.position[1] + block[1]
            # -- Check if block in new pos will hit wall
            if (
                bx < 0 or bx > GRID_WIDTH-1 or
                by < 0 or by > GRID_HEIGHT-1
            ):
                is_blocked = True
                break

            # -- Check if block in new pos will hit already placed blocks
            try:
                if grid[by][bx] == PLACED:
                    is_blocked = True
                    break
            except:
                # This is a bit hacky but screw it
                is_blocked = True
                break

        # -- Allow rotate if all is good
        if not is_blocked:
            self.rotation = new_rot

class TetroJ(Tetro):
    def __init__(self, pos, rotation=Rotation.LEFT):
        Tetro.__init__(self, pos, rotation)
        self.position = pos
        self.rotation = rotation

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

class TetroT(Tetro):
    def __init__(self, pos, rotation=Rotation.BOTTOM):
        Tetro.__init__(self, pos, rotation)
        self.position = pos
        self.rotation = rotation

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