from enum import Enum


AIR = 0
BLOCK = 1
PLACED = 2
GRID_WIDTH = 10
GRID_HEIGHT = 20


class Rotation(Enum):
    LEFT=0
    TOP=1
    RIGHT=2
    BOTTOM=3