#[compute]

#version 450

const uint CHUNK_SIZE = 16;
const uint CHUNK_SIZE_1 = 17;
const uint CHUNK_SIZE2 = CHUNK_SIZE * CHUNK_SIZE;
const uint CHUNK_SIZE_1_2 = CHUNK_SIZE * CHUNK_SIZE;
const float SURFACE_LEVEL = 0.5;

struct Triangle {
	vec4 vertexA;
	vec4 vertexB;
	vec4 vertexC;
	vec4 normal;
};

uint indexFromCoord(uint x, uint y, uint z) {
	return CHUNK_SIZE_1_2 * z + CHUNK_SIZE_1 * y + x;
}

vec3 interpolateVerts(vec4 v1, vec4 v2) {
	float t = (SURFACE_LEVEL - v1.w) / (v2.w - v1.w);
	return v1.xyz + t * (v2.xyz - v1.xyz);
}

layout(local_size_x = 1, local_size_y = 1, local_size_z = 1) in;

layout(set = 0, binding = 0, std430) buffer TrianglesBuffer {
	Triangle triangles[];
};

layout(set = 0, binding = 1, std430) buffer PointsBuffer {
	float points[CHUNK_SIZE_1 * CHUNK_SIZE_1 * CHUNK_SIZE_1];
};

void main() {
	// color_buffer.data[gl_GlobalInvocationID.x] = gl_GlobalInvocationID.x;
	uvec3 p_index = gl_GlobalInvocationID.xyz;
	uint t_index = (CHUNK_SIZE2 * p_index.z + CHUNK_SIZE * p_index.y + p_index.x) * 5;

	float point = points[indexFromCoord(p_index.x, p_index.y, p_index.z)];

	uvec3 nwg = gl_NumWorkGroups.xyz;
	uvec3 wgi = gl_WorkGroupID.xyz;
	uvec3 gii = gl_GlobalInvocationID.xyz;
	uvec3 lii = gl_LocalInvocationID.xyz;

	Triangle tri;
	tri.vertexA = vec4(float(p_index.x), float(p_index.y), float(p_index.z), -1.0);
	tri.vertexB = vec4(float(p_index.x + 1), float(p_index.y), float(p_index.z), -1.0);
	tri.vertexC = vec4(float(p_index.x + 1), float(p_index.y), float(p_index.z + 2), -1.0);
	tri.normal  = vec4(float(point), float(point), float(point), -1.0);
	
	// Triangle tri;
	// tri.vertexA = vec4(float(nwg.x), float(nwg.y), float(nwg.z), -1.0);
	// tri.vertexB = vec4(float(wgi.x), float(wgi.y), float(wgi.z), -1.0);
	// tri.vertexC = vec4(float(gii.x), float(gii.y), float(gii.z), -1.0);
	// tri.normal  = vec4(float(lii.x), float(lii.y), float(lii.z), -1.0);
	
	triangles[t_index + 0] = tri;
	triangles[t_index + 1] = tri;
	triangles[t_index + 2] = tri;
	triangles[t_index + 3] = tri;
	triangles[t_index + 4] = tri;
}