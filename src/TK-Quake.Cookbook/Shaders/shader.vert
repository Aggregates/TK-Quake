#version 330 core

in vec3 position;
in vec3 color;
in vec2 texcoord;
in vec3 normal;		// Do I even have normal registered?		

out vec3 fragColor;
out vec2 fragTexCoord;
out vec3 fragNormal;
out vec3 fragVert;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
	fragColor = color;
	fragTexCoord = texcoord;
	fragNormal = normal;
	fragVert = position;
    gl_Position = proj * view * model * vec4(position, 1.0);
}
