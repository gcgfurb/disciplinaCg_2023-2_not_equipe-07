using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg 
{
    class Circulo : Objeto 
    {
        public Circulo(Objeto paiRef, ref char _rotulo, Ponto4D ptoCentro, double raio) : base(paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 5;
            double angulo = Matematica.GerarPtosCirculoSimetrico(raio);
            for (int i = 0; i < 72; i++)
            {
                base.PontosAdicionar(Matematica.GerarPtosCirculo(angulo, raio));
                angulo += 35;
            }
            base.ObjetoAtualizar();
        }
    }
}