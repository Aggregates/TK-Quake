#version 330 core

in vec3 position;
in vec3 color;
in vec3 texcoord;
in vec3 normal;

out vec3 Color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
	Color = color;
    gl_Position = proj * view * model * vec4(position, 1.0);
}