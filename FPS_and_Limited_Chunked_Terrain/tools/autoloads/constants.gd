extends Node

const CHUNK_SIZE = 256
const CHUNK_SIZE_P_1 = CHUNK_SIZE + 1
const CHUNK_SIZE_P_1_F = float(CHUNK_SIZE_P_1)
const CHUNK_SIZE_P_1_N = CHUNK_SIZE_P_1 + 2

const MAP_SIZE = 2048
const HEIGHT_MULTIPLIER = 32.0

const MAX_LOD = 6

const PLAYER_STATE_IDLE = "idle"
