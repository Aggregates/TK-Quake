#version 330 core

uniform sampler2D uTexLinearDepth;
uniform sampler2D uTexInput;

uniform mat4 uInverseModelViewMat; // inverse model->view
uniform mat4 uPrevModelViewProj; // previous model->view->projection

noperspective in vec2 vTexcoord;
noperspective in vec3 vViewRay; // for extracting current world space position

out result;

void main() {

	//http://john-chapman-graphics.blogspot.com/2013/01/what-is-motion-blur-motion-pictures-are.html

	// get current world space position:
	vec3 current = vViewRay * texture(uTexLinearDepth, vTexcoord).r;
	current = uInverseModelViewMat * current;

	// get previous screen space position:
	vec4 previous = uPrevModelViewProj * vec4(current, 1.0);
	previous.xyz /= previous.w;
	previous.xy = previous.xy * 0.5 + 0.5;

	vec2 blurVec = previous.xy - vTexcoord;

	// perform blur:
	float nSamples = 20;
	result = texture(uTexInput, vTexcoord);
	for (int i = 1; i < nSamples; ++i) {
		// get offset in range [-0.5, 0.5]:
		vec2 offset = blurVec * (float(i) / float(nSamples - 1) - 0.5);

		// sample & add to result:
		result += texture(uTexInput, vTexcoord + offset);
	}

	result /= float(nSamples);
}