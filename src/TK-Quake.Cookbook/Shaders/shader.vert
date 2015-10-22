#version 330 core

// In from world
in vec3 position;
in vec3 color;
in vec2 texcoord;
in vec2 lightmapcoord;
in vec3 normal;

// Out to Fragment Shader
out vec3 Color;
out vec2 TexCoord;
out vec2 LightMapCoord;
out vec3 ViewRay;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
	Color = color;
	TexCoord = texcoord;
	LightMapCoord = lightmapcoord;
    gl_Position = proj * view * model * vec4(position, 1.0);
	ViewRay = gl_Position.xyz;
}
