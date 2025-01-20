using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

public class SoundManager
{
   /// <summary>
   /// MP3 player => AudioSource
   /// MP3 음원 => AudioClip
   /// 관객(귀) => AudioListener or AudioMixer (볼륨 조절을 위해 Audio Mixer를 사용할 예정)
   /// </summary>
   private AudioSource[] audioSources = new AudioSource[(int)Define.Sound.MaxCount];
   private Dictionary<string, AudioClip> sfxAudioClips = new Dictionary<string, AudioClip>(); //Key = audiou clip의 path
   public AudioMixer audioMixer;
   private AudioMixerGroup[] audioMixerGroups;

   public void Init()
   {
      //AudioMixer 설정
      if (audioMixer == null)
      {
         audioMixer =  Managers.Resource.Load<AudioMixer>("Sounds/MasterAudioMixer");
         audioMixerGroups = audioMixer.FindMatchingGroups("Master");
         
         //소리 데이터 저장 및 로드 - 데이터 연결 완료(0116)
         //1. 저장된 Setting 파일이 있는지 확인한다.
         //있으면 로드
         if (Managers.Data.HasSaveDataFile(Define.SaveKey.SettingData))
         {
            Managers.Game.SettingData = Managers.Data.LoadData<SettingData>(Define.SaveKey.SettingData);
         }
         
         //2. 없으면 초기화 파일 로드
         else
         {
            Managers.Game.SettingData = Managers.Data.LoadInitData<SettingData>(Define.SaveKey.SettingData);
            
            //초기화 후 저장 경로에 해당 데이터를 저장한다.
            Managers.Data.SaveData<SettingData>(Define.SaveKey.SettingData, Managers.Game.SettingData);
         }
        
         
         //4. 사운드 셋팅 적용
         SetBgmVolume(Managers.Game.SettingData.audioVolume.bgm);
         SetSfxVolume(Managers.Game.SettingData.audioVolume.sfx);
         
         // if (PlayerPrefs.HasKey("BgmVolume"))
         // {
         //    SetBgmVolume(PlayerPrefs.GetFloat("BgmVolume"));
         // }
         // else
         // {
         //    SetBgmVolume(1.0f);
         // }
         //
         // if (PlayerPrefs.HasKey("SfxVolume"))
         // {
         //    SetSfxVolume(PlayerPrefs.GetFloat("SfxVolume"));
         // }
         // else
         // {
         //    PlayerPrefs.SetFloat("SfxVolume",1.0f);
         //    SetSfxVolume(1.0f);
         // }
        
      }
      
      //AudioSource 생성 & 할당
      GameObject root = GameObject.Find("@Sound");
      if (root == null)
      {
         root = new GameObject { name = "@Sound" };
         Object.DontDestroyOnLoad(root);

         string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
         for (int i = 0; i < soundNames.Length - 1; i++)
         {
            GameObject go = new GameObject { name = soundNames[i] };
            audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = root.transform;
         }

      }
      
      //bgm 연속 재생
      audioSources[(int)Define.Sound.Bgm].loop = true;
      
      
      //output 할당
      audioSources[(int)Define.Sound.Bgm].outputAudioMixerGroup = audioMixerGroups[1];
      audioSources[(int)Define.Sound.Sfx].outputAudioMixerGroup = audioMixerGroups[2];
      
   }
   
   
   public void SetBgmVolume(float volume)
   {
      PlayerPrefs.SetFloat("BgmVolume", volume);
      PlayerPrefs.Save();
      float bgmVolume = Mathf.Log10(volume) * 20;
      audioMixer.SetFloat("BGM", bgmVolume);
   }

   public void SetSfxVolume(float volume)
   {
      PlayerPrefs.SetFloat("SfxVolume", volume);
      PlayerPrefs.Save();
      float sfxVolume = Mathf.Log10(volume) * 20;
      audioMixer.SetFloat("SFX", sfxVolume);
   }
   


   public void Clear()
   {
      // //audioSource 모두 stop & audioClip 빼기
      // foreach (AudioSource audioSource in audioSources)
      // {
      //    audioSource.clip = null;
      //    audioSource.Stop();
      // }
      
      //audioClip Dictionary 비우기
      sfxAudioClips.Clear();
   }

   /// <summary>
   /// bgm은 하나만 재생, sfx는 중첩 가능하도록 play one shot으로 재생된다.
   /// </summary>
   /// <param name="audioClip"></param>
   /// <param name="type"></param>
   public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Sfx)
   {
      if (audioClip == null) return;
      
      //BGM인 경우
      if (type == Define.Sound.Bgm)
      {
         AudioSource audioSource = audioSources[(int)Define.Sound.Bgm];
         if (audioSource.isPlaying)
         {
            if (audioSource.clip == audioClip)
            {
               return;
               //같은 곡을 재생하는 경우 무시한다.
            }
            audioSource.Stop();
         }
         audioSource.clip = audioClip;
         audioSource.Play();
      }
      //SFX인 경우
      else
      {
         AudioSource audioSource = audioSources[(int)Define.Sound.Sfx];
         audioSource.PlayOneShot(audioClip);
      }
   }
   

   public void Play(string path, Define.Sound type = Define.Sound.Sfx)
   {
      AudioClip audioClip = GetOrAddAudioClip(path, type);
      Play(audioClip, type);
   }

   AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Sfx)
   {
      if (path.Contains("Sounds/") == false)
      {
         path = $"Sounds/{path}"; //Resource/Sounds 폴더 안에 리소스가 위치하도록 
      }

      AudioClip audioClip = null;
      if (type == Define.Sound.Bgm)
      {
         audioClip = Managers.Resource.Load<AudioClip>(path);
      }
      else
      {
         //Dictionary에 있는지 확인하고 없으면 가져온다.
         if (sfxAudioClips.TryGetValue(path, out audioClip) == false)
         {
            audioClip = Managers.Resource.Load<AudioClip>(path);
            sfxAudioClips.Add(path,audioClip);
         }
      }

      if (audioClip == null)
      {
         Debug.Log($"Audio Clip is Missing! Can't find audio clip : {path}");
      }

      return audioClip;
   }
   
   
   
}

