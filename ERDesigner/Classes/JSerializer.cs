using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ERDesigner.Shape;

namespace ERDesigner
{
    class JSerializer
    {
        XmlSerializer serializer;
        public void saveProjectToXML(String fpath, ProjectData obj)
        {
            serializer = new XmlSerializer(typeof(ProjectData));
            FileStream fs = new FileStream(fpath, FileMode.Create);
            serializer.Serialize(fs, obj);
            fs.Close();
        }

        public ProjectData loadProjectFromXML(String fpath)
        {
            serializer = new XmlSerializer(typeof(ProjectData));
            FileStream fs = new FileStream(fpath, FileMode.Open);
            ProjectData obj = (ProjectData)serializer.Deserialize(fs);
            fs.Close();

            return obj;
        }
        public void saveConceptualToXML(String fpath, MetaData obj)
        {
            serializer = new XmlSerializer(typeof(MetaData));
            FileStream fs = new FileStream(fpath, FileMode.Create);
            serializer.Serialize(fs, obj);
            fs.Close();
        }

        public MetaData loadConceptualFromXML(String fpath)
        {
            serializer = new XmlSerializer(typeof(MetaData));
            FileStream fs = new FileStream(fpath, FileMode.Open);
            MetaData obj = (MetaData)serializer.Deserialize(fs);
            fs.Close();

            return obj;
        }
        public void savePhysicalToXML(String fpath, MetaDataPhysical obj)
        {
            serializer = new XmlSerializer(typeof(MetaDataPhysical));
            FileStream fs = new FileStream(fpath, FileMode.Create);
            serializer.Serialize(fs, obj);
            fs.Close();
        }

        public MetaDataPhysical loadPhysicalFromXML(String fpath)
        {
            serializer = new XmlSerializer(typeof(MetaDataPhysical));
            FileStream fs = new FileStream(fpath, FileMode.Open);
            MetaDataPhysical obj = (MetaDataPhysical)serializer.Deserialize(fs);
            fs.Close();

            return obj;
        }
    }
}
