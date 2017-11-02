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

namespace WebApplication1.Helpers
{
    public class InsertSlide
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
        public static void InsertNewSlide(PresentationDocument presentationDocument, int position, string slideTitle, string content, bool addImage, string serverPath)
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

                // Specify the text of the title shape.
                titleShape.TextBody = new TextBody(new Drawing.BodyProperties(),
                        new Drawing.ListStyle(),
                        new Drawing.Paragraph(new Drawing.Run(new Drawing.Text() { Text = slideTitle })));
            }

            if (!string.IsNullOrEmpty(content))
            {
                // Declare and instantiate the body shape of the new slide.
                Shape bodyShape = slide.CommonSlideData.ShapeTree.AppendChild(new Shape());

                drawingObjectId++;

                //Specify the required shape properties for the body shape.
                bodyShape.NonVisualShapeProperties = new NonVisualShapeProperties(new NonVisualDrawingProperties() { Id = drawingObjectId, Name = "Content Placeholder" },
                        new NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                        new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Index = 1 }));
                bodyShape.ShapeProperties = new ShapeProperties();

                Drawing.Transform2D transform2D = new Drawing.Transform2D();
                Drawing.Offset offset = new Drawing.Offset() { X = 1453896, Y = 1188720 };
                Drawing.Extents extents = new Drawing.Extents() { Cx = 8732520, Cy = 4270248 };
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

            if (addImage)
                InsertImageInLastSlide(slidePart.Slide, Path.Combine(serverPath, "image.png") , "image/png");

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
            useLocalDpi.AddNamespaceDeclaration("a14",
                "http://schemas.microsoft.com/office/drawing/2010/main");

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

            Drawing.Transform2D transform2D = new Drawing.Transform2D();
            Drawing.Offset offset = new Drawing.Offset() { X = 838200, Y = 1877264 };
            Drawing.Extents extents = new Drawing.Extents() { Cx = 3284002, Cy = 3214689 };

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
            FileStream fileStream = new FileStream(imagePath, FileMode.Open);
            imagePart.FeedData(fileStream);
            fileStream.Close();
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
