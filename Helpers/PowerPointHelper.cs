/****************************** Module Header ******************************\
* Module Name:  CSOpenXmlInsertImageToPPT.cs
* Project:      CSOpenXmlInsertImageToPPT
* Copyright(c)  Microsoft Corporation.
* 
* The Class is used to Insert Image into PowerPoint using Open XML SDK.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Packaging;
using Drawing = DocumentFormat.OpenXml.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;

namespace WebApplication1.Helpers
{
    public class PowerPointHelper
    {
        // Insert a slide into the specified presentation.
        public static void InsertNewSlide(string presentationFile, int position, string slideTitle)
        {
            // Open the source document as read/write. 
            using (PresentationDocument presentationDocument = PresentationDocument.Open(presentationFile, true))
            {
                // Pass the source document and the position and title of the slide to be inserted to the next method.
                //InsertNewSlide(presentationDocument, position, slideTitle);
            }
        }

        // Insert the specified slide into the presentation at the specified position.
        public static void InsertNewSlide(PresentationDocument presentationDocument, int position, string slideTitle, string content, string imageName, string serverPath)
        {

            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            if (slideTitle == null)
            {
                throw new ArgumentNullException("slideTitle");
            }

            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Verify that the presentation is not empty.
            if (presentationPart == null)
            {
                throw new InvalidOperationException("The presentation document is empty.");
            }

            // Declare and instantiate a new slide.
            Slide slide = new Slide(new CommonSlideData(new ShapeTree()));
            uint drawingObjectId = 1;

            // Construct the slide content.            
            // Specify the non-visual properties of the new slide.
            NonVisualGroupShapeProperties nonVisualProperties = slide.CommonSlideData.ShapeTree.AppendChild(new NonVisualGroupShapeProperties());
            nonVisualProperties.NonVisualDrawingProperties = new NonVisualDrawingProperties() { Id = 1, Name = "" };
            nonVisualProperties.NonVisualGroupShapeDrawingProperties = new NonVisualGroupShapeDrawingProperties();
            nonVisualProperties.ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties();

            // Specify the group shape properties of the new slide.
            slide.CommonSlideData.ShapeTree.AppendChild(new GroupShapeProperties());

            if (!string.IsNullOrEmpty(slideTitle))
            {
                // Declare and instantiate the title shape of the new slide.
                Shape titleShape = slide.CommonSlideData.ShapeTree.AppendChild(new Shape());

                drawingObjectId++;

                //Specify the required shape properties for the title shape.
                titleShape.NonVisualShapeProperties = new NonVisualShapeProperties
                    (new NonVisualDrawingProperties() { Id = drawingObjectId, Name = "Title" },
                    new NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                    new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Title }));
                titleShape.ShapeProperties = new ShapeProperties();

                Drawing.Transform2D transform2D = new Drawing.Transform2D();
                //position
                Drawing.Offset offset = new Drawing.Offset() { X = 159657, Y = 2692399 };
                //size
                Drawing.Extents extents = new Drawing.Extents() { Cx = 11872686, Cy = 1505311 };
                transform2D.Append(offset);
                transform2D.Append(extents);

                titleShape.ShapeProperties.Transform2D = transform2D;

                // Specify the text of the title shape.
                titleShape.TextBody = new TextBody(new Drawing.BodyProperties(),
                        new Drawing.ListStyle(),
                        new Drawing.Paragraph(new Drawing.ParagraphProperties() { Alignment = Drawing.TextAlignmentTypeValues.Center }, new Drawing.Run(new Drawing.Text() { Text = slideTitle })));
            }

            if (!string.IsNullOrEmpty(content))
            {
                long cx;
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(HostingEnvironment.MapPath(imageName)))
                {
                    cx = (long)img.Width * (long)((float)914400 / img.HorizontalResolution);
                }

                // Declare and instantiate the body shape of the new slide.
                Shape bodyShape = slide.CommonSlideData.ShapeTree.AppendChild(new Shape());

                drawingObjectId++;

                //Specify the required shape properties for the body shape.
                bodyShape.NonVisualShapeProperties = new NonVisualShapeProperties(new NonVisualDrawingProperties() { Id = drawingObjectId, Name = "Content Placeholder" },
                        new NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                        new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Index = 1 }));
                bodyShape.ShapeProperties = new ShapeProperties();

                Drawing.Transform2D transform2D = new Drawing.Transform2D();
                //position
                Drawing.Offset offset = new Drawing.Offset() { X = 1418626 + cx, Y = 1188720 };
                //size
                Drawing.Extents extents = new Drawing.Extents() { Cx = 6665831, Cy = 3270248 };
                transform2D.Append(offset);
                transform2D.Append(extents);

                bodyShape.ShapeProperties.Transform2D = transform2D;

                //Specify the text of the body shape.
                bodyShape.TextBody = new TextBody(new Drawing.BodyProperties(),
                       new Drawing.ListStyle(),
                       new Drawing.Paragraph(new Drawing.Run(new Drawing.Text() { Text = content })));
            }

            // Create the slide part for the new slide.
            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();

            // Save the new slide part.
            slide.Save(slidePart);

            if (!string.IsNullOrEmpty(imageName))
            {
                //InsertImageInLastSlide(slidePart.Slide, Path.Combine(serverPath, imageName), "image/png");
                InsertImageInLastSlide(slidePart.Slide, imageName, "image/png");
            }

            // Modify the slide ID list in the presentation part.
            // The slide ID list should not be null.
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

            // Find the highest slide ID in the current list.
            uint maxSlideId = 1;
            SlideId prevSlideId = null;

            foreach (SlideId slideId in slideIdList.ChildElements)
            {
                if (slideId.Id > maxSlideId)
                {
                    maxSlideId = slideId.Id;
                }

                position--;
                if (position == 0)
                {
                    prevSlideId = slideId;
                }

            }

            maxSlideId++;

            // Get the ID of the previous slide.
            SlidePart lastSlidePart;

            if (prevSlideId != null)
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(prevSlideId.RelationshipId);
            }
            else
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(((SlideId)(slideIdList.ChildElements[0])).RelationshipId);
            }

            // Use the same slide layout as that of the previous slide.
            if (null != lastSlidePart.SlideLayoutPart)
            {
                slidePart.AddPart(lastSlidePart.SlideLayoutPart);
            }

            // Insert the new slide into the slide list after the previous slide.
            SlideId newSlideId = slideIdList.InsertAfter(new SlideId(), prevSlideId);
            newSlideId.Id = maxSlideId;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(slidePart);

            // Save the modified presentation.
            presentationPart.Presentation.Save();
        }

        public static void CloneLastSlide(PresentationDocument presentationDocument)
        {
            var presentationPart = presentationDocument.PresentationPart;
            var templatePart = presentationPart.GetSlidePartsInOrder().Last();

            var newSlidePart = templatePart.CloneSlide();

            ReplaceImage(newSlidePart);

            presentationPart.AppendSlide(newSlidePart);

            presentationPart.Presentation.Save();
        }

        public static void ReplaceImage(SlidePart slidePart)
        {
            var pics = slidePart.Slide.CommonSlideData.ShapeTree.Elements<Picture>();

            ImagePart imagePart = slidePart.AddNewPart<ImagePart>("image/png", pics.First().BlipFill.Blip.Embed);
            FileStream fileStream = new FileStream(@"C:\Users\sergiou\Desktop\New folder\description\image2.png", FileMode.Open);
            imagePart.FeedData(fileStream);
            fileStream.Close();
        }

        public static void InsertImageInLastSlide(Slide slide, string imagePath, string imageExt)
        {
            // Creates a Picture instance and adds its children.
            Picture picture = new Picture();
            string embedId = string.Empty;
            embedId = "rId" + (slide.Elements<Picture>().Count() + 915).ToString();
            NonVisualPictureProperties nonVisualPictureProperties = new NonVisualPictureProperties(
                new NonVisualDrawingProperties() { Id = (DocumentFormat.OpenXml.UInt32Value)4U, Name = "Picture 5" },
                new NonVisualPictureDrawingProperties(new Drawing.PictureLocks() { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties());

            BlipFill blipFill = new BlipFill();
            Drawing.Blip blip = new Drawing.Blip() { Embed = embedId };

            // Creates a BlipExtensionList instance and adds its children
            Drawing.BlipExtensionList blipExtensionList = new Drawing.BlipExtensionList();
            Drawing.BlipExtension blipExtension = new Drawing.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

            DocumentFormat.OpenXml.Office2010.Drawing.UseLocalDpi useLocalDpi = new DocumentFormat.OpenXml.Office2010.Drawing.UseLocalDpi() { Val = false };
            useLocalDpi.AddNamespaceDeclaration("a14","http://schemas.microsoft.com/office/drawing/2010/main");

            blipExtension.Append(useLocalDpi);
            blipExtensionList.Append(blipExtension);
            blip.Append(blipExtensionList);

            Drawing.Stretch stretch = new Drawing.Stretch();
            Drawing.FillRectangle fillRectangle = new Drawing.FillRectangle();
            stretch.Append(fillRectangle);

            blipFill.Append(blip);
            blipFill.Append(stretch);

            // Creates a ShapeProperties instance and adds its children.
            ShapeProperties shapeProperties = new ShapeProperties();
            
            //http://en.wikipedia.org/wiki/English_Metric_Unit#DrawingML
            //http://stackoverflow.com/questions/1341930/pixel-to-centimeter
            //http://stackoverflow.com/questions/139655/how-to-convert-pixels-to-points-px-to-pt-in-net-c
            long cx, cy;
            long maxCx = 12192000;
            long maxCy = 6858000;
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(HostingEnvironment.MapPath(imagePath)))
            {
                cx = (long)img.Width * (long)((float)914400 / img.HorizontalResolution);
                cy = (long)img.Height * (long)((float)914400 / img.VerticalResolution);

                if (cx > maxCx)
                {
                    double reductionPercent = ((double)maxCx) / ((double)cx);
                    cx = (long)(cx * reductionPercent);
                    cy = (long)(cy * reductionPercent);
                }

                if (cy > maxCy)
                {
                    double reductionPercent = ((double)maxCy) / ((double)cy);
                    cx = (long)(cx * reductionPercent);
                    cy = (long)(cy * reductionPercent);
                }
            }

            Drawing.Transform2D transform2D = new Drawing.Transform2D();
            
            long x = 1418626;
            long y = 1188720;

            if (x + cx > maxCx)
            {
                x = 0;
            }

            if (y + cy > maxCy)
            {
                y = 0;
            }

            //Position
            Drawing.Offset offset = new Drawing.Offset() { X = x, Y = y };
            //Size
            Drawing.Extents extents = new Drawing.Extents() { Cx = cx, Cy = cy };

            transform2D.Append(offset);
            transform2D.Append(extents);

            Drawing.PresetGeometry presetGeometry = new Drawing.PresetGeometry() { Preset = Drawing.ShapeTypeValues.Rectangle };
            Drawing.AdjustValueList adjustValueList = new Drawing.AdjustValueList();

            presetGeometry.Append(adjustValueList);

            shapeProperties.Append(transform2D);
            shapeProperties.Append(presetGeometry);

            picture.Append(nonVisualPictureProperties);
            picture.Append(blipFill);
            picture.Append(shapeProperties);

            slide.CommonSlideData.ShapeTree.AppendChild(picture);

            // Generates content of imagePart.
            ImagePart imagePart = slide.SlidePart.AddNewPart<ImagePart>(imageExt, embedId);

            WebRequest req = HttpWebRequest.Create(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + imagePath);
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                imagePart.FeedData(stream);
            }
            
        }

        // Delete the specified slide from the presentation.
        public static void DeleteSlide(PresentationDocument presentationDocument, int slideIndex)
        {
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            // Use the CountSlides sample to get the number of slides in the presentation.
            int slidesCount = CountSlides(presentationDocument);

            if (slideIndex < 0 || slideIndex >= slidesCount)
            {
                throw new ArgumentOutOfRangeException("slideIndex");
            }

            // Get the presentation part from the presentation document. 
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Get the presentation from the presentation part.
            Presentation presentation = presentationPart.Presentation;

            // Get the list of slide IDs in the presentation.
            SlideIdList slideIdList = presentation.SlideIdList;

            // Get the slide ID of the specified slide
            SlideId slideId = slideIdList.ChildElements[slideIndex] as SlideId;

            // Get the relationship ID of the slide.
            string slideRelId = slideId.RelationshipId;

            // Remove the slide from the slide list.
            slideIdList.RemoveChild(slideId);

            //
            // Remove references to the slide from all custom shows.
            if (presentation.CustomShowList != null)
            {
                // Iterate through the list of custom shows.
                foreach (var customShow in presentation.CustomShowList.Elements<CustomShow>())
                {
                    if (customShow.SlideList != null)
                    {
                        // Declare a link list of slide list entries.
                        LinkedList<SlideListEntry> slideListEntries = new LinkedList<SlideListEntry>();
                        foreach (SlideListEntry slideListEntry in customShow.SlideList.Elements())
                        {
                            // Find the slide reference to remove from the custom show.
                            if (slideListEntry.Id != null && slideListEntry.Id == slideRelId)
                            {
                                slideListEntries.AddLast(slideListEntry);
                            }
                        }

                        // Remove all references to the slide from the custom show.
                        foreach (SlideListEntry slideListEntry in slideListEntries)
                        {
                            customShow.SlideList.RemoveChild(slideListEntry);
                        }
                    }
                }
            }

            // Save the modified presentation.
            presentation.Save();

            // Get the slide part for the specified slide.
            SlidePart slidePart = presentationPart.GetPartById(slideRelId) as SlidePart;

            // Remove the slide part.
            presentationPart.DeletePart(slidePart);
        }

        // Count the slides in the presentation.
        public static int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }

            // Return the slide count to the previous method.
            return slidesCount;
        }
    }

        public static class OpenXmlUtils
    {
        public static IEnumerable<SlidePart> GetSlidePartsInOrder(this PresentationPart presentationPart)
        {
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

            return slideIdList.ChildElements
                .Cast<SlideId>()
                .Select(x => presentationPart.GetPartById(x.RelationshipId))
                .Cast<SlidePart>();
        }

        public static SlidePart CloneSlide(this SlidePart templatePart)
        {
            // find the presentationPart: makes the API more fluent
            var presentationPart = templatePart.GetParentParts()
                .OfType<PresentationPart>()
                .Single();

            // clone slide contents
            Slide currentSlide = (Slide)templatePart.Slide.CloneNode(true);
            var slidePartClone = presentationPart.AddNewPart<SlidePart>();
            currentSlide.Save(slidePartClone);

            // copy layout part
            slidePartClone.AddPart(templatePart.SlideLayoutPart);

            return slidePartClone;
        }

        public static void AppendSlide(this PresentationPart presentationPart, SlidePart newSlidePart)
        {
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

            // find the highest id
            uint maxSlideId = slideIdList.ChildElements
                .Cast<SlideId>()
                .Max(x => x.Id.Value);

            // Insert the new slide into the slide list after the previous slide.
            var id = maxSlideId + 1;

            SlideId newSlideId = new SlideId();
            slideIdList.Append(newSlideId);
            newSlideId.Id = id;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(newSlidePart);
        }
    }
}
