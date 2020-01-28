using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace GetImagesFromSimulation
{
    class Program
    {

        static void Main(string[] args)
        {

            var path = "";

            // LoadConfiguration
            {

                string DLLFullName = Assembly.GetExecutingAssembly().CodeBase;
                string DLLFullPath = System.IO.Path.GetDirectoryName(DLLFullName);
                string filePath = DLLFullPath + "\\" + "config.xml";

                var config = XDocument.Load(filePath);
                
                var cfg = config.Element(@"Configuration");
                var simulationDirectory = cfg.Element(@"SimulationDirectory");
                path = simulationDirectory.Attribute(@"Path").Value;

            }

            var engineFile = "";
            var quizblockFile = "";

            {
                DirectoryInfo directoryEngine = new DirectoryInfo(path);
                FileInfo[] filesEngine = directoryEngine.GetFiles("*.xml");

                DirectoryInfo directoryQuizzblock = new DirectoryInfo(path + @"QuizBlocks\");
                FileInfo[] filesQuizblock = directoryQuizzblock.GetFiles("*.xml");

                foreach (var e in filesEngine)
                {
                    if (e.Name.Contains(@"Engine_") == true)
                    {
                        engineFile = path + e.Name;
                    }
                }

                foreach (var q in filesQuizblock)
                {
                    if (q.Name.Contains(@"QuizBlocks_") == true)
                    {
                        quizblockFile = path + @"QuizBlocks\" + q.Name;
                    }
                }

            }

            var engine = XDocument.Load(engineFile);
            var quizzblock = XDocument.Load(quizblockFile);

            var allImages = new List<string>();
            allImages.AddRange(GetALLImagesInSImulation(engine));
            allImages.AddRange(GetALLImagesInQuizblock(quizzblock));

            // Write the string to a file.
            {
                
                string DLLFullName = Assembly.GetExecutingAssembly().Location;
                string DLLFullPath = System.IO.Path.GetDirectoryName(DLLFullName);
                var saveFile = DLLFullPath + @"\result.txt";
                System.IO.StreamWriter file = new System.IO.StreamWriter(saveFile);
                foreach (var img in allImages)
                {
                    file.WriteLine(img);
                }
                file.Close();
            }

        }

        private static List<string> GetALLImagesInSImulation(XDocument doc)
        {
            var result = new List<string>();
            foreach (var element in doc.Descendants())
            {
                foreach (var attribute in element.Attributes())
                {
                    var value = attribute.Value;
                    if (value.Contains(@".png") == true || value.Contains(@".jpg") == true)
                    {
                        if (value.Contains(@"src=") == true)
                        {
                            var words = value.Split(' ');
                            foreach (var word in words)
                            {
                                if (word.Contains(@"src=") == true)
                                {
                                    var tmp = word.Split('"');
                                    result.Add(tmp[1]);
                                }
                            }
                        }
                        else
                        {
                            result.Add(value);
                        }
                    }
                }
            }
            return result;
        }

        private static List<string> GetALLImagesInQuizblock(XDocument doc)
        {
            var result = new List<string>();
            foreach (var element in doc.Descendants())
            {
                foreach (var attribute in element.Attributes())
                {
                    var value = attribute.Value;
                    if (value.Contains(@".png") == true || value.Contains(@".jpg") == true)
                    {
                        var words = value.Split(' ');
                        foreach (var word in words)
                        {
                            if (word.Contains(@"src=") == true)
                            {
                                var tmp = word.Split('"');
                                result.Add(tmp[1]);
                            }
                        }
                    }
                }
            }
            return result;
        }

    }
}
