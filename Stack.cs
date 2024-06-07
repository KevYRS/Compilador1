using System;
using System.IO;

namespace sintaxis3
{
    public class Stack
    {
        int maxElementos;
        int ultimo;
        float[] elementos;

        public Stack(int maxElementos)
        {
            this.maxElementos = maxElementos;
            ultimo = 0;
            elementos = new float[maxElementos];
        }

        public void push(float element, StreamWriter bitacora, int linea, int caracter)
        {
            if (ultimo < maxElementos)
            {
                bitacora.WriteLine("PUSH= " + element);
                elementos[ultimo++] = element;
            }
            else //else levantar excepción de stack overflow
            {
                throw new Error(bitacora,"Hay overflow en linea: " + linea + " ,en el caracter " + caracter);
                //throw new Exception("Hay overflow en linea: "+linea+" , "+caracter);
            }
        }

        public float pop(StreamWriter bitacora, int linea, int caracter)
        {
            if (ultimo > 0)
            {
                bitacora.WriteLine("POP= " + elementos[ultimo - 1]);
                return elementos[--ultimo];
            }
            else //else levantar excepción de stack underflow
            {
                throw new Error(bitacora, "Hay underflow en linea: " + linea + " ,en el caracter " + caracter);
                //throw new Exception("Hay underflow "+linea+" , "+caracter);
            }
        }

        public void display(StreamWriter bitacora)
        {
            bitacora.WriteLine("Contenido del stack: ");
            for (int i = 0; i < ultimo; i++)
            {
                bitacora.Write(elementos[i] + " ");
            }
            bitacora.WriteLine("");
        }
    }
}