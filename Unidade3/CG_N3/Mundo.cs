﻿#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    Objeto mundo;
    private char rotuloNovo = '?';
    private Objeto objetoSelecionado = null;

    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;
    private bool isDesenhando;
    List<Ponto4D> pontosPoligonoNovo = new List<Ponto4D>();

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo = new Objeto(null, ref rotuloNovo);
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

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      // #region Objeto: polígono qualquer  
      // List<Ponto4D> pontosPoligonoBandeira = new List<Ponto4D>();
      // pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.25));
      // pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.25));
      // pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.75));
      // pontosPoligonoBandeira.Add(new Ponto4D(0.50, 0.50));
      // pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.75));
      // objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoBandeira);
      // #endregion
      // #region declara um objeto filho ao polígono
      // List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
      // pontosPoligonoTriangulo.Add(new Ponto4D(0.50, 0.50));
      // pontosPoligonoTriangulo.Add(new Ponto4D(0.75, 0.75));
      // pontosPoligonoTriangulo.Add(new Ponto4D(0.25, 0.75));
      // objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloNovo, pontosPoligonoTriangulo);
      // #endregion
#if CG_Privado
      #region declara um objeto neto ao polígono
      objetoSelecionado = new Circulo(objetoSelecionado, ref rotuloNovo, 0.05, new Ponto4D(0.50, 0.50));
      objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
      #endregion
#endif

      #region Objeto: retângulo  
      // objetoSelecionado = new Retangulo(mundo, ref rotuloNovo, new Ponto4D(-0.25, 0.25), new Ponto4D(-0.75, 0.75));
      // objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
      #endregion

      // #region Objeto: segmento de reta  
      // objetoSelecionado = new SegReta(mundo, ref rotuloNovo, new Ponto4D(-0.5, -0.5), new Ponto4D());
      // #endregion

      // #region Objeto: ponto  
      // objetoSelecionado = new Ponto(mundo, ref rotuloNovo, new Ponto4D(-0.25, -0.25));
      // objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
      // objetoSelecionado.PrimitivaTamanho = 5;
      // #endregion

#if CG_Privado
      #region Objeto: circulo  
      objetoSelecionado = new Circulo(mundo, ref rotuloNovo, 0.2, new Ponto4D());
      #endregion

      #region Objeto: SrPalito  
      objetoSelecionado = new SrPalito(mundo, ref rotuloNovo);
      #endregion

      #region Objeto: Spline
      objetoSelecionado = new Spline(mundo, ref rotuloNovo);
      #endregion
#endif

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo      
      Gizmo_Sru3D();
      Gizmo_BBox();
