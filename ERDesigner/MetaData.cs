using System;
using System.Collections.Generic;
using System.Text;
using ERDesigner.Shape;
using System.Drawing;

namespace ERDesigner
{
    public class EntityData : IMetaData
    {
        public String name;
        public String type;
        public int x, y;
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
        #region IMetaData Members

        public INotation createNotation()
        {
            EntityShape entity = new EntityShape();
            entity.sName = this.name;
            entity.type = this.type;
            entity.Location = new Point(this.x, this.y);
            entity.Size = new Size(this.w, this.h);

            return (INotation)entity;
        }

        #endregion
    }
    public class SubTypeConnectorData : IMetaData
    {
        public int x, y;
        public int w, h;
        public string Completeness;
        public string Disjointness;
        public string SuperType;
        public string AttributeDiscriminator;
        public List<string> SubTypes;
        public List<string> Discriminators;

        public SubTypeConnectorData()
        {
            SubTypes = new List<string>();
            Discriminators = new List<string>();
        }
        public SubTypeConnectorData(string completeness, string disjointness, int xx, int yy, int ww, int hh)
        {
            SubTypes = new List<string>();
            Discriminators = new List<string>();
            
            x = xx;
            y = yy;
            w = ww;
            h = hh;

            Completeness = completeness;
            Disjointness = disjointness;
        }

        #region IMetaData Members

        public INotation createNotation()
        {
            SubTypeConnector subtypeconnector = new SubTypeConnector(new Point(this.x, this.y), Completeness, Disjointness);
            
            subtypeconnector.Location = new Point(this.x, this.y);

            foreach (string des in this.Discriminators)
            {
                subtypeconnector.discriminators.Add(des);
            }

            return (INotation)subtypeconnector;
        }

        #endregion
    }
    public class AttributeData : IMetaData
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

        #region IMetaData Members

        public INotation createNotation()
        {
            AttributeShape attribute = new AttributeShape();
            attribute.sName = this.name;
            attribute.type = this.type;
            attribute.Location = new Point(this.x, this.y);
            attribute.Size = new Size(this.w, this.h);
            attribute.dataType = this.DataType;
            attribute.dataLength = this.Length;
            attribute.allowNull = this.AllowNull;
            attribute.description = this.Description;

            return (INotation)attribute;
        }

        #endregion
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
    public class RelationshipData : IMetaData
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

        #region IMetaData Members

        public INotation createNotation()
        {
            RelationshipShape relationship = new RelationshipShape();
            relationship.sName = this.name;
            relationship.type = this.type;
            relationship.Location = new Point(this.x, this.y);
            relationship.Size = new Size(this.w, this.h);
                
            return (INotation)relationship;
        }

        #endregion
    }
    public class MetaData
    {
        public string Name;
        public List<EntityData> Entities;
        public List<RelationshipData> Relationships;
        public List<SubTypeConnectorData> SubTypeConnectors;
        public MetaData()
        {
            Entities = new List<EntityData>();
            Relationships = new List<RelationshipData>();
            SubTypeConnectors = new List<SubTypeConnectorData>();
        }
    }
}
