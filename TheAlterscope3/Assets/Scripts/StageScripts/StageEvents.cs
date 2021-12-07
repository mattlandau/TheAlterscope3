using UnityEngine;

namespace TheAlterscope2
{
    public class StageEvents : MonoBehaviour, IHasSetup
    {
        AudioSource _boothAudioSource;
        AudioClip _slideOut;
        AudioClip _slideIn;
        ParticleSystem _boothSlideDust;
        ParticleSystem _avatarOneSlideDust;
        ParticleSystem _avatarTwoSlideDust;
        ILogger _logger;

        ConversationStateMachine _conversationStateMachine;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _boothAudioSource = myDependencies.BoothSlideAudioSource;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _slideOut = myDependencies.MySlideOutAudio;
            _slideIn = myDependencies.MySlideInAudio;

            _boothSlideDust = myDependencies.BoothSlideDust;
            _avatarOneSlideDust = myDependencies.AvatarOneSlideDust;
            _avatarTwoSlideDust = myDependencies.AvatarTwoSlideDust;
        }

        public void PlayRevolveOutEffects()
        {
            _boothAudioSource.clip = _slideOut;
            _boothAudioSource.Play();
            PlayDust();
        }

        void PlayDust()
        {
            _boothSlideDust.Play();
            _avatarOneSlideDust.Play();
            _avatarTwoSlideDust.Play();
        }

        public void StopDust()
        {
            _boothSlideDust.Stop();
            _avatarOneSlideDust.Stop();
            _avatarTwoSlideDust.Stop();
        }

        public void PlayRevolveInEffects()
        {
            _boothAudioSource.clip = _slideIn;
            _boothAudioSource.Play();
            PlayDust();
        }

        public void RevolveDone()
        {
            _conversationStateMachine.MoveNext(Command.BeginDelayBeforePlayback);
            StopDust();
        }
    }
}