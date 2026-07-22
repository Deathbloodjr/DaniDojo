using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Assets.Audio
{
    public enum DaniDojoAudio
    {
        BgmDaniOdaiPrimalLoop,
        BgmDaniResultPrimalLoop,
        SeDaniOdaiIntro,
        SeDaniPlayDisqualify,
        SeDaniPlayFusumaClose,
        SeDaniPlayFusumaOpen,
        SeDaniResultPartialPlate,
        VoiceDaniOdaiDecide,
        VoiceDaniResultAdvance,
    }

    public static class DaniDojoAudioExtensions
    {
        public static string GetFileName(this DaniDojoAudio sound)
        {
            return sound switch
            {
                DaniDojoAudio.BgmDaniOdaiPrimalLoop => "bgm_daniodai_primal_loop.bin",
                DaniDojoAudio.BgmDaniResultPrimalLoop => "bgm_daniresult_primal_loop.bin",
                DaniDojoAudio.SeDaniOdaiIntro => "se_daniodai_intro.bin",
                DaniDojoAudio.SeDaniPlayDisqualify => "se_daniplay_disqualify.bin",
                DaniDojoAudio.SeDaniPlayFusumaClose => "se_daniplay_fusuma_close.bin",
                DaniDojoAudio.SeDaniPlayFusumaOpen => "se_daniplay_fusuma_open.bin",
                DaniDojoAudio.SeDaniResultPartialPlate => "se_daniresult_partial_plate.bin",
                DaniDojoAudio.VoiceDaniOdaiDecide => "voice_daniodai_decide.bin",
                DaniDojoAudio.VoiceDaniResultAdvance => "voice_daniresult_advance.bin",
                _ => LogMissingAudio(sound)
            };
        }

        private static string LogMissingAudio(DaniDojoAudio sound)
        {
            ModLogger.Log($"No file mapped for audio enum: {sound}", LogType.Error);
            return string.Empty;
        }
    }
}
