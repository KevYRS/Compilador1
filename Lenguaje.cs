﻿using System;
using System.Collections.Generic;
using System.Text;

/*
Requerimiento 1: Implementar las secuencias de escape: \n, \t cuando se imprime una cadena y 
                 eliminar las dobles comillas.
Requerimiento 2: Levantar excepciones en la clase Stack.
Requerimiento 3: Agregar el tipo de dato en el Inserta de ListaVariables.
Requerimiento 4: Validar existencia o duplicidad de variables. Mensaje de error: 
                 "Error de sintaxis: La variable (x26) no ha sido declarada."
                 "Error de sintaxis: La variables (x26) está duplicada." 
Requerimiento 5: Modificar el valor de la variable o constante al momento de su declaración.
Completados: R2(20/09), R3(20/09), R1(21/09), R4(22/09), R5(22/09)
*/

namespace sintaxis3
{
    class Lenguaje : Sintaxis
    {
        Stack s;
        ListaVariables l;
        public Lenguaje()
        {
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }
        // Programa -> Libreria Main
        public void Programa()
        {
            Libreria();
            Main();
            l.imprime(bitacora);
        }

        // Libreria -> (#include <identificador(.h)?> Libreria) ?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(clasificaciones.identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }
        // Main -> tipoDato main() BloqueInstrucciones 
        private void Main()
        {
            match(clasificaciones.tipoDato);
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
        }
        // BloqueInstrucciones -> { Instrucciones }
        private void BloqueInstrucciones()
        {
            match(clasificaciones.inicioBloque);
            Instrucciones();
            match(clasificaciones.finBloque);
        }

        // Lista_IDs -> identificador (= Expresion)? (,Lista_IDs)? 
        private void Lista_IDs(Variable.tipo Tipos)
        {
            string nombre = getContenido();
            if (!l.Existe(nombre))
            {
                l.Inserta(nombre, Tipos);
                match(clasificaciones.identificador);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Variable duplicada (" + nombre + ") " + "(" + linea + ", " + caracter + ")");
            }

            if (getClasificacion() == clasificaciones.asignacion)
            {
                match(clasificaciones.asignacion);
                string contenidos = getContenido();
                if (getClasificacion() == clasificaciones.cadena)
                {
                    l.setValor(nombre, contenidos);
                    match(clasificaciones.cadena);
                }
                else
                {
                    Expresion();
                    l.setValor(nombre, s.pop(bitacora, linea, caracter).ToString());
                }
            }

            if (getContenido() == ",")
            {
                match(",");
                Lista_IDs(Tipos);
            }
        }

        // Variables -> tipoDato Lista_IDs; 
        private void Variables()
        {
            string contenidos = getContenido();
            match(clasificaciones.tipoDato);
            Variable.tipo Tipos;
            switch (contenidos)
            {
                case "const":
                    Tipos = Variable.tipo.CONST;
                    break;
                case "int":
                    Tipos = Variable.tipo.INT;
                    break;
                case "float":
                    Tipos = Variable.tipo.FLOAT;
                    break;
                case "char":
                    Tipos = Variable.tipo.CHAR;
                    break;
                case "string":
                    Tipos = Variable.tipo.STRING;
                    break;
                default:
                    Tipos = Variable.tipo.CHAR;
                    break;
            }
            Lista_IDs(Tipos);

            match(clasificaciones.finSentencia);
        }
        // Instruccion -> (If | cin | cout | const | Variables | asignacion) ;
        private void Instruccion()
        {
            if (getContenido() == "do")
            {
                DOWHILE ();
            }
            else if (getContenido() == "while")
            {
                WHILE();
            }
            else if (getContenido() == "for")
            {
                FOR();
            }
            else if (getContenido() == "if")
            {
                IF();
            }
            else if (getContenido() == "cin")
            {
                match("cin"); //Requerimiento 5
                match(clasificaciones.flujoEntrada);

                string contenido = getContenido();
                if (l.Existe(contenido))
                {
                    string lector = Console.ReadLine();
                    match(clasificaciones.identificador);
                    l.setValor(contenido, lector);
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + contenido + ") " + "(" + linea + ", " + caracter + ")");
                }
                match(clasificaciones.finSentencia);
            }
            else if (getContenido() == "cout")
            {
                match("cout");
                ListaFlujoSalida();
                match(clasificaciones.finSentencia);
            }
            else if (getContenido() == "const")
            {
                Constante();
            }
            else if (getClasificacion() == clasificaciones.tipoDato)
            {
                Variables();
            }
            else
            {
                string nombre = getContenido();
                if (l.Existe(nombre))
                {
                    match(clasificaciones.identificador);
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + nombre + ") " + "(" + linea + ", " + caracter + ")");
                }
                match(clasificaciones.asignacion);

