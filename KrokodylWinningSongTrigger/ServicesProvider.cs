using SoundFingerprinting.Audio;
using SoundFingerprinting.InMemory;
using SoundFingerprinting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrokodylWinningSongTrigger
{
    internal class ServicesProvider
    {
        internal static readonly IModelService ModelService = new InMemoryModelService();
        internal static readonly IAudioService AudioService = new SoundFingerprintingAudioService();
    }
}
