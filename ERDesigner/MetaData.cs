using System;
using System.Collections.Generic;
using System.Text;


namespace ERDesigner
{
    public class EntityData
    {
        public String name;
        public String type;
        public int x,y;
        public int w, h;
        public List<AttributeData> Attributes;

        public EntityData() 
        {
            Attributes = new List<AttributeData>();
        }
        public EntityData(string n, string t, int xx, int yy, int ww, int hh)
        {
            Attributes = new List<AttributeData>();
            name = n;
            type = t;
            x = xx;
            y = yy;
            w = ww;
            h = hh;
        }
    }
    public class AttributeData
    {
        public String name;
        public bool isComposite;
        public String type;
        public int x, y;
        public int w, h;
        public List<AttributeData> AttributeChilds;
        public string DataType;
        public int Length;
        public bool AllowNull;
        public string Description;

        public AttributeData()
        {
            AttributeChilds = new List<AttributeData>();
        }
        public AttributeData(string n, string t, int xx, int yy, int ww, int hh, string dt, int l, bool allownull, string des)
        {
            AttributeChilds = new List<AttributeData>();
            name = n;
            type = t;
            x = xx;
            y = yy;
            w = ww;
            h = hh;
            DataType = dt;
            Length = l;
            AllowNull = allownull;
            isComposite = false;
            Description = des;
        }
    }
    public class CardinalityData
    {
        public String Entity;
        public int MinCardinality;
        public int MaxCardinality;
        public CardinalityData() { }
        public CardinalityData(string en, int min, int max)
        {
            Entity = en;
            MinCardinality = min;
            MaxCardinality = max;
        }
    }
    public class RelationshipData
    {
        public String name;
        public String type;
        public int x, y;
        public int w, h;
        public List<CardinalityData> Cardinalities;
        public List<AttributeData> Attributes;
        public RelationshipData()
        {
            Attributes = new List<AttributeData>();
            Cardinalities = new List<CardinalityData>();
        }
        public RelationshipData(string n, string t, int xx, int yy, int ww, int hh)
        {
            Attributes = new List<AttributeData>();
            Cardinalities = new List<CardinalityData>();
            name = n;
            type = t;
            x = xx;
            y = yy;
            w = ww;
            h = hh;
        }
    }
    public class MetaData
    {
        public string Name;
        public List<EntityData> Entities;
        public List<RelationshipData> Relationships;
        public MetaData() 
        {
            Entities = new List<EntityData>();
            Relationships = new List<RelationshipData>();
        }
    }
}
