#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

//FIXME: padrão Singleton

namespace gcgcg
{
      public class Mundo : GameWindow
      {
            Objeto mundo;
            private char rotuloAtual = '?';
            private Objeto objetoSelecionado = null;

            private readonly float[] _sruEixos =
            {
      -0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.0f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
    };

            private int _vertexBufferObject_sruEixos;
            private int _vertexArrayObject_sruEixos;

            private Shader _shaderVermelha;
            private Shader _shaderVerde;
            private Shader _shaderAzul;

            private bool _firstMove = true;
            private Vector2 _lastPos;

            private int _totalEspaco = 1;

            private Ponto pontoSpline1;
            private Ponto pontoSpline2;
            private Ponto pontoSpline3;
            private Ponto pontoSpline4;
            private Ponto pontoSplineSelecionado;
            private Spline spline;

            public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
                   : base(gameWindowSettings, nativeWindowSettings)
            {
                  mundo = new Objeto(null, ref rotuloAtual);
            }

            private void DesenharSpline()
            {
                  Ponto4D ponto1 = new Ponto4D(-0.5, -0.5);
                  Ponto4D ponto2 = new Ponto4D(-0.5, 0.5);
                  Ponto4D ponto3 = new Ponto4D(0.5, 0.5);
                  Ponto4D ponto4 = new Ponto4D(0.5, -0.5);

                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  pontoSpline1 = new Ponto(mundo, ref rotuloAtual, ponto1);
                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  pontoSpline2 = new Ponto(mundo, ref rotuloAtual, ponto2);
                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  pontoSpline3 = new Ponto(mundo, ref rotuloAtual, ponto3);
                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  pontoSpline4 = new Ponto(mundo, ref rotuloAtual, ponto4);

                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  new SegReta(mundo, ref rotuloAtual, ponto1, ponto2);

                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  new SegReta(mundo, ref rotuloAtual, ponto2, ponto3);

                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  new SegReta(mundo, ref rotuloAtual, ponto3, ponto4);

                  rotuloAtual = Utilitario.CharProximo(rotuloAtual);
                  spline = new Spline(mundo, ref rotuloAtual);
                  spline.SplineQtdPto(10);
                  var listaPontos = new List<Ponto4D>();
                  listaPontos.Add(ponto1);
                  listaPontos.Add(ponto2);
                  listaPontos.Add(ponto3);
                  listaPontos.Add(ponto4);
                  spline.setPontos(listaPontos);
                  spline.DesenharObjeto();
            }

            private void SelecionarPonto(Ponto ponto)
            {
                  if (spline == null)
                  {
                        return;
                  }
                  pontoSpline1.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
                  pontoSpline2.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
                  pontoSpline3.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
                  pontoSpline4.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
                  ponto.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
                  pontoSplineSelecionado = ponto;
            }
            
            private void Cima() {
                  //altere o Y do ponto para cima
                  pontoSplineSelecionado.PontosId(0).Y += 0.1;
                  pontoSplineSelecionado.ObjetoAtualizar();

            }

            private void MoverPonto(Ponto ponto) {

            }

            private void Diretivas()
            {
#if DEBUG
      Console.WriteLine("Debug version");
#endif
#if RELEASE
    Console.WriteLine("Release version");
#endif
#if CG_Gizmo
                  Console.WriteLine("#define CG_Gizmo  // debugar gráfico.");
#endif
#if CG_OpenGL
                  Console.WriteLine("#define CG_OpenGL // render OpenGL.");
#endif
#if CG_DirectX
      Console.WriteLine("#define CG_DirectX // render DirectX.");
#endif
#if CG_Privado
      Console.WriteLine("#define CG_Privado // código do professor.");
#endif
                  Console.WriteLine("__________________________________ \n");
            }

            protected override void OnLoad()
            {
                  base.OnLoad();

                  Diretivas();

                  GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);

