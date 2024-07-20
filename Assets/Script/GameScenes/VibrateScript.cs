using UnityEngine;

public class VibrateScript
{
    private bool isVibrating = false; //�o�C�u���[�V�����̏�Ԃ�ǐՂ���t���O
    private static VibrateScript instance; //VibrateScript�N���X�̃C���X�^���X
    private AndroidJavaObject vibrator; //Vibrator�T�[�r�X

    //VibrateScript�N���X�̃C���X�^���X���擾����v���p�e�B
    public static VibrateScript Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new VibrateScript();
            }
            return instance;
        }
    }

    //Android�̗v�f���擾����֐�
    private void elementsAcquisition()
    {
        // Android��UnityPlayer�N���X���擾
        AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        // ���݂̃A�N�e�B�r�e�B���擾
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // Vibrator�T�[�r�X���擾
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }

    //Android�̃o�C�u���[�V���������s���郁�\�b�h
    public void StartVibration()
    {
        //�o�C�u���[�V��������~���Ă���A����Android�v���b�g�t�H�[���ł���ꍇ�Ɏ��s
        if (!isVibrating && Application.platform == RuntimePlatform.Android)
        {
            elementsAcquisition();
            //�o�C�u���[�V�������J�n
            vibrator.Call("vibrate", 1000);
            isVibrating = true; //�o�C�u���[�V�������J�n���ꂽ���Ƃ��L�^
        }
        else
        {
            Debug.Log("Android�ł͂Ȃ��v���b�g�t�H�[���ł�"); //Android�v���b�g�t�H�[���łȂ��ꍇ�Ƀ��O���o��
        }
    }

    //�o�C�u���[�V�������~���郁�\�b�h
    public void StopVibration()
    {
        //�o�C�u���[�V���������s���ł���A����Android�v���b�g�t�H�[���ł���ꍇ�Ɏ��s
        if (isVibrating && Application.platform == RuntimePlatform.Android)
        {
            elementsAcquisition();
            //�o�C�u���[�V�������~
            vibrator.Call("cancel");
            isVibrating = false; //�o�C�u���[�V��������~���ꂽ���Ƃ��L�^
        }
        else
        {
            Debug.Log("Android�ł͂Ȃ��v���b�g�t�H�[���ł�"); //Android�v���b�g�t�H�[���łȂ��ꍇ�Ƀ��O���o��
        }
    }
}