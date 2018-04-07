#version 430 compatibility

uniform vec4 color;

in vec4 scol;

void main()
{
    gl_FragColor = color * scol;
}
