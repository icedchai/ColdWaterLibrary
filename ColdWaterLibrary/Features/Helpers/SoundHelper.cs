namespace ColdWaterLibrary.Audio.Features.Helpers
{
    using Exiled.API.Features;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// Manages and plays sounds in groups.
    /// </summary>
    public class SoundHelper
    {
        public static Dictionary<string, List<string>> RegisteredNoiseLookupTable { get; internal set; } = new();

        /// <summary>
        /// Registers a sound group.
        /// </summary>
        /// <param name="soundGroupToRegister">The sound group to register.</param>
        public static void RegisterSoundGroup(Dictionary<string, List<string>> soundGroupToRegister)
        {
            foreach (KeyValuePair<string, List<string>> kvp in soundGroupToRegister)
            {
                foreach (string path in kvp.Value)
                {
                    string newPath = path;
                    if (path.Contains("{defaultpath}"))
                    {
                        string[] pathsSplit = path.Split(new string[] { "{defaultpath}" }, StringSplitOptions.RemoveEmptyEntries);
                        newPath = Path.Combine(Paths.Plugins, pathsSplit[0]);
                    }

                    AudioClipStorage.LoadClip(newPath, path);
                    Log.Debug($"registered {newPath} under {kvp.Key}");
                }

                if (!RegisteredNoiseLookupTable.TryGetValue(kvp.Key, out _))
                {
                    RegisteredNoiseLookupTable.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    Log.Warn($"Attempted to register a sound group that already exists!");
                }
            }
        }

        /// <summary>
        /// De-registers a sound group.
        /// </summary>
        /// <param name="soundGroupToUnregister">The sound group to de-register.</param>
        /// <returns><see cref="true"/> if the sound group was successfully unregistered, otherwise <see cref="false"/>.</returns>
        public static bool UnregisterSoundGroup(string soundGroupToUnregister)
        {
            return RegisteredNoiseLookupTable.Remove(soundGroupToUnregister);
        }

        /// <summary>
        /// Unregisters a specific noise from a sound group.
        /// </summary>
        /// <param name="soundGroup">The sound group to remove this from.</param>
        /// <param name="soundPath">The sound path to remove.</param>
        /// <returns><see cref="true"/> if the sound group was successfully unregistered, otherwise <see cref="false"/>.</returns>
        public static bool UnregisterNoise(string soundGroup, string soundPath)
        {
            return RegisteredNoiseLookupTable.TryGetValue(soundGroup, out _) && RegisteredNoiseLookupTable[soundGroup].Remove(soundPath);
        }

        private static long AudioPlayers { get; set; } = long.MinValue;

        /// <summary>
        /// Plays a sound in the specified position.
        /// </summary>
        /// <param name="pos">The position to play the sound at.</param>
        /// <param name="sound">The name of the sound to play.</param>
        /// <param name="audioPlayer">The audioplayer playing the noise.</param>
        /// <param name="speaker">The speaker playing the noise.</param>
        /// <param name="loop">Whether to loop the noise.</param>
        /// <param name="spatial">Whether to make the sound spatial.</param>
        public static void PlaySound(Vector3 pos, string sound, out AudioPlayer audioPlayer, out Speaker speaker, bool loop = false, bool spatial = true, float minDistance = 5, float maxDistance = 15f)
        {
            AudioPlayers++;
            audioPlayer = AudioPlayer.CreateOrGet($"icedchqi_audioplayer_{AudioPlayers}", destroyWhenAllClipsPlayed: true);
            speaker = audioPlayer.AddSpeaker("main", position: pos, isSpatial: spatial, minDistance: minDistance, maxDistance: maxDistance);
            audioPlayer.AddClip(RegisteredNoiseLookupTable[sound] is null ? sound : RegisteredNoiseLookupTable[sound].RandomItem(), loop: loop);
        }

        public static void PlaySound(string sound, bool loop = false) => PlaySound(Vector3.zero, sound, out AudioPlayer _, out Speaker _, loop, false, 50000, 50000);

        public static void PlaySound(Vector3 pos, string sound) => PlaySound(pos, sound, out AudioPlayer _, out Speaker _);

        public static void PlaySound(Player player, string sound, bool loop = false)
        {
            PlaySound(player.Position, sound, out AudioPlayer _, out Speaker speaker, loop: loop);
            speaker.transform.parent = player.Transform;
        }
    }
}
