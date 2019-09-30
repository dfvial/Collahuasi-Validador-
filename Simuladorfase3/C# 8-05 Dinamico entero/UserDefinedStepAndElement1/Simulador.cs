using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Simulador
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "Simulador"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "Sesacaunatesis"; }
        }

        /// <summary>
        /// Property returning an icon to display for the step in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Property returning a unique static GUID for the step.
        /// </summary>
        public Guid UniqueID
        {
            get { return MY_ID; }
        }
        static readonly Guid MY_ID = new Guid("{480d249b-a809-4b8b-8668-1767f0797de9}");

        /// <summary>
        /// Property returning the number of exits out of the step. Can return either 1 or 2.
        /// </summary>
        public int NumberOfExits
        {
            get { return 1; }
        }

        /// <summary>
        /// Method called that defines the property schema for the step.
        /// </summary>
        public void DefineSchema(IPropertyDefinitions schema)
        {
            // Example of how to add a property definition to the step.
            IPropertyDefinition pd;
            pd = schema.AddStateProperty("Timenow");
            pd.Description = "Recibe el tiempo en horas";

            pd = schema.AddStateProperty("Salida");
            pd.Description = "Siguiente pala";

            pd = schema.AddStateProperty("Aux");
            pd.Description = "Camino i para despues";


        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process.
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new UserStep1(properties);
        }

        #endregion
    }

    class UserStep1 : IStep
    {
        IPropertyReaders _properties;
        IStateProperty _propTimenow;
        IStateProperty _propSalida;
        IStateProperty _propAux;

        Vect vectores;

        public UserStep1(IPropertyReaders properties)
        {
            _properties = properties;
            _propTimenow = (IStateProperty)_properties.GetProperty("Timenow");
            _propSalida = (IStateProperty)_properties.GetProperty("Salida");
            _propAux = (IStateProperty)_properties.GetProperty("Aux");

            vectores = new Vectores.Vect();
        }

        // OTRAS FUNCIONES
        public double Min1D(double[] arreglo) // encuentra el minimo de un vector
        {
            double minimo = arreglo[0];
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                if (arreglo[i] < minimo)
                {
                    minimo = arreglo[i];
                }
            }
            return minimo;
        }
        public double Max1D(double[] arreglo) // encuentra el maximo de un vector
        {
            double maximo = arreglo[0];
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                if (arreglo[i] > maximo)
                {
                    maximo = arreglo[i];
                }
            }
            return maximo;
        }
        public double Min2D(double[,] arreglo) // encuentra el minimo en una matriz de 2 dimensiones
        {
            double minimo = arreglo[0, 0];
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                for (int j = 0; j < arreglo.GetLength(1); j++)
                {
                    if (arreglo[i, j] < minimo)
                    {
                        minimo = arreglo[i, j];
                    }
                }
            }
            return minimo;
        }

        public double Max2D(double[,] arreglo) // encuentra el maximo en una matriz de 2 dimensiones
        {
            double maximo = arreglo[0, 0];
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                for (int j = 0; j < arreglo.GetLength(1); j++)
                {
                    if (arreglo[i, j] > maximo)
                    {
                        maximo = arreglo[i, j];
                    }
                }
            }
            return maximo;
        }
        public double Min2D1C(double[,] arreglo, int col) // encuentra el minimo de una columna de una matriz de 2 dimensiones
        {
            double minimo = arreglo[0, col];
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                if (arreglo[i, col] < minimo)
                {
                    minimo = arreglo[i, col];
                }
            }
            return minimo;
        }

        public double Max2D1C(double[,] arreglo, int col) // encuentra el maximo de una columna de una matriz de 2 dimensiones
        {
            double maximo = arreglo[0, col];
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                if (arreglo[i, col] > maximo)
                {
                    maximo = arreglo[i, col];
                }
            }
            return maximo;
        }
        public double SumaColumna2Dint(double[,] arreglo, int col) // cuanto suma una columna en 2 dimensiones
        {
            double suma = 0;
            for (int i = 0; i < arreglo.GetLength(0); i++)
            {
                suma = suma + arreglo[i, col];
            }
            return suma;
        }
        public double RandomTriangular(double TMin, double TModa, double TMax) //Saca tiempo aleatorio triangular
        {
            double ValorRandom = 0;
            Random rnd = new Random();
            double U = rnd.NextDouble();
            //Genera tiempo de carga aleatorio
            if (U <= (TModa - TMin) / (TMax - TMin))
            {
                ValorRandom = TMin + Math.Sqrt((TMax - TMin) * (TModa - TMin) * U);
            }
            else
            {
                ValorRandom = TMax - Math.Sqrt((1 - U) * (TMax - TMin) * (TMax - TModa));
            }
            return ValorRandom;
        }

        // METODOS DE DESPACHO DINAMICO
        // 1
        public int Li(int NPalas, int NSitios, double TNOW, double[,] FlujoCamionesCargado, double[] TUltimaAsignacion, int[] Disponible)
        {
            double[] tij_op = new double[NPalas];
            double[] desv_tij = new double[NPalas];
            double[] Puntaje = new double[NPalas];
            double Pmax = -10000000000;
            int Destino = 0;
            for (int j = 0; j < NPalas; j++)
            {
                if (Disponible[j] > 0)
                {
                    double FlujoCamionesCargadoTotal = 0;
                    for (int i = 0; i < NSitios; i++)
                    {
                        FlujoCamionesCargadoTotal = FlujoCamionesCargadoTotal + FlujoCamionesCargado[i, j];
                    }
                    tij_op[j] = 1 / FlujoCamionesCargadoTotal;
                    desv_tij[j] = (TNOW - TUltimaAsignacion[j]) - tij_op[j];
                    Puntaje[j] = desv_tij[j] / tij_op[j];
                    if (Puntaje[j] > Pmax)
                    {
                        Pmax = Puntaje[j];
                        Destino = j;
                    }
                }
            }
            return Destino;
        }
        // 2
        public int LiCasiOriginal(int NPalas, int NSitios, double TNOW, double[,] FlujoCamionesVacio, double[,] FlujoCamionesCargado, double[] TUltimaAsignacion, int[] Disponible, int[] Posicion, int Camion)
        {
            double[] tij_op = new double[NPalas];
            double[] desv_tij = new double[NPalas];
            double[] Puntaje = new double[NPalas];
            double Pmax = -10000000000;
            int Destino = 0;
            int[] Candidata = new int[NPalas];
            for (int j = 0; j < NPalas; j++)
            {
                Candidata[j] = 0;
                if (FlujoCamionesVacio[Posicion[Camion] - NPalas, j] > 0 && Disponible[j] > 0)
                {
                    Candidata[j] = 1;
                }
            }
            for (int j = 0; j < NPalas; j++)
            {
                if (Candidata[j] > 0)
                {
                    double FlujoCamionesCargadoTotal = 0;
                    for (int i = 0; i < NSitios; i++)
                    {
                        FlujoCamionesCargadoTotal = FlujoCamionesCargadoTotal + FlujoCamionesCargado[i, j];
                    }
                    tij_op[j] = 1 / FlujoCamionesCargadoTotal;
                    desv_tij[j] = (TNOW - TUltimaAsignacion[j]) - tij_op[j];
                    Puntaje[j] = desv_tij[j] / tij_op[j];
                    if (Puntaje[j] > Pmax)
                    {
                        Pmax = Puntaje[j];
                        Destino = j;
                    }
                }
            }
            return Destino;
        }
        // 3
        public int LiOriginal(int NPalas, int NSitios, double TNOW, double[,] FlujoCamionesVacio, double[,] FlujoCamionesCargado, double[] TUltimaAsignacion, int[] Disponible, int[] Posicion, int Camion)
        {
            double[] tij_op = new double[NPalas];
            double[] desv_tij = new double[NPalas];
            double[] Puntaje = new double[NPalas];
            double Pmax = -10000000000;
            int Destino = 0;
            int[] Candidata = new int[NPalas];
            for (int j = 0; j < NPalas; j++)
            {
                Candidata[j] = 0;
                if (FlujoCamionesVacio[Posicion[Camion] - NPalas, j] > 0 && Disponible[j] > 0)
                {
                    Candidata[j] = 1;
                }
            }
            for (int j = 0; j < NPalas; j++)
            {
                if (Candidata[j] > 0)
                {
                    tij_op[j] = 1 / FlujoCamionesVacio[Posicion[Camion] - NPalas, j];
                    desv_tij[j] = (TNOW - TUltimaAsignacion[j]) - tij_op[j];
                    Puntaje[j] = desv_tij[j] / tij_op[j];
                    if (Puntaje[j] > Pmax)
                    {
                        Pmax = Puntaje[j];
                        Destino = j;
                    }
                }
            }
            return Destino;
        }

        // METODO QUE LLAMA A LOS DESPACHOS DINAMICOS
        public int FMS(int Metodo, int Camion, int[] Posicion, int NPalas, int NSitios, double TNOW, double[] TUltimaAsignacion, int[] Disponible, double[,] FlujoCamionesCargado, double[,] FlujoCamionesVacio)
        {
            int destino = -1;
            if (Metodo == 1)
            {
                destino = Li(NPalas, NSitios, TNOW, FlujoCamionesCargado, TUltimaAsignacion, Disponible);
            }
            else if (Metodo == 2)
            {
                destino = LiCasiOriginal(NPalas, NSitios, TNOW, FlujoCamionesVacio, FlujoCamionesCargado, TUltimaAsignacion, Disponible, Posicion, Camion);
            }
            else if (Metodo == 3)
            {
                destino = LiOriginal(NPalas, NSitios, TNOW, FlujoCamionesVacio, FlujoCamionesCargado, TUltimaAsignacion, Disponible, Posicion, Camion);
            }
            return destino;
        }

        // SIMULACION
        public int[] Simulacion(int Metodo, double timenow)
        {
            //VARIABLES
            int NumPalas = 20;
            int NumSitios = 30;
            int NumCamiones = 81;
            int CapCamion = 300;
            double[,] TiempoVacio = (double[,])vectores.TCV.Clone();
            double[,] TiempoCargado = (double[,])vectores.TCC.Clone();
            int CuentaDespachos = 0;

            //RESULTADO (Almacena la cantidad de cargas realizadas en cada pala y en la última fila (+1) el destino del camión que solicita despacho)
            int[] Resultado = new int[NumPalas + 1];
            for (int j = 0; j < NumPalas + 1; j++)
            {
                Resultado[j] = 0;
            }

            // ATRIBUTOS PALA
            double[] Flujo = new double[NumPalas];
            for (int j = 0; j < NumPalas; j++)
            {
                Flujo[j] = vectores.PlTc[30, j];
            }
            double[] TCargaModa = new double[NumPalas]; //vectores.TCargaModa
            double[] TCargaMin = new double[NumPalas]; //vectores.TCargaMin
            double[] TCargaMax = new double[NumPalas]; //vectores.TCargaMax
            double TAculatado = 70;
            double[] TUltimaAsignacion = new double[NumPalas];
            for (int j = 0; j < NumPalas; j++)
            {
                TUltimaAsignacion[j] = vectores.Din1[30 * j, 4];
            }
            int[] BotaderoPala = new int[] { 8, 8, 8, 1, 1, 1, 4, 4, 3, 1, 4, 5, 3, 3, 3, 3, 1, 1, 3, 3 };

            // ATRIBUTOS PALAS Y SITIOS
            double[,] FlujoCamionesVacio = (double[,])vectores.PlYv.Clone();
            double[,] FlujoCamionesCargado = (double[,])vectores.PlYc.Clone();
            int[] Disponible = new int[NumPalas + NumSitios];
            for (int j = 0; j < NumPalas; j++)
            {
                Disponible[j] = Convert.ToInt32(vectores.Uj[j]);
            }
            for (int j = NumPalas; j < NumPalas + NumSitios; j++)
            {
                Disponible[j] = Convert.ToInt32(vectores.Ui[j]);
            }
            double[,] Colas = new double[NumCamiones, NumPalas + NumSitios]; //Posicion en la cola de pala o sitio j de camion i
            double[] Cola = new double[NumPalas + NumSitios]; //Se actualiza al inicio de la simulación
            double[] ColaPromedio = new double[NumPalas + NumSitios]; //Vector con cola promedio de cada pala y sitio
            int[] CamionesViaje = new int[NumPalas + NumSitios]; //DesCam columna 9 si 1 C2 Pala y C3 Botadero viajando, 2 si C2 Botadero C3 Pala viajando, 3 en pala, 4 en botadero
            // Se construye de DesCam columna 9


            //double[] TAcum = new double[NumPalas + NumSitios];
            double[] Atendidos = new double[NumPalas + NumSitios];
            //double[] TMedio = new double[NumPalas + NumSitios];
            double[] TAcumCola = new double[NumPalas + NumSitios];
            int[] SalenCola = new int[NumPalas + NumSitios];
            double[] TMedioCola = new double[NumPalas + NumSitios];

            // ATRIBUTOS SITIOS
            int[] DescargasSimultaneas = new int[NumSitios]; //vectores.DescargasSimultaneas
            double[] TDescargaModa = new double[NumSitios]; //vectores.TCargaModa
            double[] TDescargaMin = new double[NumSitios]; //vectores.TCargaMin
            double[] TDescargaMax = new double[NumSitios]; //vectores.TCargaMax

            // ATRIBUTOS CAMION
            int[] Posicion = new int[NumCamiones];
            int[] Destino = new int[NumCamiones];
            for (int i = 0; i < NumCamiones; i++)
            {
                if (vectores.DesCam[i, 8] == 1) // DesCam columa 8 = 1 si C2 Pala y C3 Botadero, 2 si C2 Botadero C3 Pala, 3 si C2 y C3 Pala
                {
                    Posicion[i] = Convert.ToInt32(vectores.DesCam[i, 2]);
                    Destino[i] = NumPalas + Convert.ToInt32(vectores.DesCam[i, 3]);
                }
                else if (vectores.DesCam[i, 8] == 2)
                {
                    Posicion[i] = NumPalas + Convert.ToInt32(vectores.DesCam[i, 2]);
                    Destino[i] = Convert.ToInt32(vectores.DesCam[i, 3]);
                }
                else
                {
                    Posicion[i] = Convert.ToInt32(vectores.DesCam[i, 2]);
                    Destino[i] = Convert.ToInt32(vectores.DesCam[i, 3]);
                }
            }
            double[] TInicioCarga = new double[NumCamiones]; //Vector de camiones con tiempo inicio carga
            double[] TLlegadaCola = new double[NumCamiones]; //Vector de camiones con tiempo llegada cola
            double[] TFinCarga = new double[NumCamiones]; //Vector de camiones con tiempo fin última carga
            double[] TSalidaCola = new double[NumCamiones]; //Vector de camiones con tiempo salida cola
            double[] TUltimoDespacho = new double[NumCamiones]; //Vector de camiones con tiempo de última vez que fue despachado
            int[] Botadero = new int[NumCamiones];

            // RELOJ DE SIMULACION
            double TINICIO = timenow;
            double TSIM = timenow;
            double HORIZONTE = 1;
            double TANT = timenow;

            // MINIMOS PARA PROXIMO EVENTO
            double MinFinViaje = 9999;
            double MinFinCarga = 9999;
            double MinFinDescarga = 9999;
            double MinGlobal = 9999;
            double[,] FinViaje = new double[NumCamiones, NumPalas + NumSitios];
            double[,] FinCarga = new double[NumCamiones, NumPalas];
            double[,] FinDescarga = new double[NumCamiones, NumSitios];
            for (int i = 0; i < NumCamiones; i++)
            {
                for (int j = 0; j < NumPalas + NumSitios; j++)
                {
                    FinViaje[i, j] = 9999;
                }
                for (int j = 0; j < NumPalas; j++)
                {
                    FinCarga[i, j] = 9999;
                }
                for (int j = 0; j < NumSitios; j++)
                {
                    FinDescarga[i, j] = 9999;
                }
            }


            // TONELAJE CARGADO HASTA EL INICIO DE LA SIMULACION
            //double[] TON_PALA = new double[] { };

            // INICIA SIMULACION

            while (TSIM < TINICIO + HORIZONTE)
            {
                //ACTUALIZA COLAS
                for (int i = 0; i < NumPalas + NumSitios; i++)
                {
                    Cola[i] = Max2D1C(Colas, i);
                    ColaPromedio[i] = (TANT * ColaPromedio[i] + Cola[i] * (TSIM - TANT)) / TSIM;
                }

                TANT = TSIM;
                //BUSCA PROXIMO EVENTO
                MinFinViaje = Min2D(FinViaje);
                MinFinCarga = Min2D(FinCarga);
                MinFinDescarga = Min2D(FinDescarga);
                var Mins = new double[] { MinFinViaje, MinFinCarga, MinFinDescarga };
                MinGlobal = Min1D(Mins);

                if (MinGlobal == MinFinViaje)
                {
                    for (int i = 0; i < NumCamiones; i++)
                    {
                        for (int j = 0; j < NumPalas + NumSitios; j++)
                        {
                            if (MinFinViaje == FinViaje[i, j])
                            {
                                TSIM = FinViaje[i, j];
                                Posicion[i] = j;
                                CamionesViaje[Posicion[i]] = CamionesViaje[Posicion[i]] - 1;
                                if (Posicion[i] <= NumPalas)
                                {
                                    if (SumaColumna2Dint(Colas, Posicion[i]) == 0 && Disponible[Posicion[i]] == 0)
                                    {
                                        TInicioCarga[i] = TSIM;
                                        //Genera tiempo de carga aleatorio
                                        FinCarga[i, Posicion[i]] = TSIM + TAculatado / 60 / 60 + RandomTriangular(TCargaMin[j], TCargaModa[j], TCargaMax[j]);
                                        Disponible[Posicion[i]] = 1;
                                    }
                                    else
                                    {
                                        Colas[i, Posicion[i]] = Max2D1C(Colas, Posicion[i]) + 1;
                                        TLlegadaCola[i] = TSIM;
                                    }
                                }
                                else
                                {
                                    if (SumaColumna2Dint(Colas, Posicion[i]) == 0 && Disponible[Posicion[i]] <= DescargasSimultaneas[j - NumPalas])
                                    {
                                        TInicioCarga[i] = TSIM;
                                        //Genera tiempo de carga aleatorio
                                        FinDescarga[i, Posicion[i]] = TSIM + TAculatado / 60 / 60 + RandomTriangular(TDescargaMin[j - NumPalas], TDescargaModa[j - NumPalas], TDescargaMax[j - NumPalas]);
                                        Disponible[Posicion[i]] = Disponible[Posicion[i]] + 1;
                                    }
                                    else
                                    {
                                        Colas[i, Posicion[i]] = Max2D1C(Colas, Posicion[i]) + 1;
                                        TLlegadaCola[i] = TSIM;
                                    }
                                }
                                FinViaje[i, Posicion[i]] = 9999;
                            }
                        }
                    }
                }
                else if (MinGlobal == MinFinCarga)
                {
                    for (int i = 0; i < NumCamiones; i++)
                    {
                        for (int j = 0; j < NumPalas; j++)
                        {
                            if (MinFinCarga == FinCarga[i, j])
                            {
                                TSIM = FinCarga[i, j];
                                TFinCarga[i] = TSIM;
                                double TCarga = TFinCarga[i] - TInicioCarga[i];
                                //TAcum[Posicion[i]] = TAcum[Posicion[i]] + TCarga;
                                Atendidos[Posicion[i]] = Atendidos[Posicion[i]] + 1;
                                //TMedio[Posicion[i]] = TAcum[Posicion[i]] / Atendidos[Posicion[i]];
                                int Aux = -1;
                                //Actualiza Cola
                                if (SumaColumna2Dint(Colas, j) > 0)
                                {
                                    for (int l = 0; l < NumCamiones; l++)
                                    {
                                        if (Colas[l, j] == 1)
                                        {
                                            Aux = l;
                                            Colas[l, j] = 0;
                                        }
                                        else if (Colas[l, j] != 0)
                                        {
                                            Colas[l, j] = Colas[l, j] - 1;
                                        }
                                    }
                                    TSalidaCola[Aux] = TSIM;
                                }
                                else
                                {
                                    Disponible[j] = 0;
                                }
                                if (Aux != -1)
                                {
                                    TInicioCarga[Aux] = TSIM;
                                    if (Max2D1C(Colas, j) == 0)
                                    {
                                        FinCarga[Aux, j] = TSIM + TAculatado / 60 / 60 + RandomTriangular(TCargaMin[j], TCargaModa[j], TCargaMax[j]);
                                    }
                                    else
                                    {
                                        FinCarga[Aux, j] = TSIM + RandomTriangular(TCargaMin[j], TCargaModa[j], TCargaMax[j]);
                                    }
                                }
                                double TCola = TSalidaCola[i] - TLlegadaCola[i];
                                TAcumCola[Posicion[i]] = TAcumCola[Posicion[i]] + TCola;
                                SalenCola[Posicion[i]] = SalenCola[Posicion[i]] + 1;
                                TMedioCola[Posicion[i]] = TAcumCola[Posicion[i]] / SalenCola[Posicion[i]];
                                Destino[i] = Botadero[i];
                                TUltimoDespacho[i] = TSIM;
                                //TUltimaAsignacion[Destino[i]] = TSIM;
                                FinViaje[i, Destino[i]] = TSIM + TiempoCargado[Destino[i] - NumPalas, Posicion[i]];
                                CamionesViaje[Destino[i]] = CamionesViaje[Destino[i]] + 1;
                                FinCarga[i, j] = 9999;

                                //ACTUALIZA RESULTADO
                                Resultado[Posicion[i]] = Resultado[Posicion[i]] + 1;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < NumCamiones; i++)
                    {
                        for (int j = 0; j < NumSitios; j++)
                        {
                            if (MinFinDescarga == FinDescarga[i, j])
                            {
                                TSIM = FinDescarga[i, j];
                                TFinCarga[i] = TSIM;
                                double TCarga = TFinCarga[i] - TInicioCarga[i];
                                //TAcum[Posicion[i]] = TAcum[Posicion[i]] + TCarga;
                                Atendidos[Posicion[i]] = Atendidos[Posicion[i]] + 1;
                                //TMedio[Posicion[i]] = TAcum[Posicion[i]] / Atendidos[Posicion[i]];
                                int Aux = -1;
                                if (SumaColumna2Dint(Colas, NumPalas + j) > 0)
                                {
                                    for (int l = 0; l < NumCamiones; l++)
                                    {
                                        if (Colas[l, NumPalas + j] == 1)
                                        {
                                            Aux = l;
                                            Colas[l, NumPalas + j] = 0;
                                        }
                                        else if (Colas[l, NumPalas + j] != 0)
                                        {
                                            Colas[l, NumPalas + j] = Colas[l, NumPalas + j] + 1;
                                        }
                                    }
                                    TSalidaCola[Aux] = TSIM;
                                }
                                else
                                {
                                    Disponible[NumPalas + j] = Disponible[NumPalas + j] - 1;
                                }
                                if (Aux != -1)
                                {
                                    TInicioCarga[Aux] = TSIM;
                                    FinDescarga[Aux, j] = TSIM + RandomTriangular(TDescargaMin[j], TDescargaModa[j], TDescargaMax[j]);
                                }
                                double TCola = TSalidaCola[i] - TLlegadaCola[i];
                                TAcumCola[Posicion[i]] = TAcumCola[Posicion[i]] + TCola;
                                SalenCola[Posicion[i]] = SalenCola[Posicion[i]] + 1;
                                TMedioCola[Posicion[i]] = TAcumCola[Posicion[i]] / SalenCola[Posicion[i]];

                                //DESPACHA
                                Destino[i] = FMS(Metodo, i, Posicion, NumPalas, NumSitios, TSIM, TUltimaAsignacion, Disponible, FlujoCamionesCargado, FlujoCamionesVacio);
                                Botadero[i] = BotaderoPala[Destino[i]];
                                if (CuentaDespachos == 0)
                                {
                                    Resultado[NumPalas] = Destino[i];
                                }
                                CuentaDespachos = CuentaDespachos + 1;
                                TUltimaAsignacion[Destino[i]] = TSIM;
                                TUltimoDespacho[Destino[i]] = TSIM;
                                FinViaje[i, Destino[i]] = TSIM + TiempoVacio[Posicion[i] - NumPalas, Destino[i]];
                                CamionesViaje[Destino[i]] = CamionesViaje[Destino[i]] + 1;
                                FinDescarga[i, j] = 9999;
                            }
                        }
                    }
                }



            }
            double TTOTAL = TSIM - TINICIO;

            return Resultado;
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {

            IState _timenow = _propTimenow.GetState(context);
            double timenow = Convert.ToDouble(_timenow.StateValue);
            IState _salida = _propSalida.GetState(context);
            double salida = Convert.ToDouble(_salida.StateValue);
            IState _aux = _propAux.GetState(context);
            double aux = Convert.ToDouble(_aux.StateValue);

            SerializarElement sr = new SerializarElement();

            if (File.Exists(sr.SerializationFile))
            {

                Vect v = sr.deserializa();
                vectores = sr.deserializa();

                vectores.MipYc = v.MipYc;
                vectores.MipYv = v.MipYv;
                vectores.MipTc = v.MipTc;
                vectores.MipTv = v.MipTv;
                vectores.PlYc = v.PlYc;
                vectores.PlYv = v.PlYv;
                vectores.PlTc = v.PlTc;
                vectores.PlTv = v.PlTv;
                vectores.PlTcmalo = v.PlTcmalo;

                //Dinamico
                vectores.Din1 = v.Din1;
                vectores.DesCam = v.DesCam;
                vectores.Fila = v.Fila;

                //LP
                vectores.TCV = v.TCV;
                vectores.TCC = v.TCC;
                vectores.TCj = v.TCj;
                vectores.TDi = v.TDi;
                vectores.Destij = v.Destij;
                vectores.Dj = v.Dj;
                vectores.Uj = v.Uj;
                vectores.Ui = v.Ui;
                vectores.RLi = v.RLi;
                vectores.RUi = v.RUi;
                vectores.Mine = v.Mine;
                vectores.Mineralocado = v.Mineralocado;
                vectores.Lastrealocado = v.Lastrealocado;
            }

            /* programa aca*/



            int[,] Resultado = new int[20 + 1, 3];
            int[] VectorResultado = new int[20 + 1];
            Parallel.For(1, 3, m =>
            {
                VectorResultado = Simulacion(m, timenow);
                for (int i = 0; i < 20 + 1; i++)
                {
                    Resultado[i, m - 1] = VectorResultado[i];
                }
            }
            )
            ;



            sr.serializa(vectores);
            sr.closeStream();
            return ExitType.FirstExit;

        }

    }

}
#endregion