                  #region Eixos: SRU  
                  _vertexBufferObject_sruEixos = GL.GenBuffer();
                  GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
                  GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
                  _vertexArrayObject_sruEixos = GL.GenVertexArray();
                  GL.BindVertexArray(_vertexArrayObject_sruEixos);
                  GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                  GL.EnableVertexAttribArray(0);
                  _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
                  _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
                  _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
                  #endregion

                  // #region Objeto: Segmento reta  
                  // objetoSelecionado = new SegReta(mundo, ref rotuloAtual, new Ponto4D(0, 0), new Ponto4D(0.25, 0.25));
                  // #endregion
                  DesenharSpline();

#if CG_Privado
      #region Objeto: circulo  
      objetoSelecionado = new Circulo(mundo, ref rotuloAtual, 0.2, new Ponto4D());
      objetoSelecionado.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Objeto: SrPalito  
      objetoSelecionado = new SrPalito(mundo, ref rotuloAtual);
      #endregion

      #region Objeto: Spline
      objetoSelecionado = new Spline(mundo, ref rotuloAtual);
      #endregion
#endif

            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                  base.OnRenderFrame(e);

                  GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo
                  Sru3D();
#endif
                  mundo.Desenhar();
                  SwapBuffers();
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                  base.OnUpdateFrame(e);

                  #region Teclado
                  var input = KeyboardState;
                  if (input.IsKeyDown(Keys.Escape))
                  {
                        Close();
                  }
                  else
                  {
                        if (input.IsKeyDown(Keys.U))
                              SelecionarPonto(pontoSpline1);
                        else if (input.IsKeyDown(Keys.I))
                              SelecionarPonto(pontoSpline2);
                        else if (input.IsKeyDown(Keys.O))
                              SelecionarPonto(pontoSpline3);
                        else if (input.IsKeyDown(Keys.P))
                              SelecionarPonto(pontoSpline4);
                        else if (input.IsKeyDown(Keys.R) && spline != null)
                        {
                              spline.SplineQtdPto(10);
                        }
                        else if (input.IsKeyDown(Keys.C))
                              Cima();
                        // else if (e.Key == Key.B)
                        //       Baixo();
                        // if (input.IsKeyDown(Keys.Space)) {
                        //       if (_totalEspaco == 1) {
                        //             SelecionarPonto(pontoSpline1);
                        //             _totalEspaco++;
                        //       }
                        //       else if (_totalEspaco == 2) {
                        //             SelecionarPonto(pontoSpline2);
                        //             _totalEspaco++;
                        //       }
                        //       else if (_totalEspaco == 3) {
                        //             SelecionarPonto(pontoSpline3);
                        //             _totalEspaco++;
                        //       }
                        //       else if (_totalEspaco == 4) {
                        //             SelecionarPonto(pontoSpline4);
                        //             _totalEspaco = 1;
                        //       }
                        // }
                  }
                  #endregion
            }

            protected override void OnResize(ResizeEventArgs e)
            {
                  base.OnResize(e);

                  GL.Viewport(0, 0, Size.X, Size.Y);
            }

            protected override void OnUnload()
            {
                  mundo.OnUnload();

                  GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                  GL.BindVertexArray(0);
                  GL.UseProgram(0);

                  GL.DeleteBuffer(_vertexBufferObject_sruEixos);
                  GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

                  GL.DeleteProgram(_shaderVermelha.Handle);
                  GL.DeleteProgram(_shaderVerde.Handle);
                  GL.DeleteProgram(_shaderAzul.Handle);

                  base.OnUnload();
            }

#if CG_Gizmo
            private void Sru3D()
            {
#if CG_OpenGL && !CG_DirectX
                  GL.BindVertexArray(_vertexArrayObject_sruEixos);
                  // EixoX
                  _shaderVermelha.Use();
                  GL.DrawArrays(PrimitiveType.Lines, 0, 2);
                  // EixoY
                  _shaderVerde.Use();
                  GL.DrawArrays(PrimitiveType.Lines, 2, 2);
                  // EixoZ
                  _shaderAzul.Use();
                  GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
            }
#endif

      }
}
