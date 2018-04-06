#version 430 compatibility

layout(row_major) uniform Matrices
{
    mat4 model;
    mat4 proj;
    mat4 view;
};

in vec4 pos;

out float w;

void main()
{
    gl_Position = vec4(pos.xyz, 1) * model * view * proj;
    w = (1 + pos.w) / 2;
}