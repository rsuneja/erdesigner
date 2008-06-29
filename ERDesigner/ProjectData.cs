using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    public class Model
    {
        public string ModelName;
        public string ModelType;
        private static int i = 1;
        public Model()
        {
            ModelName = "";
            ModelType = "";
        }
        public Model(string name, string type)
        {
            if (name == "") name = type + i;
            ModelName = name;
            i++;
            ModelType = type;
        }
    }
    public class ProjectData
    {
        public string ProjectName;
        public string ProjectPath;
        public List<Model> Models = new List<Model>();
        public List<int> OpeningModel = new List<int>();
        public int Focusing = 0;

        public ProjectData()
        {
            ProjectName = "";
            ProjectPath = "";
        }

        public ProjectData(string name, string path)
        {
            ProjectName = name;
            ProjectPath = path + "/" + name;
        }
    }
}
