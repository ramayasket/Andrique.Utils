using Newtonsoft.Json;
using Opn.Services.Dotnet.Credits.Rtdm.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Opn.Services.Dotnet.Credits.Rtdm.Tests
{
    internal static class RtdmData
    {
        private const string BASE = "Opn.Services.Dotnet.Credits.Rtdm.Tests.";

        /// <summary>
        /// Массив входных данных для RTDM.
        /// </summary>
        public static RtdmInput[] Inputs =>
            GetStrings("RtdmInputs.json").Select(JsonConvert.DeserializeObject<RtdmInput>).ToArray();

        /// <summary>
        /// Выходные данные RTDM в формате XML.
        /// </summary>
        public static string MessageContent => GetString("RtdmOutput.xml");

        /// <summary>
        /// Читает строку из ресурсов сборки.
        /// </summary>
        /// <param name="name">Имя ресурса.</param>
        /// <returns>Строковые данные.</returns>
        private static string GetString(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(BASE + name);
            using var reader = new StreamReader(stream!);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Читает строки из ресурсов сборки.
        /// </summary>
        /// <param name="name">Имя ресурса.</param>
        /// <returns>Строковые данные.</returns>
        private static string[] GetStrings(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list = new List<string>();

            //// ReSharper disable AssignNullToNotNullAttribute
            using (var stream = assembly.GetManifestResourceStream(BASE + name))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    list.Add(line);
            }

            return list.ToArray();
        }
    }
}
