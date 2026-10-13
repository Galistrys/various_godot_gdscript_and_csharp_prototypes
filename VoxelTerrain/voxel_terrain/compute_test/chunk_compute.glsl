#[compute]
#version 450

const int WIDTH = 16;
const int HEIGHT = 16;
const int DEPTH = 16;

const int MAX_BLOCKS_COUNT = WIDTH * HEIGHT * DEPTH;
const int MAX_VERTICIES_COUNT = 4 * 6 * MAX_BLOCKS_COUNT;
const int MAX_INDICES_COUNT = 6 * 6 * MAX_BLOCKS_COUNT;

layout(local_size_x = 8, local_size_y = 8, local_size_z = 8) in;

layout(set = 0, binding = 0, std430) restrict writeonly buffer VerticesBuffer {
    vec3 data[];
} vertices_buffer;

layout(set = 0, binding = 1, std430) restrict writeonly buffer NormalsBuffer {
    vec3 data[];
} normals_buffer;

layout(set = 0, binding = 2, std430) restrict writeonly buffer UvsBuffer {
    vec2 data[];
} uvs_buffer;

layout(set = 0, binding = 3, std430) restrict writeonly buffer TrianglesBuffer {
    int data[];
} triangles_buffer;

layout(set = 0, binding = 4, std430) restrict readonly buffer NeighborsBuffer {
    int data[];
} neighbors_buffer;

void main() {
    // my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}