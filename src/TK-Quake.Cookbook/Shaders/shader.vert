#version 330 core

in vec3 position;
in vec3 color;
in vec2 texcoord;
in vec2 lightmapcoord;
in vec3 normal;

out vec3 Color;
out vec2 TexCoord;
out vec2 LightMapCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
	Color = color;
	TexCoord = texcoord;
	LightMapCoord = lightmapcoord;
    gl_Position = proj * view * model * vec4(position, 1.0);
}
