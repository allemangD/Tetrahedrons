#version 430

uniform ViewMats
{
    mat4 model;    
    mat4 view;    
    mat4 proj;    
};

in vec4 pos;

void main () {
    gl_Position = proj * view * model * pos;
}