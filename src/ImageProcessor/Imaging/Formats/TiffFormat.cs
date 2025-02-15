﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TiffFormat.cs" company="James Jackson-South">
//   Copyright (c) James Jackson-South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Provides the necessary information to support tiff images.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Imaging.Formats
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// Provides the necessary information to support tiff images.
    /// </summary>
    public class TiffFormat : FormatBase
    {
        private ImageCodecInfo imageCodecInfo;

        /// <summary>
        /// Gets the file headers.
        /// </summary>
        public override byte[][] FileHeaders => new[]
        {
            new byte[] { 73, 73, 42, 0 },
            new byte[] { 77, 77, 0, 42 }
        };

        /// <summary>
        /// Gets the list of file extensions.
        /// </summary>
        public override string[] FileExtensions => new[]
        {
            "tiff",
            "tif"
        };

        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public override string MimeType => "image/tiff";

        /// <summary>
        /// Gets the <see cref="ImageFormat" />.
        /// </summary>
        public override ImageFormat ImageFormat => ImageFormat.Tiff;

        /// <summary>
        /// Applies the given processor the current image.
        /// </summary>
        /// <param name="processor">The processor delegate.</param>
        /// <param name="factory">The <see cref="ImageFactory" />.</param>
        public override void ApplyProcessor(Func<ImageFactory, Image> processor, ImageFactory factory)
        {
            base.ApplyProcessor(processor, factory);

            // Set the property item information from any Exif metadata.
            // We do this here so that they can be changed between processor methods.
            if (factory.PreserveExifData)
            {
                foreach (KeyValuePair<int, PropertyItem> propertItem in factory.ExifPropertyItems)
                {
                    factory.Image.SetPropertyItem(propertItem.Value);
                }
            }
        }

        /// <summary>
        /// Saves the current image to the specified output stream.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="T:System.IO.Stream"/> to save the image information to.
        /// </param>
        /// <param name="image">
        /// The <see cref="T:System.Drawing.Image"/> to save.
        /// </param>
        /// <param name="bitDepth">
        /// The color depth in number of bits per pixel to save the image with.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Drawing.Image"/>.
        /// </returns>
        public override Image Save(Stream stream, Image image, BitDepth bitDepth)
        {
            // Tiffs can be saved with different bit depths.
            using (var encoderParameters = GetEncoderParameters(bitDepth))
            {
                image.Save(stream, GetCodecInfo(), encoderParameters);
            }

            return image;
        }

        private ImageCodecInfo GetCodecInfo()
        {
            return this.imageCodecInfo ?? (this.imageCodecInfo = Array.Find(
                ImageCodecInfo.GetImageEncoders(),
                ici => ici.MimeType.Equals(this.MimeType, StringComparison.OrdinalIgnoreCase)));
        }

        // TODO: Jpeg uses FormatUtilities.GetEncodingParameters
        private EncoderParameters GetEncoderParameters(BitDepth bitDepth)
        {
            long colorDepth = (long)bitDepth;
            if (colorDepth == (long)BitDepth.Bit2)
            {
                // No pixel formats actually support 2 bits per pixel so choose the next one up.
                colorDepth = (long)BitDepth.Bit4;
            }

            long compression = (long)(colorDepth == (long)BitDepth.Bit1 ? EncoderValue.CompressionCCITT4 : EncoderValue.CompressionLZW);

            var encoderParameters = new EncoderParameters(2);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, compression);
            encoderParameters.Param[1] = new EncoderParameter(Encoder.ColorDepth, colorDepth);

            return encoderParameters;
        }
    }
}