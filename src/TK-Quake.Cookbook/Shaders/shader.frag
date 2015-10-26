#version 330 core

in vec3 Color;
in vec2 TexCoord;
in vec2 LightMapCoord;

uniform sampler2D Texture;
uniform sampler2D LightMap;

out vec4 outColor;

void main()
{
    vec4 text = texture(Texture, TexCoord);
    vec4 lm   = texture(LightMap, LightMapCoord);

	// Tyler made this change because it interferes with particles
	//outColor  = text * (lm + vec4(0.5, 0.5, 0.5, 0.5));
	outColor  = text * (lm + vec4(0, 0, 0, 0));
	
	// Test texure pixel alpha value
	if (outColor.a < 0.5)
		discard;

}