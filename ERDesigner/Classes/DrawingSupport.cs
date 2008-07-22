using System;
using System.Collections.Generic;
using System.Text;
using ERDesigner.Shape;
using System.Drawing;

namespace ERDesigner
{
    class DrawingSupport
    {
        public static int CalculateCardiDirection(ShapeBase shape1, ShapeBase shape2)
        {
            if (shape1 == null || shape2 == null)
                return 0;

            //Cạnh đặt Shape - > 1,2,3,4: Up, Right, Down, Left
            int edgeCardiPlace = -1;

            Point point1 = shape1.CenterPoint;
            Point point2 = shape2.CenterPoint;

            //Cách khác: phương trình qua 2 điểm centerRelationship và point1 (y = mx + b)
            double m = point2.Y - point1.Y;

            if (point2.X != point1.X)
                m /= point2.X - point1.X;
            else
                m = 0;

            double b = point2.Y - m * point2.X;


            if (point1.X < point2.X) //shape 2 nằm bên phải
            {
                //thế "x + width" của shape 1 vào phương trình để tính y
                //Nếu y vượt quá khoảng y của shape 2 thì đặt cardi ở up hoặc down: else đặt right
                double ytemp = m * (shape1.Location.X + shape1.Width) + b;

                if (ytemp < shape1.Location.Y) edgeCardiPlace = 1; //Up
                else if (ytemp > shape1.Location.Y + shape1.Height) edgeCardiPlace = 3; //Down

                if (ytemp >= shape1.Location.Y && ytemp <= shape1.Location.Y + shape1.Height) edgeCardiPlace = 2; //Right
            }
            else //shape 2 nằm bên trái hoặc nằm thẳng hàng
            {
                //thế "x" của shape 1 vào phương trình để tính y
                //Nếu y vượt quá khoảng y của shape 2 thì đặt cardi ở up hoặc down: else đặt left
                double ytemp = m * (shape1.Location.X) + b;

                if (ytemp < shape1.Location.Y) edgeCardiPlace = 1; //Up
                else if (ytemp > shape1.Location.Y + shape1.Height) edgeCardiPlace = 3; //Down

                if (ytemp >= shape1.Location.Y && ytemp <= shape1.Location.Y + shape1.Height) edgeCardiPlace = 4; //Left
            }
            return edgeCardiPlace;
        }
        public static void DrawCardinalitiesLine(Graphics g, CardinalityShape cardi, int edgeCardiPlace, int pos, int numCardi)
        {
            EntityShape entity = cardi.Entity;
            RelationshipShape rel = cardi.Relationship;

            //debug
            //g.DrawLine(new Pen(Color.Red, 1), entity.CenterPoint, rel.CenterPoint);

            Point centerRelationship = rel.CenterPoint;
            Point centerEntity = entity.CenterPoint;

            Point TopLeft = entity.Location;
            Point BottomRight = new Point(entity.Location.X + entity.Width, entity.Location.Y + entity.Height);

            int stepX = pos * entity.Width / (numCardi + 1);
            int stepY = pos * entity.Height / (numCardi + 1);

            if (rel.type != RelationshipType.AssociativeEntity)
            {
                Bitmap myCardi = GetCardinalitiesShape(cardi, edgeCardiPlace);
                switch (edgeCardiPlace)
                {
                    case 1: g.DrawImage(myCardi, TopLeft.X + stepX - myCardi.Width / 2, TopLeft.Y - myCardi.Height);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, TopLeft.X + stepX, TopLeft.Y - myCardi.Height);
                        break;
                    case 2: g.DrawImage(myCardi, BottomRight.X, BottomRight.Y - (entity.Height - stepY) - myCardi.Height / 2);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, BottomRight.X + myCardi.Width, BottomRight.Y - (entity.Height - stepY));
                        break;
                    case 3: g.DrawImage(myCardi, BottomRight.X - (entity.Width - stepX) - myCardi.Width / 2 + 1, BottomRight.Y);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, BottomRight.X - (entity.Width - stepX), BottomRight.Y + myCardi.Height);
                        break;
                    case 4: g.DrawImage(myCardi, TopLeft.X - myCardi.Width, TopLeft.Y + stepY - myCardi.Height / 2 + 1);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, TopLeft.X - myCardi.Width, TopLeft.Y + stepY);
                        break;
                }
            }
            else //nếu relationship là Associative Entity
            {
                //Point TopLeftRel = rel.Location;
                //Point BottomRightRel = new Point(rel.Location.X + rel.Width, rel.Location.Y + rel.Height);

                //int stepXRel = (index + 1) * rel.Width / (rel.cardiplaces[newEdgeCardiPlace - 1].Count + 1);
                //int stepYRel = (index + 1) * rel.Height / (rel.cardiplaces[newEdgeCardiPlace - 1].Count + 1);

                ////cardi 1, 1
                //CardinalityShape cardi1 = new CardinalityShape();
                //cardi1.setValue(1, 1);

                ////lấy cardi của đầu bên kia Relationship
                //CardinalityShape cardi2 = new CardinalityShape();
                //foreach (CardinalityShape card in cardi.relationship.cardinalities)
                //{
                //    if (card != cardi)
                //        cardi2 = card;
                //}

                //Bitmap CardiAtEn = GetCardinalitiesShape(cardi1, edgeCardiPlace);
                //Bitmap CardiAtRel = GetCardinalitiesShape(cardi2, newEdgeCardiPlace);

                ////Vẽ cardi cho En
                //switch (edgeCardiPlace)
                //{
                //    case 1: g.DrawImage(CardiAtEn, TopLeft.X + stepX - CardiAtEn.Width / 2, TopLeft.Y - CardiAtEn.Height);
                //        break;
                //    case 2: g.DrawImage(CardiAtEn, BottomRight.X, BottomRight.Y - (entity.Height - stepY) - CardiAtEn.Height / 2);
                //        break;
                //    case 3: g.DrawImage(CardiAtEn, BottomRight.X - (entity.Width - stepX) - CardiAtEn.Width / 2 + 1, BottomRight.Y);
                //        break;
                //    case 4: g.DrawImage(CardiAtEn, TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY - CardiAtEn.Height / 2 + 1);
                //        break;
                //}

                ////Vẽ cardi cho Ass En
                //switch (newEdgeCardiPlace)
                //{
                //    case 1: g.DrawImage(CardiAtRel, TopLeftRel.X + stepXRel - CardiAtRel.Width / 2, TopLeftRel.Y - CardiAtRel.Height);
                //        break;
                //    case 2: g.DrawImage(CardiAtRel, BottomRightRel.X, BottomRightRel.Y - (rel.Height - stepYRel) - CardiAtRel.Height / 2);
                //        break;
                //    case 3: g.DrawImage(CardiAtRel, BottomRightRel.X - (rel.Width - stepXRel) - CardiAtRel.Width / 2 + 1, BottomRightRel.Y);
                //        break;
                //    case 4: g.DrawImage(CardiAtRel, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel - CardiAtRel.Height / 2 + 1);
                //        break;
                //}

                ////Vẽ Line
                //switch (edgeCardiPlace)
                //{
                //    case 1:
                //        switch (newEdgeCardiPlace)
                //        {
                //            case 2: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X + CardiAtRel.Width, BottomRightRel.Y - (rel.Height - stepYRel), TopLeft.X + stepX, TopLeft.Y - CardiAtEn.Height);
                //                break;
                //            case 3: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X - (rel.Width - stepXRel), BottomRightRel.Y + CardiAtRel.Height, TopLeft.X + stepX, TopLeft.Y - CardiAtRel.Height);
                //                break;
                //            case 4: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel, TopLeft.X + stepX, TopLeft.Y - CardiAtRel.Height);
                //                break;
                //        }
                //        break;
                //    case 2:
                //        switch (newEdgeCardiPlace)
                //        {
                //            case 1: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X + stepXRel, TopLeftRel.Y - CardiAtRel.Height, BottomRight.X + CardiAtRel.Width, BottomRight.Y - (entity.Height - stepY));
                //                break;
                //            case 3: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X - (rel.Width - stepXRel), BottomRightRel.Y + CardiAtRel.Height, BottomRight.X + CardiAtRel.Width, BottomRight.Y - (entity.Height - stepY));
                //                break;
                //            case 4: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel, BottomRight.X + CardiAtRel.Width, BottomRight.Y - (entity.Height - stepY));
                //                break;
                //        }
                //        break;
                //    case 3:
                //        switch (newEdgeCardiPlace)
                //        {
                //            case 1: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X + stepXRel, TopLeftRel.Y - CardiAtRel.Height, BottomRight.X - (entity.Width - stepX), BottomRight.Y + CardiAtRel.Height);
                //                break;
                //            case 2: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X + CardiAtRel.Width, BottomRightRel.Y - (rel.Height - stepYRel), BottomRight.X - (entity.Width - stepX), BottomRight.Y + CardiAtEn.Height);
                //                break;
                //            case 4: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel, BottomRight.X - (entity.Width - stepX), BottomRight.Y + CardiAtEn.Height);
                //                break;
                //        }
                //        break;
                //    case 4:
                //        switch (newEdgeCardiPlace)
                //        {
                //            case 1: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X + stepXRel, TopLeftRel.Y - CardiAtRel.Height, TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY);
                //                break;
                //            case 2: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X + CardiAtRel.Width, BottomRightRel.Y - (rel.Height - stepYRel), TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY);
                //                break;
                //            case 3: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X - (rel.Width - stepXRel), BottomRightRel.Y + CardiAtRel.Height, TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY);
                //                break;
                //        }
                //        break;
                //}

            }
        }
        private static Bitmap GetCardinalitiesShape(CardinalityShape cardi, int edgeCardiPlace)
        {
            Bitmap carshape = new Bitmap(20, 20);
            Graphics gimage = Graphics.FromImage(carshape);

            if (cardi.MaxCardinality == -1)
            {
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 8), new Point(3, 20));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 8), new Point(17, 20));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 0), new Point(10, 20));

                if (cardi.MinCardinality == 0)
                {
                    //gimage.FillEllipse(new SolidBrush(this.BackColor), 7, 0, 6, 6);
                    gimage.DrawEllipse(ThongSo.ConectiveLinePen, 7, 0, 6, 6);
                }
                else
                    gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 5), new Point(17, 5));
            }
            else
            {
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 13), new Point(17, 13));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 0), new Point(10, 20));

                if (cardi.MinCardinality == 0)
                {
                    //gimage.FillEllipse(new SolidBrush(this.BackColor), 7, 3, 6, 6);
                    gimage.DrawEllipse(ThongSo.ConectiveLinePen, 7, 3, 6, 6);
                }
                else
                    gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 8), new Point(17, 8));
            }
            switch (edgeCardiPlace)
            {
                case 1:
                    break;
                case 2: carshape.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 3: carshape.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4: carshape.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return carshape;
        }
    }
}
