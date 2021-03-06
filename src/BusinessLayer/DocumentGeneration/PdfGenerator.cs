﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using iText;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace BusinessLayer.DocumentGeneration
{
    public static class PdfGenerator
    {
        public static void Generate(string file, Field field)
        {
            using PdfWriter writer = new PdfWriter(file);
            using PdfDocument pdf = new PdfDocument(writer);
            using Document doc = new Document(pdf);

            Table table = new Table(field.SideLength + 1);

            for (sbyte i = 0; i < field.SideLength; i++)
            {
                for (sbyte j = 0; j < field.SideLength; j++)
                {
                    Cell cell = new Cell();
                    cell.SetWidth(30);
                    cell.SetHeight(30);

                    Position pos = new Position(j, i);
                    BoatBit boatBit = field.Boats.SelectMany(b => b.BoatBits).SingleOrDefault(bb => bb.XYPosition == pos && bb.WasFound);
                    
                    if (boatBit != null)
                    {
                        cell.Add(GetImage(boatBit.GetBoatBitType()));
                    }
                    table.AddCell(cell);
                }

                //Number of Boats in the row.
                Cell boatcount = new Cell();
                boatcount.SetWidth(30);
                boatcount.Add(GetParagraph(field.GetNumOfBoatsInRow(i)));
                table.AddCell(boatcount);
            }

            //Number of boats in a column
            for (int i = 0; i < field.SideLength; i++)
            {
                Cell cell = new Cell();
                cell.Add(GetParagraph(field.GetNumOfBoatsInColumn(i)));
                table.AddCell(cell);
            }

            table.SetMarginBottom(30);

            doc.Add(table);

            RenderBoatList(doc, field.Boats);

            doc.Close();
            pdf.Close();
            writer.Close();
        }

        private static void RenderBoatList(Document doc, List<Boat> boats)
        {
            foreach(Boat boat in boats.OrderBy(b => b.BoatBits.Count))
            {
                Paragraph boatParagraph = new Paragraph();
                if(boat.BoatBits.Count == 1)
                {
                    boatParagraph.Add(GetImage(BoatTile.Single));
                }
                else
                {
                    boatParagraph.Add(GetImage(BoatTile.Left));
                    
                    for(int i = 0; i < boat.BoatBits.Count - 2; i++)
                    {
                        boatParagraph.Add(GetImage(BoatTile.Center));
                    }

                    boatParagraph.Add(GetImage(BoatTile.Right));
                }

                doc.Add(boatParagraph);
            }
        }

        private static Paragraph GetParagraph(object text)
        {
            Paragraph paragraph = new Paragraph(text.ToString());
            paragraph.SetTextAlignment(TextAlignment.CENTER);
            return paragraph;
        }

        private static Image GetImage(BoatTile tile)
        {
            Image img = new Image(ImageDataFactory.Create(GetImagePath(tile)));
            img.SetWidth(30);
            img.SetHeight(30);

            return img;
        }

        private static string GetImagePath(BoatTile tile)
        {
            string file = "";

            switch (tile)
            {
                case BoatTile.Center:
                    file = "Ship_mid.png";
                    break;
                case BoatTile.Top:
                    file = "Ship_top.png";
                    break;
                case BoatTile.Bottom:
                    file = "Ship_bottom.png";
                    break;
                case BoatTile.Right:
                    file = "Ship_right.png";
                    break;
                case BoatTile.Left:
                    file = "Ship_left.png";
                    break;
                case BoatTile.Single:
                    file = "Ship_one.png";
                    break;
            }

            return Path.Combine("Ressources", file);
        }
    }
}
