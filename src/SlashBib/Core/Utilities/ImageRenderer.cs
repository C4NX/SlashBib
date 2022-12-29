using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    public class ImageRenderer
    {
        private bool _alpha;
        private IList<IRendererMutator> _mutators;

        public ImageRenderer(bool useAlpha) 
        {
            _alpha = useAlpha;
            _mutators = new List<IRendererMutator>();
        }

        public async Task<Image> RenderAsync(Size size)
        {
            Image output = _alpha ? new Image<Rgba32>(size.Width, size.Height) : new Image<Rgb24>(size.Width, size.Height);

            output.Mutate((x) =>
            {
                for (int i = 0; i < _mutators.Count; i++)
                {
                    _mutators[i].Mutate(size, x);
                }
            });

            return output;
        }

        public async Task<Stream> RenderStreamAsync(Size size, IImageEncoder encoder)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (var image = await RenderAsync(size))
            {
                await image.SaveAsync(memoryStream, encoder);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
