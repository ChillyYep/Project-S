using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConfigDataExpoter
{
    /// <summary>
    /// 表数据导出工具
    /// </summary>
    public class DataExporter : FileExporter
    {
        public DataExporter(Dictionary<Type, List<object>> allTableDatas, FormatterType formatterType, MultiLanguageExchanger multiLanguageWriter)
        {
            m_allTableDatas = allTableDatas;
            binaryFormatter = new Formatter(FormatterType.Binary, multiLanguageWriter);
        }

        public void ExportData(string directory)
        {
            string defaultDirectory = directory;
            string jsonDirectory = directory + "/JsonData";
            try
            {
                if (Directory.Exists(defaultDirectory))
                {
                    Directory.Delete(defaultDirectory, true);
                }
                if (Directory.Exists(jsonDirectory))
                {
                    Directory.Delete(jsonDirectory, true);
                }
                Directory.CreateDirectory(defaultDirectory);
                Directory.CreateDirectory(jsonDirectory);
            }
            catch (Exception e)
            {
                throw new ParseExcelException(e.Message);
            }
            foreach (var item in m_allTableDatas)
            {
                // 二进制
                var str = SerializeDataTable(item.Value, item.Key);
                var filePath = Path.Combine(defaultDirectory, item.Key.Name + ".txt");
                ExportFile(filePath, str, false);
                // json文件
                var jsonStr = SerializeJsonDataTable(item.Value, item.Key);
                filePath = Path.Combine(jsonDirectory, item.Key.Name + ".json");
                ExportTextFile(filePath, jsonStr, false);
            }

        }
        public string SerializeJsonDataTable(List<object> dataTable, Type classType)
        {
            return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
        }

        public List<T> DeSerializeJsonDataTable<T>(Stream stream)
        {
            List<T> result;
            using (StreamReader sr = new StreamReader(stream))
            {
                var text = sr.ReadToEnd();
                result = JsonConvert.DeserializeObject<List<T>>(text);
            }
            return result;
        }
        public byte[] SerializeDataTable(List<object> dataTable, Type classType)
        {
            return binaryFormatter.SerializeDataTable(dataTable, classType) as byte[];
        }

        public List<T> DeSerializeDataTable<T>(Stream stream) where T : ConfigData.IBinaryDeserializer, new()
        {
            return binaryFormatter.DeSerializeDataTable<T>(stream);
        }

        public Dictionary<Type, List<object>> m_allTableDatas;

        private Formatter binaryFormatter;
    }
}
