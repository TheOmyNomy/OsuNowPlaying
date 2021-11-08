using ProcessMemoryDataFinder.API;

namespace Klserjht
{
    public class OsuMemoryReader : OsuMemoryDataProvider.OsuMemoryReader
    {
        private const int _artistId = 100, _titleId = 101, _creatorId = 102, _versionId = 103;
        private const int _currentBeatmapDataId = 1;

        public OsuMemoryReader()
        {
            Signatures.Add(_artistId, new SigEx
            {
                ParentSig = Signatures[_currentBeatmapDataId],
                PointerOffsets = { 0x18 }
            });
            Signatures.Add(_titleId, new SigEx
            {
                ParentSig = Signatures[_currentBeatmapDataId],
                PointerOffsets = { 0x24 }
            });
            Signatures.Add(_creatorId, new SigEx
            {
                ParentSig = Signatures[_currentBeatmapDataId],
                PointerOffsets = { 0x78 }
            });
            Signatures.Add(_versionId, new SigEx
            {
                ParentSig = Signatures[_currentBeatmapDataId],
                PointerOffsets = { 0xAC }
            });
        }

        public string ReadArtist() => GetString(_artistId);
        public string ReadTitle() => GetString(_titleId);
        public string ReadCreator() => GetString(_creatorId);
        public string ReadVersion() => GetString(_versionId);
    }
}