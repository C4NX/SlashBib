using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    public interface IRendererMutator
    {
        void Mutate(Size imageSize, IImageProcessingContext processingContext);
    }
}
