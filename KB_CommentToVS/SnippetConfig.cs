using System;
using System.IO;
using System.Xml.Linq;

namespace KB_CommentToVS
{
    public class SnippetConfig
    {
        public string DateFormat { get; set; } = "yyyy-MM-dd HH:mm";
        public string Modifier { get; set; } = "云看北";
        public string CommentPrefix { get; set; } = "///";

        /// <summary>
        /// 每行一条内容，不含前缀。用 \n 分隔。
        /// 可用 {Modifier} 和 {DateTime} 占位符。
        /// </summary>
        public string Template { get; set; } = "备注\n修改人:{Modifier}_修改日期:{DateTime}";

        private static string GetConfigPath()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "KB_CommentToVS");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "config.xml");
        }

        public void Save()
        {
            var doc = new XDocument(
                new XElement("SnippetConfig",
                    new XElement("DateFormat", DateFormat),
                    new XElement("Modifier", Modifier),
                    new XElement("CommentPrefix", CommentPrefix),
                    new XElement("Template", Template)
                )
            );
            doc.Save(GetConfigPath());
        }

        public static SnippetConfig Load()
        {
            string path = GetConfigPath();
            if (!File.Exists(path))
                return new SnippetConfig();

            try
            {
                var doc = XDocument.Load(path);
                var root = doc.Root;
                return new SnippetConfig
                {
                    DateFormat = root.Element("DateFormat")?.Value ?? "yyyy-MM-dd HH:mm:ss",
                    Modifier = root.Element("Modifier")?.Value ?? "姓名",
                    CommentPrefix = root.Element("CommentPrefix")?.Value ?? "///",
                    Template = root.Element("Template")?.Value ?? "备注\n修改人:{Modifier}_修改日期:{DateTime}"
                };
            }
            catch
            {
                return new SnippetConfig();
            }
        }
    }
}



