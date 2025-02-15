﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatBase.cs" company="James Jackson-South">
//   Copyright (c) James Jackson-South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   The supported format base implement this class when building a supported format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Imaging.Formats
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// The supported format base. Implement this class when building a supported format.
    /// </summary>
    public abstract class FormatBase : ISupportedImageFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class.
        /// </summary>
        protected FormatBase() => this.Quality = 90;

        /// <summary>
        /// Gets the file headers.
        /// </summary>
        public abstract byte[][] FileHeaders { get; }

        /// <summary>
        /// Gets the list of file extensions.
        /// </summary>
        public abstract string[] FileExtensions { get; }

        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public abstract string MimeType { get; }

        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public string DefaultExtension => this.MimeType.Replace("image/", string.Empty);

        /// <summary>
        /// Gets the file format of the image. 
        /// </summary>
        public abstract ImageFormat ImageFormat { get; }

        /// <summary>
        /// Gets or sets the quality of output for images.
        /// </summary>
        public int Quality { get; set; } = 100;

        /// <summary>
        /// Applies the given processor the current image.
        /// </summary>
        /// <param name="processor">The processor delegate.</param>
        /// <param name="factory">The <see cref="ImageFactory" />.</param>
        public virtual void ApplyProcessor(Func<ImageFactory, Image> processor, ImageFactory factory) => factory.Image = processor.Invoke(factory);

        /// <summary>
        /// Decodes the image to process.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="T:System.IO.stream" /> containing the image information.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Drawing.Image" />.
        /// </returns>
        public virtual Image Load(Stream stream)
        {
            // Keep the colors but don't validate the data. The windows decoders are robust.
            return Image.FromStream(stream, true, false);
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
        public virtual Image Save(Stream stream, Image image, BitDepth bitDepth)
        {
            image.Save(stream, this.ImageFormat);
            return image;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ISupportedImageFormat format))
            {
                return false;
            }

            return this.MimeType.Equals(format.MimeType) && this.Quality.Equals(format.Quality);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.MimeType.GetHashCode();
                return (hashCode * 397) ^ this.Quality.GetHashCode();
            }
        }
    }
}
