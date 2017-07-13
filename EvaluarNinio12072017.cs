

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoZ
{

    public class EvaluarNiños
    {
        /// <summary>
        /// Definiciones
        /// </summary>
        public DateTime fechaNacimiento { get; set; }
        public DateTime fechaAntropometria { get; set; }
        public string sexo { get; set; }
        public decimal peso { get; set; }
        public decimal talla { get; set; }
        public decimal IMC {
            get
            {
                return (peso / (talla * talla)) * 10000;
            }
        }
        public decimal edadMeses {
            get
            {
                //Obtengo la diferencia en años.
                int edad = fechaAntropometria.Year - fechaNacimiento.Year;

                //Obtengo la fecha de cumpleaños de este año.
                DateTime nacimientoAhora = fechaNacimiento.AddYears(edad);
                //Le resto un año si la fecha actual es anterior 
                //al día de nacimiento.
                if (DateTime.Now.CompareTo(nacimientoAhora) < 0)
                {
                    edad--;
                }
                return edad;
            }
        }
        public int edadMesesCumplidos
        {
            get
            {
                int edad;
                //Obtengo la diferencia en años.
                int edadAños = fechaAntropometria.Year - fechaNacimiento.Year;

                //Obtengo la fecha de cumpleaños de este año.
                //DateTime nacimientoAhora = fechaNacimiento.AddYears(edad);
                //Le resto un año si la fecha actual es anterior 
                if (fechaAntropometria.Month < fechaNacimiento.Month || (fechaAntropometria.Month == fechaNacimiento.Month && fechaAntropometria.Day < fechaNacimiento.Day))
                    edadAños--;

                int edadMeses = fechaAntropometria.Month - fechaNacimiento.Month;
                if (fechaAntropometria.Day < fechaNacimiento.Day)
                    edadMeses--;
                if (edadMeses < 0)
                    edadMeses += 12;
                edad = edadMeses + (edadAños * 12);
                return edad;
            }
        }

        public int edadDiasCumplidos
        {
            get
            {
                TimeSpan diferencia;
                diferencia = fechaAntropometria - fechaNacimiento;

                return diferencia.Days;
            }
        }

        /// <summary>
        /// Las siguientes propiedades corresponden solo a niños menores de 5 años
        /// </summary>
        public decimal perimetro_cefalico { get; set; }
        public decimal perimetro_braquial { get; set; }
        public decimal pliegue_cutaneo_subescapular { get; set; }
        public decimal pliegue_cutaneo_triceps { get; set; }
        public bool parado { get; set; }
     

        decimal MathAbs_Double(decimal x)
        {
            return ((x >= 0) ? x : -x);
        }
        decimal MathPow_Double_Int(decimal x, int n)
        {
            decimal ret;
            if ((x == (decimal)1.0) || (n == 1))
            {
                ret = x;
            }
            else if (n < 0)
            {
                ret = (decimal)1.0 / MathPow_Double_Int(x, -n);
            }
            else {
                ret = (decimal)1.0;
                while (n!=0)
                {
                    ret *= x;
                    n--;
                }
            }
            return (ret);
        }

        decimal MathLn_Double(decimal x)
        {
            decimal ret = (decimal)0.0, d;
            if (x > 0)
            {
                int n = 0;
                do
                {
                    int a = 2 * n + 1;
                    d = (decimal)(1.0 / a) * MathPow_Double_Int((x - 1) / (x + 1), a);
                    ret += d;
                    n++;
                } while (MathAbs_Double(d) >(decimal) 1.0E-300);
            }
            else {
                return 0;
            }
            return (ret * 2);
        }

        decimal MathExp_Double(decimal x)
        {
            decimal ret;
            if (x == (decimal) 1.0)
            {
                ret = (decimal)2.7182818284590452353602874713527;
            }
            else if (x < 0)
            {
                ret = (decimal)1.0 / MathExp_Double(-x);
            }
            else {
                int n = 2;
                decimal d;
                ret = (decimal)1.0 + x;
                do
                {
                    d = x;
                    for (int i = 2; i <= n; i++)
                    {
                        d *= x / i;
                    }
                    ret += d;
                    n++;
                } while (d > (decimal)1.0E-300);
            }
            return (ret);
        }

        decimal MathPow_Double_Double(decimal x, decimal a)
        {
            decimal ret;
            if ((x == (decimal)1.0) || (a == (decimal)1.0))
            {
                ret = x;
            }
            else if (a < 0)
            {
                ret = (decimal)1.0 / MathPow_Double_Double(x, -a);
            }
            else {
                ret = MathExp_Double(a * MathLn_Double(x));
            }
            return (ret);
        }


        public decimal calculaZ(decimal Y , Dictionary<string, decimal> dato)
        {
            decimal Z = (MathPow_Double_Double(((decimal)Y / (decimal)(dato["M"])), (decimal)(dato["L"])) - 1) / (decimal)((decimal)(dato["S"]) * (decimal)(dato["L"]));
            decimal ZT;
            //si no cumple con el rango se hacen los ajustes

            decimal sd3Pos = ((dato["M"] ) * MathPow_Double_Double((1 + (dato["L"] ) * (dato["S"] ) * 3), 1 / ((dato["L"] ))));
            decimal sd3Neg = ((dato["M"] ) * MathPow_Double_Double((1 + (dato["L"] ) * (dato["S"] ) * (-3)), 1 / ((dato["L"] ))));
            decimal sd23Pos = ((dato["M"] ) * MathPow_Double_Double((1 + (dato["L"] ) * (dato["S"] ) * 3), 1 / ((dato["L"] )))) - ((dato["M"] ) * MathPow_Double_Double((1 + (dato["L"] ) * (dato["S"] ) * 2), 1 / ((dato["L"] ))));
            decimal sd23Neg = ((dato["M"] ) * MathPow_Double_Double((1 + (dato["L"] ) * (dato["S"] ) * (-2)), 1 / ((dato["L"] )))) - ((dato["M"] ) * MathPow_Double_Double((1 + (dato["L"] ) *(dato["S"] ) * (-3)), 1 / ((dato["L"] ))));
            if (Z > 3)
            {
                // ZT = 3 + ((Y - dato["SD3"]) / (dato["SD3"] - dato["SD2"]));
                ZT = 3 + ((Y - sd3Pos) / sd23Pos);

            }
            else if (Z < -3)
            {
                //ZT = -3 + ((Y - dato["SD3neg"]) / (dato["SD2neg"] - dato["SD3neg"]));
                ZT = -3 + ((Y - sd3Neg) / sd23Neg);
            }
            else
                ZT = Z;
            return ZT;
        }

        /// <summary>
        /// metodo que calcula el indice Z en base a la medida del peso del niño
        /// si el indice requiere un ajuste (si z mayor que 3 o menor que (menos)3) se realiza el ajuste
        /// </summary>
        /// <returns>regresa el valor del indice Z</returns>
        public decimal puntuacionZPesoEdad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 120)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdad(this.edadDiasCumplidos, this.sexo, 'p');
            if (Dato.Count<5)
                return -6;
            return calculaZ(peso, Dato);
        }

        public decimal puntuacionZTallaEdad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 228)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdad(this.edadDiasCumplidos, this.sexo, 't');
            if (Dato.Count<5)
                return -6;
            return calculaZ(talla, Dato);

        }

        public decimal puntuacionZ_IMCEdad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 228)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = getRowByEdad(edadDiasCumplidos, sexo, 'm');
            if (Dato.Count<5)
                return -6;
            return calculaZ(IMC, Dato);
        }

        public decimal puntuacionZPesoLong()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (talla <45 || talla > 110 || edadMesesCumplidos>24 )
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdad(this.talla, this.sexo, 'l');
            if (Dato.Count<5)
                return -6;
            return calculaZ(peso, Dato);
        }

        public decimal puntuacionZPesoTalla()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (talla < 65 || talla > 120 || edadMesesCumplidos > 61 || edadMesesCumplidos < 24)
                return -6;

            decimal tallaAux = this.talla;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdad(tallaAux, this.sexo, 'e');
            if (Dato.Count<5)
                return -6;
            return calculaZ(peso, Dato);
        }

        public decimal puntuacionZ_PC_edad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 61)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = getRowByEdad(edadMesesCumplidos, sexo, 'c');
            if (Dato.Count<5)
                return -6;
            return calculaZ(perimetro_cefalico, Dato);
        }

        public decimal puntuacionZ_PB_edad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 61 && edadMesesCumplidos < 3)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = getRowByEdad(edadMesesCumplidos, sexo, 'b');
            if (Dato == null)
                return -6;
            return calculaZ(perimetro_braquial, Dato);
        }

        public decimal puntuacionZ_PCS_edad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 61 && edadMesesCumplidos < 3)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = getRowByEdad(edadMesesCumplidos, sexo, 's');
            if (Dato == null)
                return -6;
            return calculaZ(pliegue_cutaneo_subescapular, Dato);
        }

        public decimal puntuacionz_PCT_edad()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 61 && edadMesesCumplidos < 3)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = getRowByEdad(edadMesesCumplidos, sexo, 'r');
            if (Dato == null)
                return -6;
            return calculaZ(pliegue_cutaneo_triceps, Dato);
        }


        ////PUNTUACION BASADA EN DIAS DE VIDA
        public decimal puntuacionZPesoEdadD()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 120)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdadDias(this.edadDiasCumplidos, this.sexo, 'p');
            if (Dato.Count < 5)
                return -6;
            return calculaZ(peso, Dato);
        }

        public decimal puntuacionZTallaEdadD()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos > 228)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdadDias(this.edadDiasCumplidos, this.sexo, 't');
            if (Dato.Count < 5)
                return -6;
            return calculaZ(talla, Dato);

        }
        
        public decimal puntuacionZPesoTallaD()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (talla < 65 || talla > 110 )
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, decimal> Dato = this.getRowByEdadDias(this.talla, this.sexo, 's');
            if (Dato.Count < 5)
                return -6;
            return calculaZ(peso, Dato);
        }

        /// <summary>
        /// Metodo que onbtiene el estado nutricional del niño en base a su indice Z y su peso
        /// </summary>
        /// <returns>segresa una cadena de caracter con la descripcion de su estado nutricional,
        /// tambien puede ser que no aplique para el niño si es que no estaba entre los rangos de edad
        /// o si su indice Z tampoco esta dentro de los rangos establecidos (ente 5 y (menos) 5)</returns>
        public string estadoNutricionPeso()
        {
            decimal Z = this.puntuacionZPesoEdad();
            if (Z > -5 && Z <= -3)
                return "GRAVE";
            else if (Z > -3 && Z <= -2)
                return "MODERADO";
            else if (Z > -2 && Z <= -1)
                return "LEVE";
            else if (Z > -1 && Z <= 1)
                return "NORMAL";
            else if (Z > 1 && Z <= 2)
                return "SOBREPESO";
            else if (Z > 2 && Z < 5)
                return "OBESIDAD";
            else
                return "SIN DIAGNÓSTICO";

        }

        public string estadoNutricionTalla()
        {
            decimal Z = this.puntuacionZTallaEdad();
            if (Z > -5 && Z <= -3)
                return "GRAVE";
            else if (Z > -3 && Z <= -2)
                return "MODERADO";
            else if (Z > -2 && Z <= -1)
                return "LEVE";
            else if (Z > -1 && Z < 5)
                return "NORMAL";
            else
                return "SIN DIAGNÓSTICO";

        }
        /// <summary>
        /// metodo que obtiene los valores necesarios para el calculo del indice Z 
        /// dependiendo del genero, la edad y la medida que se quiere generar (peso, talla o IMC)
        /// </summary>
        /// <param name="edad">La edad del niño expresada en MESES</param>
        /// <param name="sexo">El sexo del niño, solo puede ser M o H</param>
        /// <param name="medida">El tipo de medida del que se quiere sacar el indice Z (Peso:p ; Talla:t ; IMC:m)</param>
        /// <returns></returns>

        public string estadoNutricionPT()
        {
            decimal Z;
            if (this.edadMesesCumplidos < 24)
               Z = this.puntuacionZPesoLong();
            else
                Z = this.puntuacionZPesoTalla();
            if (Z >= -6 && Z <= -3)
                return "GRAVE";
            else if (Z > -3 && Z <= -2)
                return "MODERADO";
            else if (Z > -2 && Z <= -1)
                return "LEVE";
            else if (Z > -1 && Z <= 1)
                return "NORMAL";
            else if (Z > 1 && Z <= 2)
                return "SOBREPESO";
            else if (Z > 2 && Z <= 5)
                return "OBESIDAD";
            else
                return "SIN DIAGNÓSTICO";
        }

        public Dictionary<string, decimal> getRowByEdad(decimal indice, string sexo, char medida)
        {
            decimal redondeo = Math.Round(indice,1);
            //if (redondeo > indice)
            //    indice = redondeo;
            //else if (redondeo < indice)
            //    indice = redondeo + (decimal)0.5;  
            ///se inicializa el dato de tipo "diccionario que contiene los valores con su respeciva etiqueta
            /// (L,M,S,StDev,SD5neg,SD4neg,SD3neg,SD2neg,SD1neg,SD0,SD1,SD2,SD3,SD4)
           // Dictionary<string, decimal> dato = new Dictionary<string, decimal>();
            switch (sexo) {
                ///si el sexo del niño es Femenino
                case "M":
                    if (this.edadMesesCumplidos < 61)
                    {
                        //dependiendo de la medida base
                        switch (medida)
                        {
                            case 'p':///consulta el peso

                                var Database_PeGirls = GetResourceStream("tablas_grandes/wfa_girls_p_exp.txt");
                                ///abre el archivo de texto con los datos en base al Peso
                                return ReadTable( indice, Database_PeGirls);
                                ///lo anterior se aplica para cada caso cambiando solo el archivo de texto que 
                                /// se va a consultar dependiendo del genero y el tipo de medida
                            
                            case 't':
                                if (edadMesesCumplidos>24)
                                {
                                    var Database_TeGirls = GetResourceStream("Tablas_grandes/lhfa_girls_p_exp.txt");
                                    return ReadTable( indice, Database_TeGirls);
                                }
                                else
                                {
                                    var Database_TeGirls = GetResourceStream("Tablas_grandes/lhfa_girls_p_exp.txt");
                                    return ReadTable( indice, Database_TeGirls);
                                }
                            case 'm':

                                if (edadMesesCumplidos >24)
                                {
                                    var DatabaseGirl = GetResourceStream("Tablas_grandes/bfa_girls_p_exp.txt");
                                    return ReadTable( indice, DatabaseGirl);
                                }
                                else
                                {
                                    var DatabaseGirl = GetResourceStream("Tablas_grandes/bfa_girls_p_exp.txt");
                                    return ReadTable( indice, DatabaseGirl);
                                }
                            case 'l':

                                if (edadMesesCumplidos > 24)
                                {
                                    if (talla >= 65 && talla <= 120)
                                    {
                                        var Databaselong = GetResourceStream("Tablas_grandes/wfh_girls_p_exp.txt");
                                        return ReadTable(indice, Databaselong);
                                    }
                                }
                                else
                                {
                                    if (talla >= 45 && talla <= 110)
                                    {
                                        var Databaselong = GetResourceStream("Tablas_grandes/wfl_girls_p_exp.txt");
                                        return ReadTable(indice, Databaselong);
                                    }
                                }
                                    return null;

                            case 'e':
                                var DatabaseGirls = GetResourceStream("menores_de_5/wfh_girls_2_5_zscores.txt");
                                return ReadTable( indice, DatabaseGirls);

                            case 'c':
                                var DatabaseC = GetResourceStream("menores_de_5/tab_hcfa_girls_z_0_5.txt");
                                return ReadTable( indice, DatabaseC);
                            case 'b':
                                var DatabaseB = GetResourceStream("menores_de_5/tab_acfa_girls_z_3_5.txt");
                                return ReadTable( indice, DatabaseB);
                            case 's':
                                var DatabaseS = GetResourceStream("menores_de_5/tab_ssfa_girls_z_3_5.txt");
                                return ReadTable( indice, DatabaseS);
                            case 'r':
                                var DatabaseT = GetResourceStream("menores_de_5/tab_tsfa_girls_z_3_5.txt");
                                return ReadTable( indice, DatabaseT);
                            default:
                                return null;
                        }

                    }
                    else
                    {
                        //dependiendo de la medida base
                        switch (medida)
                        {
                            case 'p':///consulta el peso

                                var Database= GetResourceStream("tablas_grandes/wfa_girls_p_exp.txt");
                                ///abre el archivo de texto con los datos en base al Peso
                                return ReadTable( indice, Database);
                                ///lo anterior se aplica para cada caso cambiando solo el archivo de texto que 
                                /// se va a consultar dependiendo del genero y el tipo de medida

                            case 't':
                                var Database_TeGirls = GetResourceStream("Tablas_grandes/lhfa_girls_p_exp.txt");
                                return ReadTable( indice, Database_TeGirls);
                            case 'm':

                                var Database_BmiGirls = GetResourceStream("tablas_grandes/bfa_girls_p_exp.txt");
                                return ReadTable( indice, Database_BmiGirls);
                            default:
                                return null;
                        }
                    }
                case "H":
                    if (this.edadMesesCumplidos < 61)
                    {
                        //dependiendo de la medida base
                        switch (medida)
                        {
                            case 'p':///consulta el peso

                                var Database = GetResourceStream("tablas_grandes/wfa_boys_p_exp.txt");
                                ///abre el archivo de texto con los datos en base al Peso
                                return ReadTable( indice, Database);
                                ///lo anterior se aplica para cada caso cambiando solo el archivo de texto que 
                                /// se va a consultar dependiendo del genero y el tipo de medida

                            case 't':
                                if (edadMesesCumplidos > 24)
                                {
                                    var Database_Teboys = GetResourceStream("tablas_grandes/lhfa_boys_p_exp.txt");
                                    return ReadTable( indice, Database_Teboys);
                                }
                                else
                                {
                                    var Database_Teboys = GetResourceStream("tablas_grandes/lhfa_boys_p_exp.txt");
                                    return ReadTable( indice, Database_Teboys);
                                }
                            case 'm':

                                if (edadMesesCumplidos > 24)
                                {
                                    var Databaseboys = GetResourceStream("tablas_grandes/bfa_boys_p_exp.txt");
                                    return ReadTable( indice, Databaseboys);
                                }
                                else
                                {
                                    var Databaseboys = GetResourceStream("tablas_grandes/bfa_boys_p_exp.txt");
                                    return ReadTable( indice, Databaseboys);
                                }
                            case 'l':
                                if (edadMesesCumplidos > 24)
                                {
                                    if (talla >= 65 && talla <= 120)
                                    {
                                        var Databaselong = GetResourceStream("tablas_grandes/wfh_boys_p_exp.txt");
                                        return ReadTable(indice, Databaselong);
                                    }
                                }
                                else
                                {
                                    if (talla >= 45 && talla < 110)
                                    {
                                        var Databaselong = GetResourceStream("tablas_grandes/wfl_boys_p_exp.txt");
                                        return ReadTable(indice, Databaselong);
                                    }
                                }
                                    return null;
                            case 'e':
                                var DatabaseGirls = GetResourceStream("menores_de_5/wfh_boys_2_5_zscores.txt");
                                return ReadTable( indice, DatabaseGirls);

                            case 'c':
                                var DatabaseC = GetResourceStream("menores_de_5/tab_hcfa_boys_z_0_5.txt");
                                return ReadTable( indice, DatabaseC);
                            case 'b':
                                var DatabaseB = GetResourceStream("menores_de_5/tab_acfa_boys_z_3_5.txt");
                                return ReadTable( indice, DatabaseB);
                            case 's':
                                var DatabaseS = GetResourceStream("menores_de_5/tab_ssfa_boys_z_3_5.txt");
                                return ReadTable( indice, DatabaseS);
                            case 'r':
                                var DatabaseT = GetResourceStream("menores_de_5/tab_tsfa_boys_z_3_5.txt");
                                return ReadTable( indice, DatabaseT);

                            default:
                                return null;
                        }
                    }
                    else
                    {
                        switch (medida)
                        {
                            case 'p':

                                var Database_PeBoys = GetResourceStream("tablas_grandes/wfa_boys_p_exp.txt");
                                return ReadTable( indice,Database_PeBoys);
                            case 't':

                                var Database_TeBoys = GetResourceStream("tablas_grandes/lhfa_boys_p_exp.txt");
                                return ReadTable( indice, Database_TeBoys);
                            case 'm':

                                var Database_BmiBoys = GetResourceStream("tablas_grandes/bfa_boys_p_exp.txt");
                                return ReadTable( indice, Database_BmiBoys);
                            default:
                                return null;
                        }
                    }
                default:
                    return null;
            }
        }

        public Dictionary<string, decimal> getRowByEdadDias(decimal indice, string sexo, char medida)
        {
            ///se inicializa el dato de tipo "diccionario que contiene los valores con su respeciva etiqueta
            /// (L,M,S,StDev,SD5neg,SD4neg,SD3neg,SD2neg,SD1neg,SD0,SD1,SD2,SD3,SD4)
            Dictionary<string, decimal> dato = new Dictionary<string, decimal>();
            switch (sexo)
            {
                ///si el sexo del niño es Femenino
                case "M":
                    if (this.edadDiasCumplidos < 1856)
                    {
                        //dependiendo de la medida base
                        switch (medida)
                        {
                            case 'p':///consulta el peso

                                var Database_PeGirls = GetResourceStream("menores_de_5/pesoedad_ninas_z.txt");
                                ///abre el archivo de texto con los datos en base al Peso
                                return ReadTable( indice, Database_PeGirls);
                            ///lo anterior se aplica para cada caso cambiando solo el archivo de texto que 
                            /// se va a consultar dependiendo del genero y el tipo de medida

                            case 't':
                                
                                var Database_TeGirls = GetResourceStream("menores_de_5/tallaedad_ninas_z.txt");
                                return ReadTable( indice, Database_TeGirls);
                               
                            case 's':
                                if (talla >= 45 && talla < 110)
                                {
                                    var DatabaseGirl = GetResourceStream("menores_de_5/pesotalla_ninas_z.txt");
                                    return ReadTable( indice, DatabaseGirl);
                                }
                                else
                                    return dato;
                            default:
                                return dato;
                        }

                    }
                    else
                    {
                        return dato;
                    }
                case "H":
                    if (this.edadDiasCumplidos < 1856)
                    {
                        //dependiendo de la medida base
                        switch (medida)
                        {
                            case 'p':///consulta el peso

                                var Database_PeGirls = GetResourceStream("menores_de_5/pesoedad_ninos_z.txt");
                                ///abre el archivo de texto con los datos en base al Peso
                                return ReadTable( indice, Database_PeGirls);
                            ///lo anterior se aplica para cada caso cambiando solo el archivo de texto que 
                            /// se va a consultar dependiendo del genero y el tipo de medida

                            case 't':

                                var Database_TeGirls = GetResourceStream("menores_de_5/tallaedad_ninos_z.txt");
                                return ReadTable( indice, Database_TeGirls);

                            case 's':

                                if (talla >= 45 && talla < 110)
                                {
                                    var DatabaseGirl = GetResourceStream("menores_de_5/pesotalla_ninos_z.txt");
                                    return ReadTable( indice, DatabaseGirl);
                                }
                                else
                                    return dato;

                            default:
                                return dato;
                        }
                    }
                    else
                    {
                        return dato;
                    }
                default:
                    return dato;
            }
        }

        /// <summary>
        /// metodo necesario para consumir recursos internos del mismo dll los cuales son la base
        /// de datos de OMS
        /// </summary>
        /// <param name="resName">Nombre del recuros interno a consultar (el nombre del archivo txt)</param>
        /// <returns>regresa un espacio de memoria que se puede usar para leer el archivo</returns>
        private static UnmanagedMemoryStream GetResourceStream(string resName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var strResources = assembly.GetName().Name + ".g.resources";
            var rStream = assembly.GetManifestResourceStream(strResources);
            var resourceReader = new ResourceReader(rStream);
            var items = resourceReader.OfType<DictionaryEntry>();
            var stream = items.First(x => (x.Key as string) == resName.ToLower()).Value;
            return (UnmanagedMemoryStream)stream;
        }
        private static Dictionary<string, decimal> ReadTable(decimal indice, UnmanagedMemoryStream database)
        {
            int i = 0;
            Dictionary<string, decimal> dato = new Dictionary<string, decimal>();
            using (StreamReader stream = new StreamReader(database))
            {
                ///se obtienen las columnas
                var Columnas = stream.ReadLine().Split('\t');

                ///se obtienen los primeros valores
                var Valores = stream.ReadLine().Split('\t');

                ///hasta que se encuentre el valor que corresponda a la edad en meses
                while (Convert.ToDecimal(Valores[0]) != indice)
                {
                    try
                    {
                        Valores = stream.ReadLine().Split('\t');

                    }
                    catch (Exception e)
                    {
                        var ex = e.Message;
                       
                    }
                }

                ///se genera el diccionario que sera la respuesta de este metodo
                foreach (string campo in Columnas)
                {
                    dato.Add(campo, Convert.ToDecimal(Valores[i]));
                    i++;
                }
            }
            return dato;
        }
    }
}
