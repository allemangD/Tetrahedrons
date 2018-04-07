#version 430 compatibility

layout(row_major) uniform Matrices
{
    mat4 model;
    mat4 view;
    mat4 proj;
};

in vec4 pos;

out vec4 scol;

void main()
{
    gl_Position = vec4(pos.xyz, 1) * model * view * proj;
    scol = vec4(pos.xyz / 5 + .5, 1);
}