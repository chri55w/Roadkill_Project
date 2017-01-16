using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    public List<AudioClip> Death = new List<AudioClip>();
    public List<AudioClip> Banter= new List<AudioClip>();
    public List<AudioClip> DriverSelect = new List<AudioClip>();
    public List<AudioClip> Newlap = new List<AudioClip>();
    public List<AudioClip> RaceStart = new List<AudioClip>();
    public List<AudioClip> RaceEnd = new List<AudioClip>();
    public List<AudioClip> WrongWay = new List<AudioClip>();
    public AudioSource MusicSource;
    public static SoundManager instance = null;

    public enum e_PlayAudio {Death, Banter, DriverSelect, Newlap, RaceStart, RaceEnd, WrongWay} 
    

    // Use this for initialization
    void StartAwake () {
        //MusicSource = GetComponent<AudioSource>();
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
	}

	public void PlaySingle (e_PlayAudio p_EffectType)
    {
        switch(p_EffectType)
        {
            case e_PlayAudio.Death:
                Debug.Log(MusicSource.isPlaying);
                MusicSource.Stop();
                MusicSource.clip = Death[(int)Random.Range(0.0f, (float)Death.Count)];
                MusicSource.Play();
                break;
            case e_PlayAudio.Banter:
                MusicSource.clip = Banter[(int)Random.Range(0.0f, (float)Banter.Count)];
                MusicSource.Play();
                break;
            case e_PlayAudio.DriverSelect:
                MusicSource.clip = DriverSelect[(int)Random.Range(0.0f, (float)DriverSelect.Count)];
                MusicSource.Play();
                break;
            case e_PlayAudio.Newlap:
                MusicSource.clip = Newlap[(int)Random.Range(0.0f, (float)Newlap.Count)];
                MusicSource.Play();
                break;
            case e_PlayAudio.RaceStart:
                MusicSource.clip = RaceStart[(int)Random.Range(0.0f, (float)RaceStart.Count)];
                MusicSource.Play();
                break;
            case e_PlayAudio.RaceEnd:
                MusicSource.clip = RaceEnd[(int)Random.Range(0.0f, (float)RaceEnd.Count)];
                MusicSource.Play();
                break;
            case e_PlayAudio.WrongWay:
                MusicSource.clip = WrongWay[(int)Random.Range(0.0f, (float)WrongWay.Count)];
                MusicSource.Play();
                break;
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
