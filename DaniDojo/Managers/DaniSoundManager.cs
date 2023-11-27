using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Managers
{
    internal class DaniSoundManager
    {
        static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        static private CriPlayer bgmPlayer;
        static private Dictionary<string, CriPlayer> players = new Dictionary<string, CriPlayer>();

        static public void SetupBgm(string fileName, bool isLoop)
        {
            if (bgmPlayer == null)
            {
                bgmPlayer = new CriPlayer(false);
                bgmPlayer.Player.AttachFader();
            }
            bgmPlayer.CueSheetName = "song_trance";
            BgmPlayerLoad(fileName, isLoop);
        }

        static private CriPlayer SetupSound(string fileName, bool isLoop)
        {
            if (players.ContainsKey(fileName))
            {
                return players[fileName];
            }
            var Player = new CriPlayer(false);
            Player.Player.AttachFader();
            Player.CueSheetName = "intro";
            SoundPlayerLoad(Player, fileName, isLoop);
            players.Add(fileName, Player);
            return Player;
        }

        static public void PlaySound(string fileName, bool isLoop)
        {
            CriPlayer player;
            if (!players.ContainsKey(fileName))
            {
                player = SetupSound(fileName, isLoop);
            }
            else
            {
                player = players[fileName];
            }
            Plugin.Instance.StartCoroutine(PlayProcess(player, "song_trance", TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Se)));
        }

        static public void StopSound(string fileName)
        {
            if (players.ContainsKey(fileName))
            {
                if (players[fileName].Player.GetStatus() == CriAtomExPlayer.Status.Stop)
                {
                    return;
                }
                players[fileName].Stop(true);
            }
        }

        static private bool BgmPlayerLoad(string fileName, bool isLoop)
        {
            return SoundPlayerLoad(bgmPlayer, fileName, isLoop);
        }

        static private bool SoundPlayerLoad(CriPlayer player, string fileName, bool isLoop)
        {
            player.IsPrepared = false;
            player.LoadingState = CriPlayer.LoadingStates.Loading;
            player.IsLoadSucceed = false;
            player.LoadTime = -1f;
            player.loadStartTime = Time.time;
            player.Player.Loop(isLoop);
            if (player.CueSheetName == "")
            {
                player.LoadingState = CriPlayer.LoadingStates.Finished;
                return false;
            }
            if (File.Exists(Path.Combine(AssetFilePath, "Sound", fileName)))
            {
                player.CueSheet = CriAtom.AddCueSheetAsync(player.CueSheetName, File.ReadAllBytes(Path.Combine(AssetFilePath, "Sound", fileName)), null, null, false);
            }
            if (player.CueSheet != null)
            {
                return true;
            }
            player.LoadingState = CriPlayer.LoadingStates.Finished;
            return false;
        }

        static public void PlayBgm()
        {
            if (bgmPlayer == null)
            {
                return;
            }
            Plugin.Instance.StartCoroutine(PlayProcess(bgmPlayer, "song_trance", TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Bgm)));
        }

        static public void StopBgm()
        {
            StopSound(bgmPlayer);
        }

        static public void StopSound(CriPlayer player)
        {
            if (player == null)
            {
                return;
            }
            if (player.Player.GetStatus() == CriAtomExPlayer.Status.Stop)
            {
                return;
            }
            player.Player.Stop(false);
        }

        static private IEnumerator PlayProcess(CriPlayer player, string cueKey, float volume)
        {
            //Plugin.LogInfo("PlayProcess: " + cueKey);
            yield return new WaitWhile(() => player.CheckLoading());
            StopSound(player);
            player.Player.SetVolume(volume);
            player.Player.UpdateAll();
            player.Play(cueKey);
            yield break;
        }
    }
}
