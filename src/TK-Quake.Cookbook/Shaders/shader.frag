#version 330 core

//in vec3 Color;
//in vec2 TexCoord;
//in vec3 Normal;
//in vec3 Vert;

//uniform sampler2D outTexture;

//out vec4 outColor;

//void main()
//{
//    outColor = texture(outTexture, TexCoord);
//}

uniform mat4 model;
uniform sampler2D outTexture;

// uniform struct Light
uniform struct Light {
   vec3 position;
   vec3 intensities; //a.k.a the color of the light
} light;

in vec3 fragColor;
in vec2 fragTexCoord;
in vec3 fragNormal;
in vec3 fragVert;

out vec4 outColor;

void main() {
	//light.position = vec3(0,0,0);
	//light.intensities = vec3(1.0,1.0,1.0);

    //calculate normal in world coordinates
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 normal = normalize(normalMatrix * fragNormal);
    
    //calculate the location of this fragment (pixel) in world coordinates
    vec3 fragPosition = vec3(model * vec4(fragVert, 1));
    
    //calculate the vector from this pixels surface to the light source
    vec3 surfaceToLight = light.position - fragPosition;

    //calculate the cosine of the angle of incidence
    float brightness = dot(normal, surfaceToLight) / (length(surfaceToLight) * length(normal));
    brightness = clamp(brightness, 0, 1);

    //calculate final color of the pixel, based on:
    // 1. The angle of incidence: brightness
    // 2. The color/intensities of the light: light.intensities
    // 3. The texture and texture coord: texture(outTexture, fragTexCoord)
    vec4 surfaceColor = texture(outTexture, fragTexCoord);
    outColor = vec4(brightness * light.intensities * surfaceColor.rgb, surfaceColor.a);
}
