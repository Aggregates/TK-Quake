#version 330 core

// From http://john-chapman-graphics.blogspot.com/2013/01/what-is-motion-blur-motion-pictures-are.html

uniform sampler2D uTexLinearDepth;
uniform sampler2D outTexture;

// Registered and updated with C# Code
uniform mat4 uInverseModelViewMat; // Inverse model->view
uniform mat4 uPrevModelViewProj; // Previous model->view->projection

// Inputs from Vertex Shader
in vec2 TexCoord;
in vec3 ViewRay; // Used to extracting current world space position

// Output to next process
out vec4 outColor;

void main() {

	// Get current world space position
	vec3 current = ViewRay * texture2D(uTexLinearDepth, TexCoord).r;
	
	// Get the first column of the matrix
	mat4 m = uInverseModelViewMat;
	current = vec3(m[0][0], m[1][0], m[2][0]) * current;

	// Get previous screen space position
	vec4 previous = uPrevModelViewProj * vec4(current, 1.0);
	previous.xyz /= previous.w;
	previous.xy = previous.xy * 0.5 + 0.5;

	// Calculate the direction of the blur vector
	vec2 blurVec = previous.xy - TexCoord;

	// Perform blur
	vec4 result = texture(outTexture, TexCoord);
	
	// Testing with 2 samples
	int nSamples = 2;
	for (int i = 1; i < nSamples; ++i) {
		// Get offset in range [-0.5, 0.5]
		vec2 offset = blurVec * (float(i) / float(nSamples - 1) - 0.5);

		// Sample from the texture and add to result
		result += texture(outTexture, TexCoord + offset);
	}

	// Get the average of the result colour
	result /= float(nSamples);
	outColor = result;

}