#endif
      mundo.Desenhar(new Transformacao4D());
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var input = KeyboardState;
      if (input.IsKeyDown(Keys.Escape))
        Close();
      if (input.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
        // objetoSelecionado.shaderObjeto = _shaderBranca;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
        // objetoSelecionado.shaderObjeto = _shaderAmarela;
      }

      if (input.IsKeyPressed(Keys.D)){
        mundo.FilhoRemover(objetoSelecionado);
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
      }

      if (input.IsKeyPressed(Keys.G))
        mundo.GrafocenaImprimir("");
      // Colocando linha amarela
      if (input.IsKeyPressed(Keys.P) && objetoSelecionado != null) {

        if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineStrip) {
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
        } else {
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
        }
        System.Console.WriteLine(objetoSelecionado.ToString());
      }

      if (input.IsKeyPressed(Keys.R) && objetoSelecionado != null) {
        objetoSelecionado.shaderObjeto = _shaderVermelha;
      }
      if (input.IsKeyPressed(Keys.G) && objetoSelecionado != null) {
        objetoSelecionado.shaderObjeto = _shaderVerde;
      }
      if (input.IsKeyPressed(Keys.B) && objetoSelecionado != null) {
        objetoSelecionado.shaderObjeto = _shaderAzul;
      }

      if (input.IsKeyPressed(Keys.E) && objetoSelecionado != null){
        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        List<Ponto4D> listaPontos = objetoSelecionado.GetPontosObjeto();
        Ponto4D pontoProximo = listaPontos[0];

        double distancia = Math.Sqrt((Math.Pow((pontoProximo.X - sruPonto.X),2)) + (Math.Pow((pontoProximo.Y - sruPonto.Y),2)));
        int id_ponto = 0;
        for (int i = 0; i < listaPontos.Count; i++){
          double calcTemp = Math.Sqrt((Math.Pow((listaPontos[i].X - sruPonto.X),2)) + (Math.Pow((listaPontos[i].Y - sruPonto.Y),2)));
          if (calcTemp < distancia){
            distancia = calcTemp;
            id_ponto = i;
          }
        }

        objetoSelecionado.PontosRemover(id_ponto);
      }

      if (input.IsKeyPressed(Keys.V) && objetoSelecionado != null) {
        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        List<Ponto4D> listaPontos = objetoSelecionado.GetPontosObjeto();
        Ponto4D pontoProximo = listaPontos[0];

        double distancia = Math.Sqrt((Math.Pow((pontoProximo.X - sruPonto.X),2)) + (Math.Pow((pontoProximo.Y - sruPonto.Y),2)));
        int id_ponto = 0;
        for (int i = 0; i < listaPontos.Count; i++){
          double calcTemp = Math.Sqrt((Math.Pow((listaPontos[i].X - sruPonto.X),2)) + (Math.Pow((listaPontos[i].Y - sruPonto.Y),2)));
          if (calcTemp < distancia){
            distancia = calcTemp;
            id_ponto = i;
          }
        }
        objetoSelecionado.PontosAlterar(sruPonto, id_ponto);
      }

      if (input.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      //TODO: não está atualizando a BBox com as transformações geométricas
      if (input.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (input.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (input.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (input.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (input.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      if (input.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      if (input.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (input.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      if (input.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      if (input.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(10);
      if (input.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(-10);
      if (input.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(10);
      if (input.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(-10);
      #endregion

      #region  Mouse

      if (MouseState.IsButtonPressed(MouseButton.Left)) {
        
            int janelaLargura = Size.X;
            int janelaAltura = Size.Y;
            Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
            Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
            Objeto verificaObj= mundo.VerificaBBox(sruPonto);
            objetoSelecionado=verificaObj;

        
            if (objetoSelecionado==null) {
            
              Console.WriteLine("A");   
            }
            else 
              {
              bool dentro =verificaObj.ScanLine(sruPonto); 
              Console.WriteLine(dentro);
              if (dentro==false)
                {
              objetoSelecionado= null;
                Console.WriteLine("b");
                }
                else 
                {
                  objetoSelecionado=verificaObj;
                  

                  objetoSelecionado.Bbox();
                  
                                
                Console.WriteLine("c");
                }
              }
        }


      // if (MouseState.IsButtonReleased(MouseButton.Right)) {
      //   // int janelaLargura = Size.X;
      //   // int janelaAltura = Size.Y;
      //   // Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
      //   // Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
      //   // pontosPoligonoNovo.Add(new Ponto4D(-0.25, 0.25));  
      //   // pontosPoligonoNovo.Add(new Ponto4D(-0.75, 0.75));

      //   // if (!isDesenhando) {
      //   //   objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoNovo);
      //   //   isDesenhando = true;
      //   // }
      //   // else {
      //   //   objetoSelecionado.PontosAdicionar(sruPonto);
      //   // }
      // }
      if (input.IsKeyPressed(Keys.Enter)) {
          isDesenhando = false;
          objetoSelecionado = null;
          // objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoNovo);
          pontosPoligonoNovo = new List<Ponto4D>();
      }

      if (MouseState.IsButtonPressed(MouseButton.Right)) {
        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
        pontosPoligonoNovo.Add(sruPonto);  
        pontosPoligonoNovo.Add(sruPonto);

        if (!isDesenhando && objetoSelecionado == null) {
          objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoNovo);
          isDesenhando = true;
        }
        else if (!isDesenhando && objetoSelecionado != null)
        {
          var objeto = new Poligono(objetoSelecionado, ref rotuloNovo, pontosPoligonoNovo);
          objetoSelecionado = objeto;
          isDesenhando = true;
        }

        // if (pontosPoligonoNovo.Count > 0) {
        //   objetoSelecionado.PontosAlterar(sruPonto, pontosPoligonoNovo.Count - 1);
        //   Console.WriteLine("Tetse");
        // }
      }

      if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
      {
        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
        
        objetoSelecionado.PontosAlterar(sruPonto, 0);
      }

      
      // if (MouseState.IsButtonReleased(MouseButton.Right))
      // {
      //   System.Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
      // }

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

      GL.DeleteBuffer(_vertexBufferObject_bbox);
      GL.DeleteVertexArray(_vertexArrayObject_bbox);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()   //FIXME: não é atualizada com as transformações globais
    {
      if (objetoSelecionado != null)
      {

#if CG_OpenGL && !CG_DirectX

        float[] _bbox =
        {
        (float) objetoSelecionado.Bbox().obterMenorX, (float) objetoSelecionado.Bbox().obterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().obterMaiorX, (float) objetoSelecionado.Bbox().obterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().obterMaiorX, (float) objetoSelecionado.Bbox().obterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().obterMenorX, (float) objetoSelecionado.Bbox().obterMaiorY, 0.0f  // D
      };

        _vertexBufferObject_bbox = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
        GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
        _vertexArrayObject_bbox = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject_bbox);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        var transform = Matrix4.Identity;
        GL.BindVertexArray(_vertexArrayObject_bbox);
        _shaderAmarela.SetMatrix4("transform", transform);
        _shaderAmarela.Use();
        GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      }
    }
#endif    

  }
}
