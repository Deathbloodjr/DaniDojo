using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if IL2CPP
using Il2CppInterop.Runtime;
#endif

namespace DaniDojo.Assets.Audio
{
    internal class DaniSoundManager
    {
        private static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        private static CriPlayer bgmPlayer;
        private static Dictionary<string, CriPlayer> players = new Dictionary<string, CriPlayer>();

        private static string GetCueNameFromFileName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private static void SetupBgm(string fileName)
        {
            if (bgmPlayer == null)
            {
                bgmPlayer = new CriPlayer(false);
                bgmPlayer.Player.AttachFader();
            }
            bgmPlayer.CueSheetName = GetCueNameFromFileName(fileName);
            SoundPlayerLoad(bgmPlayer, fileName);
        }

        public static void PlayBgm(DaniDojoAudio audio)
        {
            string fileName = audio.GetFileName();
            SetupBgm(fileName);

            if (bgmPlayer == null)
            {
                return;
            }
            string cueName = GetCueNameFromFileName(fileName);
            Plugin.Instance.StartCoroutine(PlayProcess(bgmPlayer, cueName, TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Bgm)));
        }

        public static void StopBgm()
        {
            if (bgmPlayer != null)
            {
                if (bgmPlayer.Player.GetStatus() == CriAtomExPlayer.Status.Stop)
                {
                    return;
                }
                bgmPlayer.Stop(true);
            }
        }

        private static CriPlayer SetupSound(string fileName)
        {
            if (players.ContainsKey(fileName))
            {
                return players[fileName];
            }
            var Player = new CriPlayer(false);
            Player.Player.AttachFader();
            string cueName = GetCueNameFromFileName(fileName);
            Player.CueSheetName = cueName;
            SoundPlayerLoad(Player, fileName);
            players.Add(fileName, Player);
            return Player;
        }

        public static void PlaySound(DaniDojoAudio audio)
        {
            string fileName = audio.GetFileName();
            CriPlayer player;
            if (!players.ContainsKey(fileName))
            {
                player = SetupSound(fileName);
            }
            else
            {
                player = players[fileName];
            }
            string cueName = GetCueNameFromFileName(fileName);
            Plugin.Instance.StartCoroutine(PlayProcess(player, cueName, TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Se)));
        }

        public static void StopSound(DaniDojoAudio audio)
        {
            string fileName = audio.GetFileName();
            if (players.ContainsKey(fileName))
            {
                if (players[fileName].Player.GetStatus() == CriAtomExPlayer.Status.Stop)
                {
                    return;
                }
                players[fileName].Stop(true);
            }
        }


        private static void SoundPlayerLoad(CriPlayer player, string fileName)
        {
            player.IsPrepared = false;
            player.LoadingState = CriPlayer.LoadingStates.Loading;
            player.IsLoadSucceed = false;
            player.LoadTime = -1f;
            player.loadStartTime = Time.time;
            //player.Player.Loop(isLoop);
            if (player.CueSheetName == "")
            {
                player.LoadingState = CriPlayer.LoadingStates.Finished;
                return;
            }
            if (File.Exists(Path.Combine(AssetFilePath, "Sound", fileName)))
            {
                player.CueSheet = CriAtom.AddCueSheetAsync(player.CueSheetName, File.ReadAllBytes(Path.Combine(AssetFilePath, "Sound", fileName)), null, null, false);
            }
            if (player.CueSheet != null)
            {
                return;
            }
            player.LoadingState = CriPlayer.LoadingStates.Finished;
            return;
        }

        private static IEnumerator PlayProcess(CriPlayer player, string cueKey, float volume)
        {
#if IL2CPP
            yield return new WaitWhile(DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(() => player.CheckLoading()));
#else
            yield return new WaitWhile(player.CheckLoading);
#endif
            //StopSound(player);
            player.Player.SetVolume(volume);
            player.Player.UpdateAll();
            player.Play(0);
            yield break;
        }
    }
}
