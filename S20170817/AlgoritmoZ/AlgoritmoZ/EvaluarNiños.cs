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
        public DateTime fechaNacimiento { get; set; }
        public DateTime fechaAntropometria { get; set; }
        public string sexo { get; set; }
        public double peso { get; set; }
        public double talla { get; set; }
        public double IMC {
            get
            {
                return (peso /( talla * talla))*10000;
            }            
        }
        public double edadMeses {
            get
            {
                return (((fechaAntropometria.Year - fechaNacimiento.Year) * 12) + (fechaAntropometria.Month - fechaNacimiento.Month));
            }
        }
        public int edadMesesCumplidos
        {
            get
            {
                return (int)(edadMeses - (double)0.99);
            }
        }

        /// <summary>
        /// metodo que calcula el indice Z en base a la medida del peso del niño
        /// si el indice requiere un ajuste (si z mayor que 3 o menor que (menos)3) se realiza el ajuste
        /// </summary>
        /// <returns>regresa el valor del indice Z</returns>
        public double puntuacionZPeso()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos < 61 || edadMesesCumplidos > 120)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, double> Dato = this.getRowByEdad(this.edadMesesCumplidos, this.sexo, 'p');
            double Z = ((double)Math.Pow((peso / Dato["M"]), Dato["L"]) - 1) / (Dato["S"] * Dato["L"]);
            double ZT;
            //si no cumple con el rango se hacen los ajustes
            if (Z > 3)
            {
                ZT = 3 + ((this.peso - Dato["SD3"]) / (Dato["SD3"] - Dato["SD2"]));
            }
            else if (Z < -3)
            {
                ZT = -3 + ((this.peso - Dato["SD3neg"]) / (Dato["SD2neg"] - Dato["SD3neg"]));
            }
            else
                ZT = Z;
            return ZT;

        }

        public double puntuacionZTalla()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos < 61 || edadMesesCumplidos > 228)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, double> Dato = this.getRowByEdad(this.edadMesesCumplidos, this.sexo, 't');
            double Z = ((double)Math.Pow((talla / Dato["M"]), Dato["L"]) - 1) / (Dato["S"] * Dato["L"]);
            double ZT;
            //si no cumple con el rango se hacen los ajustes
            if (Z > 3)
            {
                ZT = 3 + ((talla - Dato["SD3"]) / (Dato["SD3"] - Dato["SD2"]));
            }
            else if (Z < -3)
            {
                ZT = (-3) + ((talla - Dato["SD3neg"]) / (Dato["SD2neg"] - Dato["SD3neg"]));
            }
            else
                ZT = Z;
            return ZT;

        }

        public double puntuacionZ_IMC()
        {
            ///si la edad del niño no esta dentro del rango, no se toma en cuenta
            if (edadMesesCumplidos < 61 || edadMesesCumplidos > 228)
                return -6;
            //se obtienen los datos LMS de acuerdo a la edad en meses del niño
            Dictionary<string, double> Dato = getRowByEdad(edadMesesCumplidos, sexo, 'm');
            double Z = (Math.Pow((IMC / Dato["M"]), Dato["L"]) - 1) / (Dato["S"] * Dato["L"]);
            double ZT;
            //si no cumple con el rango se hacen los ajustes
            if (Z > 3)
            {
                ZT = 3 + ((IMC - Dato["SD3"]) / (Dato["SD3"] - Dato["SD2"]));
            }
            else if (Z < -3)
            {
                ZT = -3 + ((IMC - Dato["SD3neg"]) / (Dato["SD2neg"] - Dato["SD3neg"]));
            }
            else
                ZT = Z;
            return ZT;

        }

        /// <summary>
        /// Metodo que onbtiene el estado nutricional del niño en base a su indice Z y su peso
        /// </summary>
        /// <returns>segresa una cadena de caracter con la descripcion de su estado nutricional,
        /// tambien puede ser que no aplique para el niño si es que no estaba entre los rangos de edad
        /// o si su indice Z tampoco esta dentro de los rangos establecidos (ente 5 y (menos) 5)</returns>
        public string estadoNutricionPeso()
        {
            double Z = this.puntuacionZPeso();
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
                return "No VALIDO";

        }

        public string estadoNutricionTalla()
        {
            double Z = this.puntuacionZTalla();
            if (Z > -5 && Z <= -3)
                return "GRAVE";
            else if (Z > -3 && Z <= -2)
                return "MODERADO";
            else if (Z > -2 && Z <= -1)
                return "LEVE";
            else if (Z > -1 && Z < 5)
                return "NORMAL";
            else
                return "No VALIDO";

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

        /// <summary>
        /// metodo que obtiene los valores necesarios para el calculo del indice Z 
        /// dependiendo del genero, la edad y la medida que se quiere generar (peso, talla o IMC)
        /// </summary>
        /// <param name="edad">La edad del niño expresada en MESES</param>
        /// <param name="sexo">El sexo del niño, solo puede ser M o H</param>
        /// <param name="medida">El tipo de medida del que se quiere sacar el indice Z (Peso:p ; Talla:t ; IMC:m)</param>
        /// <returns></returns>
        public Dictionary<string, double> getRowByEdad (int edad, string sexo, char medida)
        {
            ///se inicializa el dato de tipo "diccionario que contiene los valores con su respeciva etiqueta
            /// (L,M,S,StDev,SD5neg,SD4neg,SD3neg,SD2neg,SD1neg,SD0,SD1,SD2,SD3,SD4)
            Dictionary<string, double> dato = new Dictionary<string, double>();

            ///contador de indice comun
            int i = 0;

            ///si el seño del niño es Femenino
            if (sexo == "M") {

                //dependiendo de la medida base
                switch (medida)
                {
                    case 'p':///consulta el peso

                        var Database_PeGirls = GetResourceStream("PE_girls_z_WHO2007_exp.txt");
                        ///abre el archivo de texto con los datos en base al Peso
                        using (StreamReader stream = new StreamReader(Database_PeGirls))
                        {
                            ///se obtienen las columnas
                            var Columnas = stream.ReadLine().Split('\t');

                            ///se obtienen los primeros valores
                            var Valores = stream.ReadLine().Split('\t');

                            ///hasta que se encuentre el valor que corresponda a la edad en meses
                            while(Convert.ToInt32(Valores[0])!=edad) {
                                Valores = stream.ReadLine().Split('\t');
                            }

                            ///se genera el diccionario que sera la respuesta de este metodo
                            foreach(string campo in Columnas)
                            {
                                dato.Add(campo,Convert.ToDouble(Valores[i]));
                                i++;
                            }
                        }
                        return dato;

                        ///lo anterior se aplica para cada caso cambiando solo el archivo de texto que 
                        /// se va a consultar dependiendo del genero y el tipo de medida

                    case 't':

                        var Database_TeGirls = GetResourceStream("TE_girls_z_WHO2007_exp.txt");
                        using (StreamReader stream = new StreamReader(Database_TeGirls))
                        {
                            var Columnas = stream.ReadLine().Split('\t');
                            var Valores = stream.ReadLine().Split('\t');
                            while (Convert.ToInt32(Valores[0]) != edad)
                            {
                                Valores = stream.ReadLine().Split('\t');
                            }
                            foreach (string campo in Columnas)
                            {
                                dato.Add(campo, Convert.ToDouble(Valores[i]));
                                i++;
                            }
                        }
                        return dato;
                    case 'm':

                        var Database_BmiGirls = GetResourceStream("bmi_girls_z_WHO2007_exp.txt");
                        using (StreamReader stream = new StreamReader(Database_BmiGirls))
                        {
                            var Columnas = stream.ReadLine().Split('\t');
                            var Valores = stream.ReadLine().Split('\t');
                            while (Convert.ToInt32(Valores[0]) != edad)
                            {
                                Valores = stream.ReadLine().Split('\t');
                            }
                            foreach (string campo in Columnas)
                            {
                                dato.Add(campo, Convert.ToDouble(Valores[i]));
                                i++;
                            }
                        }
                        return dato;
                    default:
                        return dato;
                }
            
            }else
            {
                switch (medida)
                {
                    case 'p':

                        var Database_PeBoys = GetResourceStream("PE_boys_z_WHO2007_exp.txt");
                        using (StreamReader stream = new StreamReader(Database_PeBoys))
                        {
                            var Columnas = stream.ReadLine().Split('\t');
                            var Valores = stream.ReadLine().Split('\t');
                            while (Convert.ToInt32(Valores[0]) != edad)
                            {
                                Valores = stream.ReadLine().Split('\t');
                            }
                            foreach (string campo in Columnas)
                            {
                                dato.Add(campo, Convert.ToDouble(Valores[i]));
                                i++;
                            }
                        }
                        return dato;
                    case 't':

                        var Database_TeBoys = GetResourceStream("TE_boys_z_WHO2007_exp.txt");
                        using (StreamReader stream = new StreamReader(Database_TeBoys))
                        {
                            var Columnas = stream.ReadLine().Split('\t');
                            var Valores = stream.ReadLine().Split('\t');
                            while (Convert.ToInt32(Valores[0]) != edad)
                            {
                                Valores = stream.ReadLine().Split('\t');
                            }
                            foreach (string campo in Columnas)
                            {
                                dato.Add(campo, Convert.ToDouble(Valores[i]));
                                i++;
                            }
                        }
                        return dato;
                    case 'm':

                        var Database_BmiBoys = GetResourceStream("bmi_boys_z_WHO2007_exp.txt");
                        using (StreamReader stream = new StreamReader(Database_BmiBoys))
                        {
                            var Columnas = stream.ReadLine().Split('\t');
                            var Valores = stream.ReadLine().Split('\t');
                            while (Convert.ToInt32(Valores[0]) != edad)
                            {
                                Valores = stream.ReadLine().Split('\t');
                            }
                            foreach (string campo in Columnas)
                            {
                                dato.Add(campo, Convert.ToDouble(Valores[i]));
                                i++;
                            }
                        }
                        return dato;
                    default:
                        return dato;
                }

            }

        }
    }
}
