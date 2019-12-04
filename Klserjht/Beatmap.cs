using System;
using System.Collections.Generic;
using System.IO;

namespace Klserjht
{
    class Beatmap
    {
        public string Artist { get; private set; }
        public string Title { get; private set; }
        public string Creator { get; private set; }
        public string Version { get; private set; }
        public int Id { get; private set; }

        public static Dictionary<int, Beatmap> Dictionary { get; private set; }

        private static string _installationPath;

        public static void Initialise()
        {
            _installationPath =
                (string) Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\\osu\\DefaultIcon", string.Empty,
                    string.Empty);

            if (string.IsNullOrWhiteSpace(_installationPath)) return;
            _installationPath = _installationPath.Substring(1).Remove(_installationPath.Length - 12);

            ReadDatabase();

            var watcher = new FileSystemWatcher
            {
                Filter = "*.osu",
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite,
                Path = $"{_installationPath}\\Songs\\"
            };

            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var beatmap = new Beatmap();

            using (var reader = new StreamReader(File.OpenRead(e.FullPath)))
            {
                while (!reader.EndOfStream)
                {
                    var tokens = reader.ReadLine()?.Split(':');
                    if (tokens == null || tokens[0] == "BeatmapSetID") break;
                    if (tokens.Length < 2) continue;

                    var value = tokens[1];
                    for (var i = 2; i < tokens.Length; i++) value += ':' + tokens[i];

                    if (tokens[0] == "Artist") beatmap.Artist = value;
                    if (tokens[0] == "Title") beatmap.Title = value;
                    if (tokens[0] == "Creator") beatmap.Creator = value;
                    if (tokens[0] == "Version") beatmap.Version = value;
                    if (tokens[0] == "BeatmapID") beatmap.Id = int.Parse(value);
                }
            }

            if (!Dictionary.ContainsKey(beatmap.Id)) Dictionary.Add(beatmap.Id, beatmap);
        }

