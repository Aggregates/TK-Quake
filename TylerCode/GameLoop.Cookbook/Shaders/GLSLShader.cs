using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Cookbook.Shaders
{
    /// <summary>
    /// Typical Usage:
    /// 1. Create GLSLShader on stack or heap
    /// 2. LoadFromFile the GLSLShader object
    /// 3. Call CreateAndLinkProgram
    /// 4. Call Use() on the shader object to bind the shader object
    /// 5. Call Add* to store locations of attributes and uniforms
    /// 6. Call UnUse to unbind the shader object
    /// </summary>
    public class GLSLShader
    {
        
        private uint _program;
        private int _totalShaders;
        private uint[] _shaders;
        private Dictionary<string, uint> _attributeList;
        private Dictionary<string, uint> _uniformList;

        public GLSLShader()
        {
            _totalShaders = 0;

            _shaders = new uint[3];
            _shaders[(int)ShaderType.VertexShader] = 0;
            _shaders[(int)ShaderType.FragmentShader] = 0;
            _shaders[(int)ShaderType.GeometryShader] = 0;

            _attributeList = new Dictionary<string, uint>();
            _uniformList = new Dictionary<string, uint>();
        }
        public void LoadFromString(GLShaderEnum whichShader, string source) { }
        public void LoadFromFile(GLShaderEnum whichShader, string filename) { }
        public void CreateAndLinkProgram() { }
        public void Use() { }
        public void UnUse() { }
        public void AddAttribute(string attribute) { }
        public void AddUniform(string uniform) { }
        public uint GetAttribute(string attribute) { return 1; }
        public uint GetUniform(string uniform) { return 1; }
        public void DeleteShaderProgram() { }

        
    }
}
