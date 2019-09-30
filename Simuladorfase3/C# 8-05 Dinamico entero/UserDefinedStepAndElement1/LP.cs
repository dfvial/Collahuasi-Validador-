using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurobi;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;

namespace Linearprograming
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "LP"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "Recalcula los flujos"; }
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
        static readonly Guid MY_ID = new Guid("{5ceba900-abfa-49a0-bf6b-851db9bad4ee}");

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
            IPropertyDefinition pd;
            pd = schema.AddStateProperty("Timenow");
            pd.Description = "Recibe el tiempo en horas";
        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process.
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new LP(properties);
        }

        #endregion
    }

    class LP : IStep
    {
        IPropertyReaders _properties;
        IStateProperty _propTimenow;
        Vect vectores;

        public LP(IPropertyReaders properties)
        {
            _properties = properties;
            _propTimenow = (IStateProperty)_properties.GetProperty("Timenow");
            vectores = new Vectores.Vect();
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {

            IState _timenow = _propTimenow.GetState(context);
            double timenow = Convert.ToDouble(_timenow.StateValue);

            SerializarElement sr = new SerializarElement();

            if (File.Exists(sr.SerializationFile))
            {

                Vect v = sr.deserializa();
                vectores = sr.deserializa();

                vectores.MipYc = v.MipYc;
                vectores.MipYv = v.MipYv;
                vectores.MipTc = v.MipTc;
                vectores.MipTv = v.MipTv;

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
                vectores.PlYc = v.PlYc;
                vectores.PlYv = v.PlYv;
                vectores.PlTc = v.PlTc;
                vectores.PlTv = v.PlTv;
                vectores.PlTcmalo = v.PlTcmalo;
                vectores.Mine = v.Mine;
                vectores.Mineralocado = v.Mineralocado;
                vectores.Lastrealocado = v.Lastrealocado;
            }

            int i = 30;
            int j = 20;
            double TA;
            TA = 0.0158333333; //tiempo de aculatado
            double expr;
            expr = 0; //variable auxiliar

            GRBEnv env = new GRBEnv();

            GRBModel disp = new GRBModel(env);

            GRBLinExpr L = 320; //Numero auxiliar de velocidad de descarga
            GRBLinExpr M = 110; //Numero max camiones  
            double[,] Tcvij = new double[30, 20];
            double[,] Tccij = new double[30, 20];

            for (int a = 0; a < i; a = a + 1) // saca tiempo de viaje total
            {
                for (int b = 0; b < j; b = b + 1)
                {
                    Tcvij[a, b] = vectores.TCV[a, b] + vectores.TDi[a];

                    if (vectores.Destij[a, b] != 0)
                    {
                        Tccij[a, b] = vectores.TCC[a, b] + vectores.TCj[b] + TA;
                    }
                    else
                    {
                        Tccij[a, b] = 9999;
                    }
                }
            }



            for (int b = 0; b < j; b = b + 1)
            {
                expr = 0;

                for (int a = 0; a < i; a = a + 1)
                {
                    expr = expr + vectores.Destij[a, b];
                }
                if (expr == 0)
                {
                    vectores.Uj[b] = 0;
                }
            }


            GRBVar[,] Ycij = new GRBVar[30, 20]; //Camiones cargados de i a j
            GRBVar[,] Yvij = new GRBVar[30, 20]; //Camiones vacios de i a j
            GRBVar[,] Tcij = new GRBVar[30, 20]; //Throughtput cargado de i a j
            GRBVar[,] Tvij = new GRBVar[30, 20]; //Throughtput vacio de i a j

            for (int a = 0; a < i; a = a + 1)
            {
                for (int b = 0; b < j; b = b + 1)
                {
                    Ycij[a, b] = disp.AddVar(0.0, 1000, 0.0, GRB.CONTINUOUS, "Yc" + a + "_" + b);
                    Yvij[a, b] = disp.AddVar(0.0, 1000, 0.0, GRB.CONTINUOUS, "Yv" + a + "_" + b);
                    Tcij[a, b] = disp.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "Tc" + a + "_" + b); //tonelaje cargado pasando hacia j en toneladas
                    Tvij[a, b] = disp.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "Tv" + a + "_" + b); //tonelaje vacio pasando hacia j en toneladas

                }

            }

            for (int a = 0; a < i; a = a + 1)
            {
                for (int b = 0; b < j; b = b + 1)
                {
                    disp.AddConstr(Tcij[a, b] == ((Ycij[a, b] / Tccij[a, b]) * L), "tonelaje_minimo" + a);
                    disp.AddConstr(Tvij[a, b] == ((Yvij[a, b] / Tcvij[a, b]) * L), "tonelaje_maximo" + b);
                }
            }

            for (int b = 0; b < j; b = b + 1)
            {
                GRBLinExpr P = 0;

                for (int a = 0; a < i; a = a + 1)
                {
                    P = P + Tcij[a, b];
                }
                disp.AddConstr((vectores.Dj[b] * vectores.Uj[b]), GRB.LESS_EQUAL, P, "tonelaje_minimo_de_palas" + b);
            }



            for (int a = 0; a < i; a = a + 1)
            {
                GRBLinExpr P = 0;

                for (int b = 0; b < j; b = b + 1)
                {
                    P = P + Tcij[a, b];
                }
                disp.AddConstr(P, GRB.LESS_EQUAL, vectores.RUi[a] * vectores.Ui[a], "tonelaje_minimo_a_basureros" + a);
            }

            //Restriccion del chancador independiente

            for (int a = 1; a < i; a = a + 1)
            {
                GRBLinExpr P = 0;

                for (int b = 0; b < j; b = b + 1)
                {
                    P = P + Tcij[a, b];
                }
                disp.AddConstr(vectores.RLi[a] * vectores.Ui[a], GRB.LESS_EQUAL, P, "tonelajecargado2" + a);
            }

            for (int a = 0; a < i; a = a + 1)
            {
                GRBLinExpr P = 0;

                for (int b = 0; b < j; b = b + 1)
                {
                    P = P + Tcij[a, b];
                }
                disp.AddConstr(vectores.Ui[a]*(vectores.RLi[a] - (vectores.PlTcmalo[0,3] * (1-vectores.Uj[3]) + vectores.PlTcmalo[0, 4] * (1-vectores.Uj[4]) + vectores.PlTcmalo[0, 5] * (1-vectores.Uj[5]) + vectores.PlTcmalo[0, 16] * (1-vectores.Uj[16]) + vectores.PlTcmalo[0, 17] * (1-vectores.Uj[17]))), GRB.LESS_EQUAL, P, "tonelajecargado2" + a);
            }




            GRBLinExpr Qonda = 0;

            for (int a = 0; a < i; a = a + 1)
            {
                for (int b = 0; b < j; b = b + 1)
                {
                    Qonda = (Qonda + Ycij[a, b] + Yvij[a, b]);
                }
            }

            disp.AddConstr(Qonda, GRB.LESS_EQUAL, M, "maximocamiones");



            for (int b = 0; b < j; b = b + 1)
            {
                GRBLinExpr P1 = 0;
                GRBLinExpr P2 = 0;

                for (int a = 0; a < i; a = a + 1)
                {
                    P1 = P1 + Tcij[a, b];
                    P2 = P2 + Tvij[a, b];
                }
                disp.AddConstr(P1, GRB.EQUAL, P2, "conservaciondeflujo" + i);

            }


            for (int a = 0; a < i; a = a + 1)
            {
                GRBLinExpr P1 = 0;
                GRBLinExpr P2 = 0;

                for (int b = 0; b < j; b = b + 1)
                {
                    P1 = P1 + Tcij[a, b];
                    P2 = P2 + Tvij[a, b];
                }

                disp.AddConstr(P1, GRB.EQUAL, P2, "conservaciondeflujo2" + i);
            }


            GRBLinExpr Pobj = 0;

            for (int a = 0; a < i; a = a + 1)
            {
                for (int b = 0; b < j; b = b + 1)
                {
                    Pobj = Pobj + Ycij[a, b] + Yvij[a, b];
                }


            }

            disp.Update();

            disp.SetObjective(Pobj, GRB.MINIMIZE);

            disp.Optimize();


            double sumayc = 0;
            double sumayv = 0;
            double sumatc = 0;
            double sumatv = 0;

            for (int a = 0; a < i; a = a + 1)
            {

                double sumajyc = 0;
                double sumajyv = 0;
                double sumajtc = 0;
                double sumajtv = 0;

                for (int b = 0; b < j; b = b + 1)
                {

                    vectores.PlYc[a, b] = Convert.ToDouble(Ycij[a, b].X);
                    vectores.PlYv[a, b] = Convert.ToDouble(Yvij[a, b].X);
                    vectores.PlTc[a, b] = Convert.ToDouble(Tcij[a, b].X);
                    vectores.PlTv[a, b] = Convert.ToDouble(Tvij[a, b].X);
                    sumayc = sumayc + vectores.PlYc[a, b]; //suma todos los flujos llenos
                    sumayv = sumayv + vectores.PlYv[a, b]; //suma todos los flujos vacios
                    sumatc = sumatc + vectores.PlTc[a, b]; //suma todos los camiones llenos
                    sumatv = sumatv + vectores.PlTv[a, b]; //suma todos los camiones vacios
                    sumajyc = sumajyc + Convert.ToDouble(Ycij[a, b].X); //suma todo el flujo cargado al botadero "a"
                    sumajyv = sumajyv + Convert.ToDouble(Yvij[a, b].X); //suma todo el flujo vacio al botadero "a"
                    sumajtc = sumajtc + Convert.ToDouble(Tcij[a, b].X); //suma todos los camiones cargados yendo a botadero "a"
                    sumajtv = sumajtv + Convert.ToDouble(Tvij[a, b].X); //suma todos los camiones vacios yendo a botadero "a"

                }

                vectores.PlYc[a, 20] = sumajyc; // deja total
                vectores.PlYv[a, 20] = sumajyv; //
                vectores.PlTc[a, 20] = sumajtc; //
                vectores.PlTv[a, 20] = sumajtv; //
            }

            for (int b = 0; b < j; b = b + 1)
            {

                double sumaiyc = 0;
                double sumaiyv = 0;
                double sumaitc = 0;
                double sumaitv = 0;

                for (int a = 0; a < i; a = a + 1)
                {

                    sumaiyc = sumaiyc + Convert.ToDouble(Ycij[a, b].X);
                    sumaiyv = sumaiyv + Convert.ToDouble(Yvij[a, b].X);
                    sumaitc = sumaitc + Convert.ToDouble(Tcij[a, b].X);
                    sumaitv = sumaitv + Convert.ToDouble(Tvij[a, b].X);

                }

                vectores.PlYc[30, b] = sumaiyc;
                vectores.PlYv[30, b] = sumaiyv;
                vectores.PlTc[30, b] = sumaitc;
                vectores.PlTv[30, b] = sumaitv;
            }

            vectores.PlYc[30, 20] = sumayc;
            vectores.PlYv[30, 20] = sumayv;
            vectores.PlTc[30, 20] = sumatc;
            vectores.PlTv[30, 20] = sumatv;

            _timenow.StateValue = vectores.PlTc[0, 20];
            sr.serializa(vectores);
            sr.closeStream();

            disp.Dispose();
            env.Dispose();

    



            return ExitType.FirstExit;
        }

        #endregion
    }
}