        private static void ReadDatabase()
        {
            Dictionary = new Dictionary<int, Beatmap>();
            var path = $"{_installationPath}\\osu!.db";

            using (var reader = new DatabaseReader(File.OpenRead(path)))
            {
                var clientVersion = reader.ReadInt32(); // The version of the client that last wrote to the database.

                reader.ReadInt32(); // The amount of folders inside the "Songs" directory.
                reader.ReadBoolean(); // Whether or not the account is locked.
                reader.ReadInt64(); // The account unlock date (0 if the account is unlocked).
                reader.ReadString(); // The account name.

                var beatmapCount = reader.ReadInt32(); // The amount of beatmaps (.osu files) inside the "Songs" directory.

                for (var i = 0; i < beatmapCount; i++)
                {
                    var beatmap = new Beatmap();

                    beatmap.Artist = reader.ReadString(); // The artist of the beatmap (romanised).
                    reader.ReadString(); // The artist of the beatmap (native).

                    beatmap.Title = reader.ReadString(); // The title of the beatmap (romanised).
                    reader.ReadString(); // The title of the beatmap (native).

                    beatmap.Creator = reader.ReadString(); // The creator (mapper) of the beatmap.
                    beatmap.Version = reader.ReadString(); // The version (difficulty) of the beatmap.

                    reader.ReadString(); // The audio file name of the beatmap.
                    reader.ReadString(); // The checksum of the file.
                    reader.ReadString(); // The beatmap file name (the .osu file) of the beatmap.

                    reader.ReadByte(); // The ranked status of the beatmap.
                    reader.ReadUInt16(); // The circle count of the beatmap.
                    reader.ReadUInt16(); // The slider count of the beatmap.
                    reader.ReadUInt16(); // The spinner count of the beatmap.
                    reader.ReadUInt64(); // The last modified date of the beatmap.

                    if (clientVersion >= 20140609)
                    {
                        reader.ReadSingle(); // The approach rate of the beatmap.
                        reader.ReadSingle(); // The circle size of the beatmap.
                        reader.ReadSingle(); // The health drain rate of the beatmap.
                        reader.ReadSingle(); // The overall difficulty of the beatmap.
                    }
                    else
                    {
                        reader.ReadByte(); // The approach rate of the beatmap.
                        reader.ReadByte(); // The circle size of the beatmap.
                        reader.ReadByte(); // The health drain rate of the beatmap.
                        reader.ReadByte(); // The overall difficulty of the beatmap.
                    }

                    reader.ReadDouble(); // The slider velocity of the beatmap.

                    if (clientVersion >= 20140609) // The star / difficulty information.
                    {
                        for (var j = 0; j < 4; j++)
                        {
                            var length = reader.ReadInt32(); // ?

                            for (var k = 0; k < length; k++)
                            {
                                reader.ReadObject(); // ?
                                reader.ReadObject(); // ?
                            }
                        }
                    }

                    reader.ReadInt32(); // The drain time (in seconds) of the beatmap.
                    reader.ReadInt32(); // The total time (in milliseconds) of the beatmap.
                    reader.ReadInt32(); // The audio preview time (in milliseconds) of the beatmap.

                    var timingPointCount = reader.ReadInt32(); // The amount of timing points in the beatmap.

                    for (var j = 0; j < timingPointCount; j++)
                    {
                        reader.ReadDouble(); // The milliseconds per beat of the timing point.
                        reader.ReadDouble(); // The time (in milliseconds) that the timing point starts from.
                        reader.ReadBoolean(); // Whether or not the timing point inherits from the timing point before it.
                    }

                    beatmap.Id = reader.ReadInt32(); // The beatmap id of the beatmap.
                    reader.ReadInt32(); // The beatmap set id of the beatmap.
                    reader.ReadInt32(); // ?

                    reader.ReadByte(); // The best grade on the beatmap (standard).
                    reader.ReadByte(); // The best grade on the beatmap (taiko).
                    reader.ReadByte(); // The best grade on the beatmap (catch the beat).
                    reader.ReadByte(); // The best grade on the beatmap (mania).

                    reader.ReadInt16(); // The local offset of the beatmap.
                    reader.ReadSingle(); // The stack leniency of the beatmap.
                    reader.ReadByte(); // The game mode of the beatmap (standard, taiko, catch the beat or mania).
                    reader.ReadString(); // The source of the beatmap.
                    reader.ReadString(); // The tags of the beatmap.
                    reader.ReadInt16(); // The online offset of the beatmap.
                    reader.ReadString(); // The title of the beatmap with styling.
                    reader.ReadBoolean(); // Whether or not the beatmap has been played.
                    reader.ReadInt64(); // The last played date of the beatmap (0 if it hasn't been played before).
                    reader.ReadBoolean(); // Whether or not the beatmap is of the osz2 file format.
                    reader.ReadString(); // The folder name of the beatmap.
                    reader.ReadInt64(); // The date of the last check against the online osu! version.

                    reader.ReadBoolean(); // Whether or not the "ignore beatmap hitsounds" setting is selected.
                    reader.ReadBoolean(); // Whether or not the "ignore beatmap skin" setting is selected.
                    reader.ReadBoolean(); // Whether or not the "disable storyboard" setting is selected.
                    reader.ReadBoolean(); // Whether or not the "disable video" setting is selected.
                    reader.ReadBoolean(); // Whether or not there are visual override(s).

                    if (clientVersion < 20140609) reader.ReadInt16(); // The background dim of the beatmap.
                    reader.ReadInt32(); // ?

                    reader.ReadByte(); // The scroll speed on mania of the beatmap.

                    if (!Dictionary.ContainsKey(beatmap.Id)) Dictionary.Add(beatmap.Id, beatmap);
                }
            }
        }

        private class DatabaseReader : BinaryReader
        {
            public DatabaseReader(Stream input) : base(input)
            {
            }

            public override string ReadString()
            {
                return ReadByte() == 11 ? base.ReadString() : null;
            }

            public object ReadObject()
            {
                var value = ReadByte();
                int count;

                switch (value)
                {
                    case 1:
                        return ReadBoolean();
                    case 2:
                        return ReadByte();
                    case 3:
                        return ReadUInt16();
                    case 4:
                        return ReadUInt32();
                    case 5:
                        return ReadUInt64();
                    case 6:
                        return ReadSByte();
                    case 7:
                        return ReadInt16();
                    case 8:
                        return ReadInt32();
                    case 9:
                        return ReadInt64();
                    case 10:
                        return ReadChar();
                    case 11:
                        return ReadString();
                    case 12:
                        return ReadSingle();
                    case 13:
                        return ReadDouble();
                    case 14:
                        return ReadDecimal();
                    case 15:
                        return ReadInt64();
                    case 16:
                        count = ReadInt32();

                        if (count > 0) return ReadBytes(count);
                        if (count < 0) return null;

                        return new byte[0];
                    case 17:
                        count = ReadInt32();

                        if (count > 0) return ReadChars(count);
                        if (count < 0) return null;

                        return new byte[0];
                    case 18:
                        throw new NotImplementedException();
                    default:
                        return null;
                }
            }
        }
    }
}