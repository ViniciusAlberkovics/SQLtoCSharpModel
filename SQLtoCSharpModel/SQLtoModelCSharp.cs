using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SQLtoCSharpModel
{
    public class SQLtoModelCSharp
    {
        public SQLtoModelCSharp()
        {
            Console.WriteLine("Iniciando exportação");
            Start();
            Console.WriteLine("Fim da exportação");
        }

        public void Start()
        {
            string path = @"C:\projects\Alpha_Project-DB\Alpha_Project.sql";
            string pathDestination = null;
            string[] nomeClass = null;

            using (var streamReader = new StreamReader(path))
            {
                var x = streamReader.ReadLine();
                while (true)
                {
                    if (x != null && x.ToLower().Contains("create table"))
                    {
                        nomeClass = x.Split(" ");
                        Regex reg = new Regex(@"\W");
                        nomeClass = reg.Split(nomeClass[2]);

                        byte pos = 0;
                        if (nomeClass.Length >= 2)
                        {
                            pos = 1;
                        }

                        Console.WriteLine($"Exportando tabela {nomeClass[pos]}");
                        pathDestination = @"C:\projects\Alpha_Project-WS\AlphaWs.Models\" + nomeClass[pos].ToLower() + "Model.cs";

                        using (var streamWriter = new StreamWriter(pathDestination, true))
                        {
                            if (nomeClass.Length >= 1)
                                streamWriter.WriteLine("public class " + nomeClass[pos].ToLower() + "\n{");

                            while (!x.Contains(");"))
                            {
                                x = streamReader.ReadLine();

                                string line = IsVariable(x, nomeClass);
                                if (!string.IsNullOrEmpty(line))
                                    streamWriter.WriteLine(line);
                            }
                            streamWriter.WriteLine("}");
                            Console.WriteLine("Concluido!!");
                        }
                    }

                    x = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(x))
                    {
                        x = streamReader.ReadLine();
                        if (string.IsNullOrEmpty(x))
                            break;
                    }
                }
            }
        }

        private string IsVariable(string line, string[] nomeClass)
        {
            try
            {
                string[] naoPermitido = { "constraint", "references", "create index", "insert ", "values", "('" };
                bool lineBool = true;

                var properties = line.Split(" ");

                if (properties[0].ToLower().Contains(naoPermitido[0]))
                {
                    lineBool = false;
                }
                else if (properties[0].ToLower().Contains(naoPermitido[1]))
                {
                    lineBool = false;
                }
                else if (properties[0].ToLower().Contains(naoPermitido[2]))
                {
                    lineBool = false;
                }
                else if (properties[0].ToLower().Contains(naoPermitido[3]))
                {
                    lineBool = false;
                }
                else if (properties[0].ToLower().Contains(naoPermitido[4]))
                {
                    lineBool = false;
                }
                else if (properties[0].ToLower().Contains(naoPermitido[5]))
                {
                    lineBool = false;
                }

                if (lineBool)
                    return ($"public {TypeVariable(properties[1], nomeClass)} {properties[0]} " + "{ get; set; }");
            }
            catch (Exception) { }

            return string.Empty;
        }

        private string TypeVariable(string tipoSQL, string[] nomeClass)
        {
            string[] tipos = { "VARCHAR", "INT", "DATETIME", "BIT", "FLOAT", "DATE", "TINYINT" };
            string[] tiposRetorno = { "string", "int", "DateTime", "bool", "float", "DateTime", "byte" };

            for (int i = 0; i < tipos.Length; i++)
            {
                if (tipoSQL.Contains(tipos[i]))
                    return tiposRetorno[i];
            }

            return "####OLHA NO ARQUIVO SQL####" + nomeClass;
        }
    }
}