                string valor;
                if (getClasificacion() == clasificaciones.cadena)
                {
                    valor = getContenido();
                    match(clasificaciones.cadena);
                }
                else
                {
                    Expresion();
                    valor = s.pop(bitacora, linea, caracter).ToString();
                }
                l.setValor(nombre, valor);
                match(clasificaciones.finSentencia);
            }
        }
        // Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones()
        {
            Instruccion();
            if (getClasificacion() != clasificaciones.finBloque)
            {
                Instrucciones();
            }
        }

        // Constante -> const tipoDato identificador = numero | cadena;
        private void Constante()
        {
            match("const"); 
            string contenidos = getContenido();
            match(clasificaciones.tipoDato);
            Variable.tipo Tipos;
            switch (contenidos)
            {
                case "const":
                    Tipos = Variable.tipo.CONST;
                    break;
                case "int":
                    Tipos = Variable.tipo.INT;
                    break;
                case "float":
                    Tipos = Variable.tipo.FLOAT;
                    break;
                case "char":
                    Tipos = Variable.tipo.CHAR;
                    break;
                case "string":
                    Tipos = Variable.tipo.STRING;
                    break;
                default:
                    Tipos = Variable.tipo.CHAR;
                    break;
            }

            string content = getContenido();
            if (!l.Existe(content))
            {
                match(clasificaciones.identificador);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + content + ") " + "(" + linea + ", " + caracter + ")");
            }
            match(clasificaciones.asignacion);

            string normal;
            if (getClasificacion() == clasificaciones.numero)
            {
                normal = getContenido();
                match(clasificaciones.numero);
                l.setValor(content, normal);
            }
            else
            {
                normal = getContenido();
                match(clasificaciones.cadena);
                l.setValor(content, normal);
            }
            match(clasificaciones.finSentencia);
        }
        // ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida()
        {
            match(clasificaciones.flujoSalida);

            if (getClasificacion() == clasificaciones.numero)
            {
                Console.Write(getContenido());
                match(clasificaciones.numero);
            }
            else if (getClasificacion() == clasificaciones.cadena)
            {
                string saltos = getContenido(); //Secuencias de escape
                if (saltos.Contains("\\n"))
                {
                    saltos = saltos.Replace("\\n","\n");
                }

                if (saltos.Contains("\\t"))
                {
                    saltos = saltos.Replace("\\t", "\t");
                }

                if (saltos.Contains("\""))
                {
                    saltos = saltos.Replace("\"", "");
                }
                Console.Write(saltos);
                match(clasificaciones.cadena);
            }
            else
            {
                string nombre = getContenido();
                if (l.Existe(nombre))
                {
                    Console.Write(l.getValor(nombre));
                    match(clasificaciones.identificador);
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + nombre + ") " + "(" + linea + ", " + caracter + ")");
                }
            }

            if (getClasificacion() == clasificaciones.flujoSalida)
            {
                ListaFlujoSalida();
            }
        }
        // IF -> if (Condicion) { BloqueInstrucciones } (else BloqueInstrucciones)?
        private void IF()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            BloqueInstrucciones();

            if (getContenido() == "else")
            {
                match("else");
                BloqueInstrucciones();
            }
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(clasificaciones.operadorRelacional);
            Expresion();
        }
        // x26 = (3+5)*8-(10-4)/2;
        // Expresion -> Termino MasTermino 
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (operadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == clasificaciones.operadorTermino)
            {
                string operador = getContenido();
                match(clasificaciones.operadorTermino);
                Termino();
                float e1 = s.pop(bitacora, linea, caracter), e2 = s.pop(bitacora, linea, caracter);
                switch (operador)
                {
                    case "+":
                        s.push(e2 + e1, bitacora, linea, caracter);
                        break;
                    case "-":
                        s.push(e2 - e1, bitacora, linea, caracter);
                        break;
                }
                s.display(bitacora);
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (operadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == clasificaciones.operadorFactor)
            {
                string operador = getContenido();
                match(clasificaciones.operadorFactor);
                Factor();
                float e1 = s.pop(bitacora, linea, caracter), e2 = s.pop(bitacora, linea, caracter);
                switch (operador)
                {
                    case "*":
                        s.push(e2 * e1, bitacora, linea, caracter);
                        break;
                    case "/":
                        s.push(e2 / e1, bitacora, linea, caracter);
                        break;
                }
                s.display(bitacora);
            }
        }
        // Factor -> identificador | numero | ( Expresion )
        private void Factor()
        {
            if (getClasificacion() == clasificaciones.identificador)
            {
                string nombre = getContenido();
                if (l.Existe(nombre))
                {
                    s.push(float.Parse(l.getValor(getContenido())), bitacora, linea, caracter);
                    s.display(bitacora);
                    match(clasificaciones.identificador);
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + nombre + ") " + "(" + linea + ", " + caracter + ")");
                }
            }
            else if (getClasificacion() == clasificaciones.numero)
            {
                // Console.Write(getContenido() + " ");
                s.push(float.Parse(getContenido()), bitacora, linea, caracter);
                s.display(bitacora);
                match(clasificaciones.numero);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
        // FOR -> for (identificador = Expresion; Condicion; identificador incrementoTermino) BloqueInstrucciones
        private void FOR()
        {
            match("for");
            match("(");
            string nombre = getContenido();
            if (l.Existe(nombre))
            {
                match(clasificaciones.identificador);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + nombre + ") " + "(" + linea + ", " + caracter + ")");
            }
            match(clasificaciones.asignacion);
            Expresion();
            match(clasificaciones.finSentencia);
            Condicion();
            match(clasificaciones.finSentencia);
            string contenido = getContenido(); //Variables diferentes para evitar confusion
            if (l.Existe(contenido))
            {
                match(clasificaciones.identificador);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Variable no existe (" + contenido + ") " + "(" + linea + ", " + caracter + ")");
            }
            match(clasificaciones.incrementoTermino);
            match(")");
            BloqueInstrucciones();
        }
        // WHILE -> while (Condicion) BloqueInstrucciones
        private void WHILE()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            BloqueInstrucciones();
        }
        // DOWHILE -> do BloqueInstrucciones while (Condicion);
        private void DOWHILE()
        {
            match("do");
            BloqueInstrucciones();
            match("while");
            match("(");
            Condicion();
            match(")");
            match(clasificaciones.finSentencia);
        }
        // x26 = (3 + 5) * 8 - (10 - 4) / 2
        // x26 = 3 + 5 * 8 - 10 - 4 / 2
        // x26 = 3 5 + 8 * 10 4 - 2 / -
    }
}